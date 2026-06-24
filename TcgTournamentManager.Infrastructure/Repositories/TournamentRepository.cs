using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class TournamentRepository : ITournamentRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;
    private readonly ICurrentOrgContext _orgContext;

    public TournamentRepository(TournamentDbContext db, StoredProcedureExecutor sp, ICurrentOrgContext orgContext)
    {
        _db = db;
        _sp = sp;
        _orgContext = orgContext;
    }

    public async Task<Tournament?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Tournaments.FirstOrDefaultAsync(t => t.Id == id && t.OrgCD == _orgContext.OrgCD, ct);

    public async Task<Tournament?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default) =>
        await _db.Tournaments
            .Include(t => t.TournamentPlayers).ThenInclude(tp => tp.Player)
            .Include(t => t.Rounds).ThenInclude(r => r.Matches)
            .FirstOrDefaultAsync(t => t.Id == id && t.OrgCD == _orgContext.OrgCD, ct);

    public async Task<IReadOnlyList<Tournament>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Tournaments
            .Where(t => t.OrgCD == _orgContext.OrgCD)
            .OrderByDescending(t => t.EventDate)
            .ToListAsync(ct);

    public async Task<Tournament?> GetActiveTournamentAsync(CancellationToken ct = default) =>
        await _db.Tournaments
            .Where(t => t.OrgCD == _orgContext.OrgCD &&
                        (t.Status == TournamentStatus.InProgress || t.Status == TournamentStatus.Registration))
            .OrderByDescending(t => t.SyncVersion)
            .FirstOrDefaultAsync(ct);

    public async Task<Tournament> CreateAsync(Tournament tournament, CancellationToken ct = default)
    {
        var id = await _sp.ExecuteWithIntOutputAsync("usp_Tournament_Insert", "@Id", ct,
            StoredProcedureExecutor.Str("@Name", tournament.Name, 200),
            StoredProcedureExecutor.Str("@GameTitle", tournament.GameTitle, 100),
            StoredProcedureExecutor.DateTimeParam("@EventDate", tournament.EventDate),
            StoredProcedureExecutor.Str("@Organizer", tournament.Organizer, 200),
            StoredProcedureExecutor.Str("@Venue", tournament.Venue, 300),
            StoredProcedureExecutor.Int("@TotalSwissRounds", tournament.TotalSwissRounds),
            StoredProcedureExecutor.Int("@TopCutSize", (int)tournament.TopCutSize),
            StoredProcedureExecutor.Int("@FirstRoundPairingMode", (int)tournament.FirstRoundPairingMode),
            StoredProcedureExecutor.Int("@MatchFormat", (int)tournament.MatchFormat),
            StoredProcedureExecutor.Bit("@HasElimination", tournament.HasElimination),
            StoredProcedureExecutor.IntNull("@EliminationLossCount", tournament.EliminationLossCount),
            StoredProcedureExecutor.Int("@Status", (int)tournament.Status),
            StoredProcedureExecutor.Int("@CurrentRound", tournament.CurrentRound),
            StoredProcedureExecutor.Str("@OrgCD", tournament.OrgCD, 50),
            StoredProcedureExecutor.OutInt("@Id"));

        tournament.Id = id;
        return (await GetByIdAsync(id, ct)) ?? tournament;
    }

    public async Task UpdateAsync(Tournament tournament, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_Tournament_Update", ct,
            StoredProcedureExecutor.Int("@Id", tournament.Id),
            StoredProcedureExecutor.Str("@Name", tournament.Name, 200),
            StoredProcedureExecutor.Str("@GameTitle", tournament.GameTitle, 100),
            StoredProcedureExecutor.DateTimeParam("@EventDate", tournament.EventDate),
            StoredProcedureExecutor.Str("@Organizer", tournament.Organizer, 200),
            StoredProcedureExecutor.Str("@Venue", tournament.Venue, 300),
            StoredProcedureExecutor.Int("@TotalSwissRounds", tournament.TotalSwissRounds),
            StoredProcedureExecutor.Int("@TopCutSize", (int)tournament.TopCutSize),
            StoredProcedureExecutor.Int("@FirstRoundPairingMode", (int)tournament.FirstRoundPairingMode),
            StoredProcedureExecutor.Int("@MatchFormat", (int)tournament.MatchFormat),
            StoredProcedureExecutor.Bit("@HasElimination", tournament.HasElimination),
            StoredProcedureExecutor.IntNull("@EliminationLossCount", tournament.EliminationLossCount),
            StoredProcedureExecutor.Int("@Status", (int)tournament.Status),
            StoredProcedureExecutor.Int("@CurrentRound", tournament.CurrentRound));
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_Tournament_SoftDelete", ct,
            StoredProcedureExecutor.Int("@Id", id));
    }
}