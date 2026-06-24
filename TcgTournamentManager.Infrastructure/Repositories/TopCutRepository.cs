using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class TopCutRepository : ITopCutRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;

    public TopCutRepository(TournamentDbContext db, StoredProcedureExecutor sp)
    {
        _db = db;
        _sp = sp;
    }

    public async Task<IReadOnlyList<TopCutBracket>> GetBracketsAsync(int tournamentId, CancellationToken ct = default) =>
        await _db.TopCutBrackets
            .Include(b => b.PlayerA!).ThenInclude(tp => tp.Player)
            .Include(b => b.PlayerB!).ThenInclude(tp => tp.Player)
            .Include(b => b.Winner!).ThenInclude(tp => tp.Player)
            .Include(b => b.Match)
            .Where(b => b.TournamentId == tournamentId)
            .OrderBy(b => b.Round)
            .ThenBy(b => b.MatchPosition)
            .ToListAsync(ct);

    public async Task CreateBracketsAsync(IEnumerable<TopCutBracket> brackets, CancellationToken ct = default)
    {
        var list = brackets.ToList();
        var nextLinks = new List<(TopCutBracket from, TopCutBracket to)>();

        foreach (var bracket in list)
        {
            if (bracket.NextBracket != null)
            {
                nextLinks.Add((bracket, bracket.NextBracket));
                bracket.NextBracket = null;
            }
        }

        foreach (var bracket in list)
        {
            var id = await _sp.ExecuteWithIntOutputAsync("usp_TopCutBracket_Insert", "@Id", ct,
                StoredProcedureExecutor.Int("@TournamentId", bracket.TournamentId),
                StoredProcedureExecutor.Int("@Round", (int)bracket.Round),
                StoredProcedureExecutor.Int("@MatchPosition", bracket.MatchPosition),
                StoredProcedureExecutor.IntNull("@PlayerAId", bracket.PlayerAId),
                StoredProcedureExecutor.IntNull("@PlayerBId", bracket.PlayerBId),
                StoredProcedureExecutor.IntNull("@WinnerId", bracket.WinnerId),
                StoredProcedureExecutor.IntNull("@NextBracketId", bracket.NextBracketId),
                StoredProcedureExecutor.Bit("@IsComplete", bracket.IsComplete),
                StoredProcedureExecutor.OutInt("@Id"));

            bracket.Id = id;
        }

        foreach (var (from, to) in nextLinks)
        {
            from.NextBracketId = to.Id;
            await _sp.ExecuteAsync("usp_TopCutBracket_Update", ct,
                StoredProcedureExecutor.Int("@Id", from.Id),
                StoredProcedureExecutor.IntNull("@PlayerAId", from.PlayerAId),
                StoredProcedureExecutor.IntNull("@PlayerBId", from.PlayerBId),
                StoredProcedureExecutor.IntNull("@WinnerId", from.WinnerId),
                StoredProcedureExecutor.IntNull("@NextBracketId", from.NextBracketId),
                StoredProcedureExecutor.Bit("@IsComplete", from.IsComplete));
        }
    }

    public async Task<TopCutBracket?> GetBracketByIdAsync(int bracketId, CancellationToken ct = default) =>
        await _db.TopCutBrackets
            .Include(b => b.PlayerA!).ThenInclude(tp => tp.Player)
            .Include(b => b.PlayerB!).ThenInclude(tp => tp.Player)
            .Include(b => b.Winner!).ThenInclude(tp => tp.Player)
            .Include(b => b.Match)
            .FirstOrDefaultAsync(b => b.Id == bracketId, ct);

    public async Task UpdateBracketAsync(TopCutBracket bracket, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_TopCutBracket_Update", ct,
            StoredProcedureExecutor.Int("@Id", bracket.Id),
            StoredProcedureExecutor.IntNull("@PlayerAId", bracket.PlayerAId),
            StoredProcedureExecutor.IntNull("@PlayerBId", bracket.PlayerBId),
            StoredProcedureExecutor.IntNull("@WinnerId", bracket.WinnerId),
            StoredProcedureExecutor.IntNull("@NextBracketId", bracket.NextBracketId),
            StoredProcedureExecutor.Bit("@IsComplete", bracket.IsComplete));
    }

    public async Task DeleteForTournamentAsync(int tournamentId, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_TopCutBracket_DeleteByTournament", ct,
            StoredProcedureExecutor.Int("@TournamentId", tournamentId));
    }
}