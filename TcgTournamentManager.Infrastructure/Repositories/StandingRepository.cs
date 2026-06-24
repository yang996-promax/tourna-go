using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class StandingRepository : IStandingRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;
    public StandingRepository(TournamentDbContext db, StoredProcedureExecutor sp, ICurrentOrgContext _)
    {
        _db = db;
        _sp = sp;
    }

    public async Task<IReadOnlyList<Standing>> GetStandingsAsync(int tournamentId, CancellationToken ct = default) =>
        await _db.Standings
            .Include(s => s.TournamentPlayer).ThenInclude(tp => tp.Player)
            .Where(s => s.TournamentId == tournamentId)
            .OrderBy(s => s.Rank)
            .ToListAsync(ct);

    public async Task UpsertStandingsAsync(IEnumerable<Standing> standings, CancellationToken ct = default)
    {
        var list = standings.ToList();
        if (list.Count == 0) return;

        var tournamentId = list[0].TournamentId;
        await DeleteForTournamentAsync(tournamentId, ct);

        foreach (var standing in list)
        {
            var id = await _sp.ExecuteWithIntOutputAsync("usp_Standing_Insert", "@Id", ct,
                StoredProcedureExecutor.Int("@TournamentId", standing.TournamentId),
                StoredProcedureExecutor.Int("@TournamentPlayerId", standing.TournamentPlayerId),
                StoredProcedureExecutor.Int("@Rank", standing.Rank),
                StoredProcedureExecutor.Int("@MatchPoints", standing.MatchPoints),
                StoredProcedureExecutor.Int("@GameWins", standing.GameWins),
                StoredProcedureExecutor.Int("@GameLosses", standing.GameLosses),
                StoredProcedureExecutor.DecimalParam("@OMWPercent", standing.OMWPercent),
                StoredProcedureExecutor.DecimalParam("@GWPercent", standing.GWPercent),
                StoredProcedureExecutor.DecimalParam("@OGWPercent", standing.OGWPercent),
                StoredProcedureExecutor.Int("@MatchesPlayed", standing.MatchesPlayed),
                StoredProcedureExecutor.Int("@MatchesWon", standing.MatchesWon),
                StoredProcedureExecutor.Int("@MatchesLost", standing.MatchesLost),
                StoredProcedureExecutor.Int("@MatchesDrawn", standing.MatchesDrawn),
                StoredProcedureExecutor.Str("@OrgCD", standing.OrgCD, 50),
                StoredProcedureExecutor.OutInt("@Id"));

            standing.Id = id;
        }
    }

    public async Task DeleteForTournamentAsync(int tournamentId, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_Standing_DeleteByTournament", ct,
            StoredProcedureExecutor.Int("@TournamentId", tournamentId));
    }
}