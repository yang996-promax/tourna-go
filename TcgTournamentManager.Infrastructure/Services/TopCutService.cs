using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Services;

public class TopCutService : ITopCutService
{
    private readonly ITournamentRepository _tournamentRepo;
    private readonly ITopCutRepository _topCutRepo;
    private readonly IStandingsService _standingsService;
    private readonly IMatchRepository _matchRepo;
    private readonly IAuditLogRepository _auditRepo;

    public TopCutService(
        ITournamentRepository tournamentRepo,
        ITopCutRepository topCutRepo,
        IStandingsService standingsService,
        IMatchRepository matchRepo,
        IAuditLogRepository auditRepo)
    {
        _tournamentRepo = tournamentRepo;
        _topCutRepo = topCutRepo;
        _standingsService = standingsService;
        _matchRepo = matchRepo;
        _auditRepo = auditRepo;
    }

    public async Task CompleteSwissAsync(int tournamentId, CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(tournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        if (tournament.TopCutSize == TopCutSize.None)
            throw new InvalidOperationException("This tournament has no top cut configured.");

        await EnsureSwissCompleteAsync(tournament, ct);
        await _auditRepo.LogAsync("CompleteSwiss", "Tournament", tournamentId, "Swiss rounds ended, ready for top cut", null, ct);
    }

    public async Task<TopCutBracketTreeDto> GenerateTopCutAsync(int tournamentId, CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(tournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        if (tournament.TopCutSize == TopCutSize.None)
            throw new InvalidOperationException("Tournament has no top cut configured.");

        await EnsureSwissCompleteAsync(tournament, ct);

        var existing = await _topCutRepo.GetBracketsAsync(tournamentId, ct);
        if (existing.Count > 0)
            throw new InvalidOperationException("Top cut bracket already exists.");

        var standings = await _standingsService.CalculateStandingsAsync(tournamentId, phase: StandingPhase.Swiss, ct: ct);
        var cutSize = (int)tournament.TopCutSize;

        if (standings.Count < cutSize)
            throw new InvalidOperationException($"Need at least {cutSize} players for Top {cutSize}. Currently {standings.Count}.");

        var seeded = standings.Take(cutSize).ToList();
        var brackets = BuildBracketTree(tournamentId, cutSize, seeded.Select(s => s.TournamentPlayerId).ToList());
        await _topCutRepo.CreateBracketsAsync(brackets, ct);

        tournament.Status = TournamentStatus.TopCutInProgress;
        await _tournamentRepo.UpdateAsync(tournament, ct);
        await _auditRepo.LogAsync("GenerateTopCut", "Tournament", tournamentId, $"Top {cutSize} bracket created", null, ct);

        return await GetBracketTreeAsync(tournamentId, ct);
    }

    public async Task<TopCutBracketTreeDto> GetBracketTreeAsync(int tournamentId, CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(tournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        var brackets = await _topCutRepo.GetBracketsAsync(tournamentId, ct);
        var standings = await _standingsService.CalculateStandingsAsync(tournamentId, phase: StandingPhase.Swiss, ct: ct);
        var cutSize = (int)tournament.TopCutSize;

        var qualified = standings
            .Take(cutSize > 0 ? cutSize : standings.Count)
            .Select((s, i) => new TopCutQualifiedPlayerDto(
                i + 1, s.TournamentPlayerId, s.PlayerName, s.PlayerNumber, s.MatchPoints))
            .ToList();

        var canGenerate = await CanGenerateTopCutAsync(tournament, brackets.Count, ct);
        var statusMessage = GetStatusMessage(tournament, brackets.Count, canGenerate);

        var dtos = brackets.Select(MapBracket).ToList();

        var final = brackets.FirstOrDefault(b => b.Round == TopCutRound.Final && b.IsComplete);
        string? championName = null;
        int? championId = null;
        if (final?.Winner != null)
        {
            championName = final.Winner.Player.Name;
            championId = final.WinnerId;
        }

        return new TopCutBracketTreeDto(dtos, qualified, championName, championId, canGenerate, statusMessage);
    }

    public async Task<TopCutBracketDto> EnterTopCutResultAsync(int bracketId, int winnerId, CancellationToken ct = default)
    {
        var bracket = await _topCutRepo.GetBracketByIdAsync(bracketId, ct)
            ?? throw new InvalidOperationException("Bracket not found.");

        if (bracket.IsComplete)
            throw new InvalidOperationException("This bracket match is already complete.");

        if (bracket.PlayerAId != winnerId && bracket.PlayerBId != winnerId)
            throw new InvalidOperationException("Winner must be one of the bracket players.");

        if (!bracket.PlayerAId.HasValue || !bracket.PlayerBId.HasValue)
            throw new InvalidOperationException("Both players must be assigned before entering a result.");

        bracket.WinnerId = winnerId;
        bracket.IsComplete = true;
        await _topCutRepo.UpdateBracketAsync(bracket, ct);

        if (bracket.NextBracketId.HasValue)
        {
            var next = await _topCutRepo.GetBracketByIdAsync(bracket.NextBracketId.Value, ct);
            if (next != null)
            {
                if (bracket.MatchPosition % 2 == 1)
                    next.PlayerAId = winnerId;
                else
                    next.PlayerBId = winnerId;

                await _topCutRepo.UpdateBracketAsync(next, ct);
            }
        }
        else
        {
            var tournament = await _tournamentRepo.GetByIdAsync(bracket.TournamentId, ct);
            if (tournament != null)
            {
                tournament.Status = TournamentStatus.Completed;
                await _tournamentRepo.UpdateAsync(tournament, ct);
            }
        }

        await RecordTopCutMatchAsync(bracket, winnerId, ct);
        await _auditRepo.LogAsync("TopCutResult", "TopCutBracket", bracketId, $"Winner: {winnerId}", null, ct);

        var updated = await _topCutRepo.GetBracketByIdAsync(bracketId, ct);
        return MapBracket(updated!);
    }

    private async Task RecordTopCutMatchAsync(TopCutBracket bracket, int winnerId, CancellationToken ct)
    {
        if (bracket.Match != null) return;

        var tournament = await _tournamentRepo.GetByIdAsync(bracket.TournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        var roundNumber = GetTopCutRoundNumber(bracket.Round, (int)tournament.TopCutSize);
        var round = await _matchRepo.GetRoundAsync(bracket.TournamentId, roundNumber, ct, RoundType.TopCut);
        if (round == null)
        {
            round = new Round
            {
                TournamentId = bracket.TournamentId,
                RoundNumber = roundNumber,
                RoundType = RoundType.TopCut,
                IsComplete = false
            };
            round = await _matchRepo.CreateRoundAsync(round, ct);
        }

        var isPlayerAWin = winnerId == bracket.PlayerAId;
        var match = new Match
        {
            RoundId = round.Id,
            TableNumber = bracket.MatchPosition,
            PlayerAId = bracket.PlayerAId,
            PlayerBId = bracket.PlayerBId,
            IsComplete = true,
            WinnerId = winnerId,
            TopCutBracketId = bracket.Id
        };
        await _matchRepo.CreateMatchesAsync([match], ct);

        match.Result = new MatchResult
        {
            MatchId = match.Id,
            ResultType = MatchResultType.Win2_0,
            PlayerAGameWins = isPlayerAWin ? 2 : 0,
            PlayerBGameWins = isPlayerAWin ? 0 : 2,
            PlayerAMatchPoints = isPlayerAWin ? 3 : 0,
            PlayerBMatchPoints = isPlayerAWin ? 0 : 3
        };
        await _matchRepo.UpsertMatchResultAsync(match.Result, ct);
        await _matchRepo.UpdateMatchAsync(match, ct);

        await TryCompleteTopCutRoundAsync(bracket.TournamentId, bracket.Round, round.Id, ct);
    }

    private async Task TryCompleteTopCutRoundAsync(int tournamentId, TopCutRound topCutRound, int roundId, CancellationToken ct)
    {
        var brackets = await _topCutRepo.GetBracketsAsync(tournamentId, ct);
        var roundBrackets = brackets.Where(b => b.Round == topCutRound).ToList();
        if (roundBrackets.Count == 0 || !roundBrackets.All(b => b.IsComplete))
            return;

        var round = await _matchRepo.GetRoundByIdAsync(roundId, ct);
        if (round == null || round.IsComplete)
            return;

        round.IsComplete = true;
        round.CompletedAt = DateTime.UtcNow;
        await _matchRepo.UpdateRoundAsync(round, ct);
    }

    private async Task EnsureSwissCompleteAsync(Tournament tournament, CancellationToken ct)
    {
        if (tournament.CurrentRound < tournament.TotalSwissRounds)
            throw new InvalidOperationException("All Swiss rounds must be completed before generating top cut.");

        if (tournament.Status == TournamentStatus.SwissComplete ||
            tournament.Status == TournamentStatus.TopCutInProgress ||
            tournament.Status == TournamentStatus.Completed)
            return;

        var lastRound = await _matchRepo.GetRoundAsync(tournament.Id, tournament.TotalSwissRounds, ct);
        if (lastRound == null || !lastRound.IsComplete)
            throw new InvalidOperationException("The final Swiss round must be fully completed before top cut.");

        tournament.Status = TournamentStatus.SwissComplete;
        await _tournamentRepo.UpdateAsync(tournament, ct);
    }

    private async Task<bool> CanGenerateTopCutAsync(Tournament tournament, int existingBrackets, CancellationToken ct)
    {
        if (tournament.TopCutSize == TopCutSize.None || existingBrackets > 0)
            return false;

        if (tournament.CurrentRound < tournament.TotalSwissRounds)
            return false;

        if (tournament.Status == TournamentStatus.SwissComplete ||
            tournament.Status == TournamentStatus.TopCutInProgress)
            return true;

        var lastRound = await _matchRepo.GetRoundAsync(tournament.Id, tournament.TotalSwissRounds, ct);
        return lastRound?.IsComplete == true;
    }

    private static string GetStatusMessage(Tournament tournament, int bracketCount, bool canGenerate)
    {
        if (tournament.TopCutSize == TopCutSize.None)
            return "This event has no top cut configured.";

        if (bracketCount > 0 && tournament.Status == TournamentStatus.Completed)
            return "Top cut complete. Champion crowned!";

        if (bracketCount > 0)
            return "Top cut in progress. Click a player to record the match winner.";

        if (canGenerate)
            return $"Swiss is complete. Generate the Top {(int)tournament.TopCutSize} single-elimination bracket.";

        if (tournament.CurrentRound < tournament.TotalSwissRounds)
            return $"Complete all {tournament.TotalSwissRounds} Swiss rounds first (currently round {tournament.CurrentRound}).";

        return "Complete all matches in the final Swiss round before generating top cut.";
    }

    public static List<TopCutBracket> BuildBracketTree(int tournamentId, int cutSize, List<int> seededPlayerIds)
    {
        var brackets = new List<TopCutBracket>();
        var rounds = GetRoundSequence(cutSize);
        var roundBrackets = new Dictionary<TopCutRound, List<TopCutBracket>>();

        foreach (var round in rounds)
        {
            var matchCount = (int)round / 2;
            roundBrackets[round] = new List<TopCutBracket>();

            for (var pos = 0; pos < matchCount; pos++)
            {
                var bracket = new TopCutBracket
                {
                    TournamentId = tournamentId,
                    Round = round,
                    MatchPosition = pos + 1
                };
                brackets.Add(bracket);
                roundBrackets[round].Add(bracket);
            }
        }

        for (var i = 0; i < rounds.Count - 1; i++)
        {
            var current = roundBrackets[rounds[i]];
            var next = roundBrackets[rounds[i + 1]];

            for (var j = 0; j < current.Count; j++)
                current[j].NextBracket = next[j / 2];
        }

        var firstRound = rounds[0];
        SeedFirstRound(roundBrackets[firstRound], seededPlayerIds, cutSize);

        return brackets;
    }

    private static void SeedFirstRound(List<TopCutBracket> firstBrackets, List<int> playerIds, int cutSize)
    {
        var seedOrder = cutSize switch
        {
            4 => new[] { 0, 3, 1, 2 },
            8 => new[] { 0, 7, 3, 4, 1, 6, 2, 5 },
            16 => new[] { 0, 15, 7, 8, 3, 12, 4, 11, 1, 14, 6, 9, 2, 13, 5, 10 },
            _ => Enumerable.Range(0, playerIds.Count).ToArray()
        };

        for (var i = 0; i < firstBrackets.Count; i++)
        {
            var aIdx = seedOrder[i * 2];
            var bIdx = seedOrder[i * 2 + 1];
            if (aIdx < playerIds.Count) firstBrackets[i].PlayerAId = playerIds[aIdx];
            if (bIdx < playerIds.Count) firstBrackets[i].PlayerBId = playerIds[bIdx];
        }
    }

    public static int GetTopCutRoundNumber(TopCutRound round, int cutSize)
    {
        var sequence = GetRoundSequence(cutSize);
        var index = sequence.IndexOf(round);
        if (index < 0)
            throw new InvalidOperationException($"Round {round} is not part of a Top {cutSize} bracket.");

        return index + 1;
    }

    private static List<TopCutRound> GetRoundSequence(int cutSize) => cutSize switch
    {
        4 => [TopCutRound.SemiFinal, TopCutRound.Final],
        8 => [TopCutRound.QuarterFinal, TopCutRound.SemiFinal, TopCutRound.Final],
        16 => [TopCutRound.RoundOf16, TopCutRound.QuarterFinal, TopCutRound.SemiFinal, TopCutRound.Final],
        _ => [TopCutRound.Final]
    };

    private static TopCutBracketDto MapBracket(TopCutBracket b)
    {
        var playable = !b.IsComplete && b.PlayerAId.HasValue && b.PlayerBId.HasValue;
        return new TopCutBracketDto(
            b.Id,
            b.Round,
            b.MatchPosition,
            b.PlayerAId,
            b.PlayerA?.Player.Name,
            b.PlayerBId,
            b.PlayerB?.Player.Name,
            b.WinnerId,
            b.Winner?.Player.Name,
            b.NextBracketId,
            b.IsComplete,
            playable,
            b.Match?.Id);
    }
}