using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class ByeHistoryRepository : IByeHistoryRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;

    public ByeHistoryRepository(TournamentDbContext db, StoredProcedureExecutor sp)
    {
        _db = db;
        _sp = sp;
    }

    public async Task<bool> HasReceivedByeAsync(int tournamentId, int tournamentPlayerId, CancellationToken ct = default) =>
        await _db.ByeHistories.AnyAsync(b => b.TournamentId == tournamentId && b.TournamentPlayerId == tournamentPlayerId, ct);

    public async Task RecordByeAsync(ByeHistory bye, CancellationToken ct = default)
    {
        var id = await _sp.ExecuteWithIntOutputAsync("usp_ByeHistory_Insert", "@Id", ct,
            StoredProcedureExecutor.Int("@TournamentId", bye.TournamentId),
            StoredProcedureExecutor.Int("@TournamentPlayerId", bye.TournamentPlayerId),
            StoredProcedureExecutor.Int("@RoundNumber", bye.RoundNumber),
            StoredProcedureExecutor.DateTimeParam("@AssignedAt", bye.AssignedAt),
            StoredProcedureExecutor.OutInt("@Id"));

        bye.Id = id;
    }

    public async Task<IReadOnlyList<int>> GetPlayersWithByeAsync(int tournamentId, CancellationToken ct = default) =>
        await _db.ByeHistories
            .Where(b => b.TournamentId == tournamentId)
            .Select(b => b.TournamentPlayerId)
            .Distinct()
            .ToListAsync(ct);
}