using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Services;

public class MatchResultService : IMatchResultService
{
    private readonly ITournamentRepository _tournamentRepo;
    private readonly IMatchRepository _matchRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IStandingsService _standingsService;
    private readonly IAuditLogRepository _auditRepo;

    public MatchResultService(
        ITournamentRepository tournamentRepo,
        IMatchRepository matchRepo,
        IPlayerRepository playerRepo,
        IStandingsService standingsService,
        IAuditLogRepository auditRepo)
    {
        _tournamentRepo = tournamentRepo;
        _matchRepo = matchRepo;
        _playerRepo = playerRepo;
        _standingsService = standingsService;
        _auditRepo = auditRepo;
    }

    public async Task<MatchDto> EnterResultAsync(int matchId, EnterMatchResultRequest request, CancellationToken ct = default)
    {
        var match = await _matchRepo.GetMatchByIdAsync(matchId, ct)
            ?? throw new InvalidOperationException("Match not found.");

        if (match.IsComplete && match.Result != null && !match.IsBye)
            throw new InvalidOperationException("Match result already recorded.");

        if (match.IsBye)
            throw new InvalidOperationException("Bye matches are auto-completed.");

        var tournament = await _tournamentRepo.GetByIdAsync(match.Round.TournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        var (aGw, bGw, aMp, bMp) = CalculatePoints(request.ResultType, match.IsBye, tournament.MatchFormat);

        int? winnerId = request.ResultType switch
        {
            MatchResultType.Win2_0 or MatchResultType.Win2_1 => match.PlayerAId,
            MatchResultType.Loss0_2 or MatchResultType.Loss1_2 => match.PlayerBId,
            MatchResultType.Draw => null,
            MatchResultType.ByeWin => match.PlayerAId,
            _ => null
        };

        if (match.Result == null)
        {
            match.Result = new MatchResult
            {
                OrgCD = tournament.OrgCD,
                MatchId = match.Id,
                ResultType = request.ResultType,
                PlayerAGameWins = aGw,
                PlayerBGameWins = bGw,
                PlayerAMatchPoints = aMp,
                PlayerBMatchPoints = bMp
            };
        }
        else
        {
            match.Result.ResultType = request.ResultType;
            match.Result.PlayerAGameWins = aGw;
            match.Result.PlayerBGameWins = bGw;
            match.Result.PlayerAMatchPoints = aMp;
            match.Result.PlayerBMatchPoints = bMp;
            match.Result.RecordedAt = DateTime.UtcNow;
        }

        await _matchRepo.UpsertMatchResultAsync(match.Result, ct);

        match.IsComplete = true;
        match.WinnerId = winnerId;
        await _matchRepo.UpdateMatchAsync(match, ct);

        await CheckRoundComplete(match.RoundId, ct);
        await ApplyEliminationAsync(tournament, ct);
        await _standingsService.RecalculateAndPersistAsync(match.Round.TournamentId, ct);
        await _auditRepo.LogAsync("EnterResult", "Match", match.Id, request.ResultType.ToString(), null, ct);

        var updated = await _matchRepo.GetMatchByIdAsync(matchId, ct);
        return MapMatch(updated!);
    }

    public (int playerAGW, int playerBGW, int playerAMP, int playerBMP) CalculatePoints(
        MatchResultType resultType, bool isBye, MatchFormat matchFormat)
    {
        if (isBye)
            return matchFormat == MatchFormat.BO1 ? (1, 0, 3, 0) : (2, 0, 3, 0);

        if (matchFormat == MatchFormat.BO1)
        {
            return resultType switch
            {
                MatchResultType.Win2_0 => (1, 0, 3, 0),
                MatchResultType.Loss0_2 => (0, 1, 0, 3),
                MatchResultType.Draw => (0, 0, 1, 1),
                MatchResultType.ByeWin => (1, 0, 3, 0),
                _ => throw new InvalidOperationException($"Result {resultType} is not valid for BO1 matches.")
            };
        }

        return resultType switch
        {
            MatchResultType.Win2_0 => (2, 0, 3, 0),
            MatchResultType.Win2_1 => (2, 1, 3, 0),
            MatchResultType.Loss1_2 => (1, 2, 0, 3),
            MatchResultType.Loss0_2 => (0, 2, 0, 3),
            MatchResultType.Draw => (1, 1, 1, 1),
            MatchResultType.ByeWin => (2, 0, 3, 0),
            _ => (0, 0, 0, 0)
        };
    }

    private async Task ApplyEliminationAsync(Tournament tournament, CancellationToken ct)
    {
        if (!tournament.HasElimination || !tournament.EliminationLossCount.HasValue)
            return;

        var threshold = tournament.EliminationLossCount.Value;
        var matches = await _matchRepo.GetMatchesForTournamentAsync(tournament.Id, ct);
        var lossCounts = new Dictionary<int, int>();

        foreach (var match in matches.Where(m => m.IsComplete && !m.IsBye && m.PlayerAId.HasValue && m.PlayerBId.HasValue))
        {
            if (match.Result?.ResultType == MatchResultType.Draw)
                continue;

            int? loserId = match.WinnerId switch
            {
                var w when w == match.PlayerAId => match.PlayerBId,
                var w when w == match.PlayerBId => match.PlayerAId,
                _ => null
            };

            if (!loserId.HasValue)
                continue;

            lossCounts[loserId.Value] = lossCounts.GetValueOrDefault(loserId.Value) + 1;
        }

        var players = await _playerRepo.GetTournamentPlayersAsync(tournament.Id, ct);
        foreach (var player in players.Where(p => !p.IsDropped))
        {
            if (lossCounts.GetValueOrDefault(player.Id) >= threshold)
            {
                player.IsDropped = true;
                await _playerRepo.UpdateTournamentPlayerAsync(player, ct);
            }
        }
    }

    private async Task CheckRoundComplete(int roundId, CancellationToken ct)
    {
        var round = await _matchRepo.GetRoundByIdAsync(roundId, ct);
        if (round == null) return;

        if (round.Matches.All(m => m.IsComplete))
        {
            round.IsComplete = true;
            round.CompletedAt = DateTime.UtcNow;
            await _matchRepo.UpdateRoundAsync(round, ct);

            var tournament = await _tournamentRepo.GetByIdAsync(round.TournamentId, ct);
            if (tournament != null && round.RoundType == RoundType.Swiss && round.RoundNumber >= tournament.TotalSwissRounds)
            {
                tournament.Status = tournament.TopCutSize == TopCutSize.None
                    ? TournamentStatus.Completed
                    : TournamentStatus.SwissComplete;
                await _tournamentRepo.UpdateAsync(tournament, ct);
            }
        }
    }

    private static MatchDto MapMatch(Match m) => new(
        m.Id,
        m.RoundId,
        m.Round.RoundNumber,
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
            m.Result.PlayerBMatchPoints));
}