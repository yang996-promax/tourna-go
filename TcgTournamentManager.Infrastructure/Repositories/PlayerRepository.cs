using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;

    public PlayerRepository(TournamentDbContext db, StoredProcedureExecutor sp)
    {
        _db = db;
        _sp = sp;
    }

    public async Task<Player> CreatePlayerAsync(Player player, CancellationToken ct = default)
    {
        var id = await _sp.ExecuteWithIntOutputAsync("usp_Player_Insert", "@Id", ct,
            StoredProcedureExecutor.Str("@ExternalPlayerId", player.ExternalPlayerId, 50),
            StoredProcedureExecutor.Str("@Name", player.Name, 200),
            StoredProcedureExecutor.StrNull("@ContactNumber", player.ContactNumber, 50),
            StoredProcedureExecutor.OutInt("@Id"));

        player.Id = id;
        return (await GetByExternalIdAsync(player.ExternalPlayerId, ct)) ?? player;
    }

    public async Task UpdatePlayerAsync(Player player, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_Player_Update", ct,
            StoredProcedureExecutor.Int("@Id", player.Id),
            StoredProcedureExecutor.Str("@ExternalPlayerId", player.ExternalPlayerId, 50),
            StoredProcedureExecutor.Str("@Name", player.Name, 200),
            StoredProcedureExecutor.StrNull("@ContactNumber", player.ContactNumber, 50));
    }

    public async Task<Player?> GetByExternalIdAsync(string externalId, CancellationToken ct = default) =>
        await _db.Players.FirstOrDefaultAsync(p => p.ExternalPlayerId == externalId, ct);

    public async Task<TournamentPlayer?> GetTournamentPlayerAsync(int tournamentId, int tournamentPlayerId, CancellationToken ct = default) =>
        await _db.TournamentPlayers
            .Include(tp => tp.Player)
            .FirstOrDefaultAsync(tp => tp.TournamentId == tournamentId && tp.Id == tournamentPlayerId, ct);

    public async Task<IReadOnlyList<TournamentPlayer>> GetTournamentPlayersAsync(int tournamentId, CancellationToken ct = default) =>
        await _db.TournamentPlayers
            .Include(tp => tp.Player)
            .Where(tp => tp.TournamentId == tournamentId)
            .OrderBy(tp => tp.PlayerNumber)
            .ToListAsync(ct);

    public async Task<TournamentPlayer> AddToTournamentAsync(TournamentPlayer tp, CancellationToken ct = default)
    {
        var id = await _sp.ExecuteWithIntOutputAsync("usp_TournamentPlayer_Insert", "@Id", ct,
            StoredProcedureExecutor.Int("@TournamentId", tp.TournamentId),
            StoredProcedureExecutor.Int("@PlayerId", tp.PlayerId),
            StoredProcedureExecutor.Int("@PlayerNumber", tp.PlayerNumber),
            StoredProcedureExecutor.StrNull("@DeckName", tp.DeckName, 200),
            StoredProcedureExecutor.Bit("@IsDropped", tp.IsDropped),
            StoredProcedureExecutor.DateTimeParam("@RegisteredAt", tp.RegisteredAt),
            StoredProcedureExecutor.OutInt("@Id"));

        tp.Id = id;
        return (await GetTournamentPlayerAsync(tp.TournamentId, id, ct)) ?? tp;
    }

    public async Task UpdateTournamentPlayerAsync(TournamentPlayer tp, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_TournamentPlayer_Update", ct,
            StoredProcedureExecutor.Int("@Id", tp.Id),
            StoredProcedureExecutor.Int("@PlayerNumber", tp.PlayerNumber),
            StoredProcedureExecutor.StrNull("@DeckName", tp.DeckName, 200),
            StoredProcedureExecutor.Bit("@IsDropped", tp.IsDropped),
            StoredProcedureExecutor.DateTimeParam("@RegisteredAt", tp.RegisteredAt));
    }

    public async Task RemoveFromTournamentAsync(int tournamentPlayerId, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_TournamentPlayer_SoftDelete", ct,
            StoredProcedureExecutor.Int("@Id", tournamentPlayerId));
    }

    public async Task<TournamentPlayer?> GetDeletedTournamentPlayerAsync(
        int tournamentId, int playerId, CancellationToken ct = default) =>
        await _db.TournamentPlayers
            .IgnoreQueryFilters()
            .Include(tp => tp.Player)
            .FirstOrDefaultAsync(tp => tp.TournamentId == tournamentId && tp.PlayerId == playerId && tp.SyncOperation == SyncOperation.D, ct);

    public async Task<IReadOnlyList<TournamentPlayer>> SearchAsync(int tournamentId, string query, CancellationToken ct = default)
    {
        var q = query.Trim().ToLower();
        return await _db.TournamentPlayers
            .Include(tp => tp.Player)
            .Where(tp => tp.TournamentId == tournamentId &&
                (tp.Player.Name.ToLower().Contains(q) ||
                 tp.Player.ExternalPlayerId.ToLower().Contains(q) ||
                 tp.DeckName != null && tp.DeckName.ToLower().Contains(q) ||
                 tp.PlayerNumber.ToString() == q))
            .OrderBy(tp => tp.PlayerNumber)
            .ToListAsync(ct);
    }
}