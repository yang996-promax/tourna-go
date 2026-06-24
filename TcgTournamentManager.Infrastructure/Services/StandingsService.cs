using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Services;

public class StandingsService : IStandingsService
{
    private readonly ITournamentRepository _tournamentRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IMatchRepository _matchRepo;
    private readonly IStandingRepository _standingRepo;

    public StandingsService(
        ITournamentRepository tournamentRepo,
        IPlayerRepository playerRepo,
        IMatchRepository matchRepo,
        IStandingRepository standingRepo)
    {
        _tournamentRepo = tournamentRepo;
        _playerRepo = playerRepo;
        _matchRepo = matchRepo;
        _standingRepo = standingRepo;
    }

    public async Task<IReadOnlyList<StandingDto>> CalculateStandingsAsync(
        int tournamentId,
        StandingSortBy sortBy = StandingSortBy.MatchPoints,
        StandingPhase phase = StandingPhase.Overall,
        CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(tournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        var players = await _playerRepo.GetTournamentPlayersAsync(tournamentId, ct);
        var matches = await _matchRepo.GetMatchesForTournamentAsync(tournamentId, ct);
        var completed = matches
            .Where(m => m.IsComplete)
            .Where(m => phase == StandingPhase.Overall || m.Round.RoundType == RoundType.Swiss)
            .ToList();

        var stats = players.ToDictionary(
            p => p.Id,
            p => new PlayerStats
            {
                TournamentPlayerId = p.Id,
                PlayerName = p.Player.Name,
                PlayerNumber = p.PlayerNumber,
                IsDropped = p.IsDropped
            });

        var opponents = players.ToDictionary(p => p.Id, _ => new List<int>());

        foreach (var match in completed)
        {
            if (match.IsBye && match.PlayerAId.HasValue)
            {
                var tpId = match.PlayerAId.Value;
                if (!stats.ContainsKey(tpId)) continue;
                stats[tpId].MatchPoints += 3;
                stats[tpId].MatchesPlayed++;
                stats[tpId].MatchesWon++;
                continue;
            }

            if (!match.PlayerAId.HasValue || !match.PlayerBId.HasValue || match.Result == null)
                continue;

            var aId = match.PlayerAId.Value;
            var bId = match.PlayerBId.Value;
            opponents[aId].Add(bId);
            opponents[bId].Add(aId);

            var a = stats[aId];
            var b = stats[bId];
            a.GameWins += match.Result.PlayerAGameWins;
            a.GameLosses += match.Result.PlayerBGameWins;
            b.GameWins += match.Result.PlayerBGameWins;
            b.GameLosses += match.Result.PlayerAGameWins;
            a.MatchPoints += match.Result.PlayerAMatchPoints;
            b.MatchPoints += match.Result.PlayerBMatchPoints;
            a.MatchesPlayed++;
            b.MatchesPlayed++;

            if (match.Result.PlayerAMatchPoints > match.Result.PlayerBMatchPoints)
            {
                a.MatchesWon++;
                b.MatchesLost++;
            }
            else if (match.Result.PlayerBMatchPoints > match.Result.PlayerAMatchPoints)
            {
                b.MatchesWon++;
                a.MatchesLost++;
            }
            else
            {
                a.MatchesDrawn++;
                b.MatchesDrawn++;
            }
        }

        foreach (var stat in stats.Values)
        {
            stat.GWPercent = CalculateGWPercent(stat.GameWins, stat.GameLosses);
            stat.OMWPercent = CalculateOMWPercent(stat.TournamentPlayerId, opponents, stats);
            stat.OGWPercent = CalculateOGWPercent(stat.TournamentPlayerId, opponents, stats);
        }

        var sorted = stats.Values
            .Where(s => !s.IsDropped || s.MatchesPlayed > 0)
            .OrderByDescending(s => s.MatchPoints)
            .ThenByDescending(s => s.OMWPercent)
            .ThenByDescending(s => s.GWPercent)
            .ThenByDescending(s => s.OGWPercent)
            .ThenBy(s => s.PlayerNumber)
            .ToList();

        if (sortBy != StandingSortBy.MatchPoints)
        {
            sorted = sortBy switch
            {
                StandingSortBy.OMWPercent => sorted.OrderByDescending(s => s.OMWPercent).ThenByDescending(s => s.MatchPoints).ToList(),
                StandingSortBy.GWPercent => sorted.OrderByDescending(s => s.GWPercent).ThenByDescending(s => s.MatchPoints).ToList(),
                StandingSortBy.OGWPercent => sorted.OrderByDescending(s => s.OGWPercent).ThenByDescending(s => s.MatchPoints).ToList(),
                _ => sorted
            };
        }

        return sorted.Select((s, i) => new StandingDto(
            i + 1,
            s.TournamentPlayerId,
            s.PlayerName,
            s.PlayerNumber,
            s.MatchPoints,
            s.GameWins,
            s.GameLosses,
            Math.Round(s.OMWPercent * 100, 2),
            Math.Round(s.GWPercent * 100, 2),
            Math.Round(s.OGWPercent * 100, 2),
            s.MatchesPlayed,
            s.MatchesWon,
            s.MatchesLost,
            s.MatchesDrawn
        )).ToList();
    }

    public async Task RecalculateAndPersistAsync(int tournamentId, CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(tournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        var standings = await CalculateStandingsAsync(tournamentId, ct: ct);
        var entities = standings.Select(s => new Standing
        {
            OrgCD = tournament.OrgCD,
            TournamentId = tournamentId,
            TournamentPlayerId = s.TournamentPlayerId,
            Rank = s.Rank,
            MatchPoints = s.MatchPoints,
            GameWins = s.GameWins,
            GameLosses = s.GameLosses,
            OMWPercent = s.OMWPercent,
            GWPercent = s.GWPercent,
            OGWPercent = s.OGWPercent,
            MatchesPlayed = s.MatchesPlayed,
            MatchesWon = s.MatchesWon,
            MatchesLost = s.MatchesLost,
            MatchesDrawn = s.MatchesDrawn
        });
        await _standingRepo.UpsertStandingsAsync(entities, ct);
    }

    private static decimal CalculateGWPercent(int gameWins, int gameLosses)
    {
        var total = gameWins + gameLosses;
        if (total == 0) return 0.33m;
        return (decimal)gameWins / total;
    }

    private static decimal CalculateOMWPercent(int playerId, Dictionary<int, List<int>> opponents, Dictionary<int, PlayerStats> stats)
    {
        var opps = opponents.GetValueOrDefault(playerId) ?? [];
        if (opps.Count == 0) return 0.33m;

        var sum = opps.Sum(oppId =>
        {
            var opp = stats[oppId];
            if (opp.MatchesPlayed == 0) return 0.33m;
            return Math.Max(0.33m, (decimal)opp.MatchPoints / (opp.MatchesPlayed * 3));
        });

        return sum / opps.Count;
    }

    private static decimal CalculateOGWPercent(int playerId, Dictionary<int, List<int>> opponents, Dictionary<int, PlayerStats> stats)
    {
        var opps = opponents.GetValueOrDefault(playerId) ?? [];
        if (opps.Count == 0) return 0.33m;

        var sum = opps.Sum(oppId => stats[oppId].GWPercent);
        return sum / opps.Count;
    }

    private class PlayerStats
    {
        public int TournamentPlayerId { get; init; }
        public string PlayerName { get; init; } = "";
        public int PlayerNumber { get; init; }
        public bool IsDropped { get; init; }
        public int MatchPoints { get; set; }
        public int GameWins { get; set; }
        public int GameLosses { get; set; }
        public int MatchesPlayed { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MatchesDrawn { get; set; }
        public decimal OMWPercent { get; set; }
        public decimal GWPercent { get; set; }
        public decimal OGWPercent { get; set; }
    }
}