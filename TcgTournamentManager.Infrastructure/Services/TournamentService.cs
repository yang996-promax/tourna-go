using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Services;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IMatchRepository _matchRepo;
    private readonly IAuditLogRepository _auditRepo;
    private readonly ICurrentOrgContext _orgContext;

    public TournamentService(
        ITournamentRepository tournamentRepo,
        IPlayerRepository playerRepo,
        IMatchRepository matchRepo,
        IAuditLogRepository auditRepo,
        ICurrentOrgContext orgContext)
    {
        _tournamentRepo = tournamentRepo;
        _playerRepo = playerRepo;
        _matchRepo = matchRepo;
        _auditRepo = auditRepo;
        _orgContext = orgContext;
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentRequest request, CancellationToken ct = default)
    {
        ValidateEliminationSettings(request.HasElimination, request.EliminationLossCount);

        var tournament = new Tournament
        {
            OrgCD = _orgContext.OrgCD,
            Name = request.Name,
            GameTitle = request.GameTitle,
            EventDate = request.EventDate,
            Organizer = request.Organizer,
            Venue = request.Venue,
            TotalSwissRounds = request.TotalSwissRounds,
            TopCutSize = request.TopCutSize,
            FirstRoundPairingMode = request.FirstRoundPairingMode,
            MatchFormat = request.MatchFormat,
            HasElimination = request.HasElimination,
            EliminationLossCount = request.HasElimination ? request.EliminationLossCount : null,
            Status = TournamentStatus.Registration
        };

        await _tournamentRepo.CreateAsync(tournament, ct);
        await _auditRepo.LogAsync("Create", "Tournament", tournament.Id, tournament.Name, null, ct);
        return await MapAsync(tournament, ct);
    }

    public async Task<TournamentDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var t = await _tournamentRepo.GetByIdAsync(id, ct);
        return t == null ? null : await MapAsync(t, ct);
    }

    public async Task<IReadOnlyList<TournamentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var tournaments = await _tournamentRepo.GetAllAsync(ct);
        var result = new List<TournamentDto>();
        foreach (var t in tournaments)
            result.Add(await MapAsync(t, ct));
        return result;
    }

    public async Task<TournamentDto> UpdateAsync(int id, UpdateTournamentRequest request, CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        if (tournament.Status != TournamentStatus.Draft && tournament.Status != TournamentStatus.Registration)
            throw new InvalidOperationException("Cannot update tournament after it has started.");

        tournament.Name = request.Name;
        tournament.GameTitle = request.GameTitle;
        tournament.EventDate = request.EventDate;
        tournament.Organizer = request.Organizer;
        tournament.Venue = request.Venue;
        tournament.TotalSwissRounds = request.TotalSwissRounds;
        tournament.TopCutSize = request.TopCutSize;
        tournament.FirstRoundPairingMode = request.FirstRoundPairingMode;
        ValidateEliminationSettings(request.HasElimination, request.EliminationLossCount);
        tournament.MatchFormat = request.MatchFormat;
        tournament.HasElimination = request.HasElimination;
        tournament.EliminationLossCount = request.HasElimination ? request.EliminationLossCount : null;

        await _tournamentRepo.UpdateAsync(tournament, ct);
        return await MapAsync(tournament, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _tournamentRepo.DeleteAsync(id, ct);
        await _auditRepo.LogAsync("Delete", "Tournament", id, "SyncOperation=D", null, ct);
    }

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default)
    {
        var active = await _tournamentRepo.GetActiveTournamentAsync(ct);
        if (active == null)
        {
            return new DashboardDto(null, 0, 0, 0, 0, "No active tournament");
        }

        var players = await _playerRepo.GetTournamentPlayersAsync(active.Id, ct);
        var rounds = await _matchRepo.GetRoundsAsync(active.Id, swissOnly: true, ct);
        var currentRound = rounds.LastOrDefault(r => !r.IsComplete) ?? rounds.LastOrDefault();
        var matches = currentRound?.Matches ?? [];
        var completed = matches.Count(m => m.IsComplete);

        return new DashboardDto(
            await MapAsync(active, ct),
            players.Count(p => !p.IsDropped),
            active.CurrentRound,
            completed,
            matches.Count,
            active.Status.ToString());
    }

    public async Task StartTournamentAsync(int id, CancellationToken ct = default)
    {
        var tournament = await _tournamentRepo.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        var players = await _playerRepo.GetTournamentPlayersAsync(id, ct);
        var activeCount = players.Count(p => !p.IsDropped);
        var minRequired = Math.Max(2, (int)tournament.TopCutSize);

        if (activeCount < minRequired)
        {
            var reason = tournament.TopCutSize != TopCutSize.None
                ? $"At least {minRequired} players required for Top {(int)tournament.TopCutSize} (currently {activeCount})."
                : $"At least 2 players required to start (currently {activeCount}).";
            throw new InvalidOperationException(reason);
        }

        tournament.Status = TournamentStatus.InProgress;
        tournament.CurrentRound = 0;
        await _tournamentRepo.UpdateAsync(tournament, ct);
        await _auditRepo.LogAsync("Start", "Tournament", id, null, null, ct);
    }

    private async Task<TournamentDto> MapAsync(Tournament t, CancellationToken ct)
    {
        var players = await _playerRepo.GetTournamentPlayersAsync(t.Id, ct);
        return new TournamentDto(
            t.Id, t.Name, t.GameTitle, t.EventDate, t.Organizer, t.Venue,
            t.TotalSwissRounds, t.TopCutSize, t.FirstRoundPairingMode,
            t.MatchFormat, t.HasElimination, t.EliminationLossCount,
            t.Status, t.CurrentRound, players.Count, t.CreatedAt);
    }

    private static void ValidateEliminationSettings(bool hasElimination, int? eliminationLossCount)
    {
        if (!hasElimination)
            return;

        if (!eliminationLossCount.HasValue || eliminationLossCount.Value < 1)
            throw new InvalidOperationException("Elimination loss count must be at least 1 when elimination is enabled.");
    }
}