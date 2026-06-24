using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Services;

/// <summary>
/// Swiss pairing engine implementing official TCG tournament pairing rules.
/// See docs/SwissPairingAlgorithm.md for flowchart and pseudocode.
/// </summary>
public class SwissPairingService : ISwissPairingService
{
    private readonly ITournamentRepository _tournamentRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IMatchRepository _matchRepo;
    private readonly IByeHistoryRepository _byeRepo;
    private readonly IStandingsService _standingsService;
    private readonly IAuditLogRepository _auditRepo;

    public SwissPairingService(
        ITournamentRepository tournamentRepo,
        IPlayerRepository playerRepo,
        IMatchRepository matchRepo,
        IByeHistoryRepository byeRepo,
        IStandingsService standingsService,
        IAuditLogRepository auditRepo)
    {
        _tournamentRepo = tournamentRepo;
        _playerRepo = playerRepo;
        _matchRepo = matchRepo;
        _byeRepo = byeRepo;
        _standingsService = standingsService;
        _auditRepo = auditRepo;
    }

    public async Task<RoundDto> GenerateRoundAsync(int tournamentId, FirstRoundPairingMode? overrideMode = null, CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(tournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        if (tournament.Status != TournamentStatus.InProgress)
            throw new InvalidOperationException("Tournament must be in progress to generate rounds.");

        var nextRound = tournament.CurrentRound + 1;
        if (nextRound > tournament.TotalSwissRounds)
            throw new InvalidOperationException("All Swiss rounds have been generated.");

        var existingRound = await _matchRepo.GetRoundAsync(tournamentId, nextRound, ct);
        if (existingRound != null)
            throw new InvalidOperationException($"Round {nextRound} already exists.");

        if (nextRound > 1)
        {
            var prevRound = await _matchRepo.GetRoundAsync(tournamentId, tournament.CurrentRound, ct);
            if (prevRound == null || !prevRound.IsComplete)
                throw new InvalidOperationException("Previous round must be completed before generating next round.");
        }

        var players = await _playerRepo.GetTournamentPlayersAsync(tournamentId, ct);
        var activePlayers = players.Where(p => !p.IsDropped).ToList();
        if (activePlayers.Count < 2)
            throw new InvalidOperationException("At least 2 active players required.");

        var standings = await _standingsService.CalculateStandingsAsync(tournamentId, phase: StandingPhase.Swiss, ct: ct);
        var standingsMap = standings.ToDictionary(s => s.TournamentPlayerId, s => s.MatchPoints);

        var previousMatches = await _matchRepo.GetMatchesForTournamentAsync(tournamentId, ct);
        var previousPairings = new HashSet<(int, int)>();
        foreach (var m in previousMatches.Where(m => !m.IsBye && m.PlayerAId.HasValue && m.PlayerBId.HasValue))
        {
            var a = m.PlayerAId!.Value;
            var b = m.PlayerBId!.Value;
            previousPairings.Add(a < b ? (a, b) : (b, a));
        }

        var playersWithBye = (await _byeRepo.GetPlayersWithByeAsync(tournamentId, ct)).ToHashSet();

        var context = new SwissPairingContext
        {
            TournamentId = tournamentId,
            RoundNumber = nextRound,
            IsLastRound = nextRound == tournament.TotalSwissRounds,
            FirstRoundMode = overrideMode ?? tournament.FirstRoundPairingMode,
            Players = activePlayers.Select(p => new SwissPlayerState
            {
                TournamentPlayerId = p.Id,
                PlayerNumber = p.PlayerNumber,
                MatchPoints = standingsMap.GetValueOrDefault(p.Id, 0),
                IsDropped = p.IsDropped
            }).ToList(),
            PreviousPairings = previousPairings,
            PlayersWithBye = playersWithBye
        };

        var pairings = ComputePairings(context);

        var round = new Round
        {
            OrgCD = tournament.OrgCD,
            TournamentId = tournamentId,
            RoundNumber = nextRound,
            RoundType = RoundType.Swiss
        };
        await _matchRepo.CreateRoundAsync(round, ct);

        var matches = pairings.Select(p => new Match
        {
            OrgCD = tournament.OrgCD,
            RoundId = round.Id,
            TableNumber = p.TableNumber,
            PlayerAId = p.PlayerAId,
            PlayerBId = p.PlayerBId,
            IsBye = p.IsBye,
            IsComplete = p.IsBye,
            WinnerId = p.IsBye ? p.PlayerAId : null
        }).ToList();

        await _matchRepo.CreateMatchesAsync(matches, ct);

        foreach (var byeMatch in matches.Where(m => m.IsBye))
        {
            await _byeRepo.RecordByeAsync(new ByeHistory
            {
                OrgCD = tournament.OrgCD,
                TournamentId = tournamentId,
                TournamentPlayerId = byeMatch.PlayerAId!.Value,
                RoundNumber = nextRound
            }, ct);
        }

        tournament.CurrentRound = nextRound;
        await _tournamentRepo.UpdateAsync(tournament, ct);
        await _auditRepo.LogAsync("GenerateRound", "Round", round.Id, $"Round {nextRound} generated with {matches.Count} matches", null, ct);

        var createdRound = await _matchRepo.GetRoundByIdAsync(round.Id, ct);
        return MapRound(createdRound!);
    }

    public IReadOnlyList<PairingProposal> ComputePairings(SwissPairingContext context)
    {
        var players = context.Players.Where(p => !p.IsDropped).ToList();

        if (context.RoundNumber == 1)
            return PairRoundOne(players, context.FirstRoundMode);

        return PairSubsequentRounds(players, context);
    }

    private static List<PairingProposal> PairRoundOne(List<SwissPlayerState> players, FirstRoundPairingMode mode)
    {
        var ordered = mode == FirstRoundPairingMode.Seeded
            ? players.OrderBy(p => p.PlayerNumber).ToList()
            : players.OrderBy(_ => Random.Shared.Next()).ToList();

        return CreatePairingsFromOrderedList(ordered);
    }

    private List<PairingProposal> PairSubsequentRounds(List<SwissPlayerState> players, SwissPairingContext context)
    {
        var scoreGroups = players
            .GroupBy(p => p.MatchPoints)
            .OrderByDescending(g => g.Key)
            .Select(g => g.OrderByDescending(p => p.PlayerNumber).ToList())
            .ToList();

        var pool = new List<SwissPlayerState>();
        var pairings = new List<PairingProposal>();
        var paired = new HashSet<int>();

        foreach (var group in scoreGroups)
        {
            pool.AddRange(group);

            while (pool.Count >= 2)
            {
                var byeCandidate = SelectByeCandidate(pool, context.PlayersWithBye, paired);
                if (pool.Count % 2 == 1 && byeCandidate != null)
                {
                    pool.Remove(byeCandidate);
                    pairings.Add(new PairingProposal
                    {
                        TableNumber = pairings.Count + 1,
                        PlayerAId = byeCandidate.TournamentPlayerId,
                        IsBye = true
                    });
                    paired.Add(byeCandidate.TournamentPlayerId);
                    continue;
                }

                var player = pool[0];
                pool.RemoveAt(0);

                var opponent = FindBestOpponent(player, pool, context.PreviousPairings, context.IsLastRound);
                if (opponent == null)
                {
                    pool.Insert(0, player);
                    break;
                }

                pool.Remove(opponent);
                pairings.Add(new PairingProposal
                {
                    TableNumber = pairings.Count + 1,
                    PlayerAId = player.TournamentPlayerId,
                    PlayerBId = opponent.TournamentPlayerId
                });
                paired.Add(player.TournamentPlayerId);
                paired.Add(opponent.TournamentPlayerId);
            }
        }

        while (pool.Count > 0)
        {
            if (pool.Count == 1)
            {
                var lone = pool[0];
                if (!context.PlayersWithBye.Contains(lone.TournamentPlayerId))
                {
                    pairings.Add(new PairingProposal
                    {
                        TableNumber = pairings.Count + 1,
                        PlayerAId = lone.TournamentPlayerId,
                        IsBye = true
                    });
                    pool.Clear();
                    break;
                }
                throw new InvalidOperationException($"Cannot pair remaining player {lone.TournamentPlayerId} without rematch or second bye.");
            }

            var p1 = pool[0];
            pool.RemoveAt(0);
            var p2 = FindBestOpponent(p1, pool, context.PreviousPairings, context.IsLastRound)
                ?? pool[^1];

            pool.Remove(p2);
            pairings.Add(new PairingProposal
            {
                TableNumber = pairings.Count + 1,
                PlayerAId = p1.TournamentPlayerId,
                PlayerBId = p2.TournamentPlayerId
            });
        }

        return pairings;
    }

    private static SwissPlayerState? SelectByeCandidate(
        List<SwissPlayerState> pool,
        IReadOnlySet<int> playersWithBye,
        HashSet<int> alreadyPaired)
    {
        return pool
            .Where(p => !playersWithBye.Contains(p.TournamentPlayerId) && !alreadyPaired.Contains(p.TournamentPlayerId))
            .OrderBy(p => p.MatchPoints)
            .ThenBy(p => p.PlayerNumber)
            .FirstOrDefault();
    }

    private static SwissPlayerState? FindBestOpponent(
        SwissPlayerState player,
        List<SwissPlayerState> candidates,
        IReadOnlySet<(int, int)> previousPairings,
        bool isLastRound)
    {
        SwissPlayerState? best = null;
        var bestScore = int.MinValue;

        foreach (var candidate in candidates)
        {
            var key = player.TournamentPlayerId < candidate.TournamentPlayerId
                ? (player.TournamentPlayerId, candidate.TournamentPlayerId)
                : (candidate.TournamentPlayerId, player.TournamentPlayerId);

            var rematch = previousPairings.Contains(key);
            if (rematch && isLastRound)
                continue;

            var score = 0;
            if (!rematch) score += 1000;
            if (candidate.MatchPoints == player.MatchPoints) score += 100;
            score -= Math.Abs(candidate.MatchPoints - player.MatchPoints);

            if (score > bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        if (best == null && !isLastRound)
        {
            best = candidates.FirstOrDefault(c =>
            {
                var key = player.TournamentPlayerId < c.TournamentPlayerId
                    ? (player.TournamentPlayerId, c.TournamentPlayerId)
                    : (c.TournamentPlayerId, player.TournamentPlayerId);
                return !previousPairings.Contains(key);
            }) ?? candidates.FirstOrDefault();
        }

        return best;
    }

    private static List<PairingProposal> CreatePairingsFromOrderedList(List<SwissPlayerState> ordered)
    {
        var pairings = new List<PairingProposal>();
        var table = 1;

        if (ordered.Count % 2 == 1)
        {
            var byePlayer = ordered[^1];
            ordered.RemoveAt(ordered.Count - 1);
            pairings.Add(new PairingProposal
            {
                TableNumber = table++,
                PlayerAId = byePlayer.TournamentPlayerId,
                IsBye = true
            });
        }

        for (var i = 0; i < ordered.Count; i += 2)
        {
            pairings.Add(new PairingProposal
            {
                TableNumber = table++,
                PlayerAId = ordered[i].TournamentPlayerId,
                PlayerBId = ordered[i + 1].TournamentPlayerId
            });
        }

        return pairings;
    }

    private static RoundDto MapRound(Round round) => new(
        round.Id,
        round.RoundNumber,
        round.RoundType,
        round.IsComplete,
        round.Matches.OrderBy(m => m.TableNumber).Select(m => new MatchDto(
            m.Id,
            round.Id,
            round.RoundNumber,
            m.TableNumber,
            m.PlayerAId,
            m.PlayerA?.Player.Name,
            m.PlayerBId,
            m.PlayerB?.Player.Name,
            m.IsBye,
            m.IsComplete,
            m.WinnerId,
            m.Result == null ? null : new MatchResultDto(
                m.Result.ResultType,
                m.Result.PlayerAGameWins,
                m.Result.PlayerBGameWins,
                m.Result.PlayerAMatchPoints,
                m.Result.PlayerBMatchPoints)
        )).ToList());
}