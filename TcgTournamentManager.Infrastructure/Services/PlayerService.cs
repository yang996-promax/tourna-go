using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Services;

public class PlayerService : IPlayerService
{
    private readonly ITournamentRepository _tournamentRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IAuditLogRepository _auditRepo;

    public PlayerService(
        ITournamentRepository tournamentRepo,
        IPlayerRepository playerRepo,
        IAuditLogRepository auditRepo)
    {
        _tournamentRepo = tournamentRepo;
        _playerRepo = playerRepo;
        _auditRepo = auditRepo;
    }

    public async Task<PlayerDto> AddPlayerAsync(int tournamentId, CreatePlayerRequest request, CancellationToken ct = default)
    {
        _ = await _tournamentRepo.GetByIdAsync(tournamentId, ct)
            ?? throw new InvalidOperationException("Tournament not found.");

        var player = await _playerRepo.GetByExternalIdAsync(request.ExternalPlayerId, ct);
        if (player == null)
        {
            player = await _playerRepo.CreatePlayerAsync(new Player
            {
                ExternalPlayerId = request.ExternalPlayerId,
                Name = request.Name,
                ContactNumber = request.ContactNumber
            }, ct);
        }
        else
        {
            player.Name = request.Name;
            player.ContactNumber = request.ContactNumber;
            await _playerRepo.UpdatePlayerAsync(player, ct);
        }

        var existingPlayers = await _playerRepo.GetTournamentPlayersAsync(tournamentId, ct);
        if (existingPlayers.Any(tp => tp.PlayerId == player.Id))
            throw new InvalidOperationException("Player already registered for this tournament.");

        var playerNumber = request.PlayerNumber ?? (existingPlayers.Count > 0 ? existingPlayers.Max(p => p.PlayerNumber) + 1 : 1);

        var restored = await _playerRepo.GetDeletedTournamentPlayerAsync(tournamentId, player.Id, ct);
        TournamentPlayer tp;
        if (restored != null)
        {
            restored.MarkRestored();
            restored.DeckName = request.DeckName;
            restored.PlayerNumber = playerNumber;
            restored.IsDropped = false;
            restored.RegisteredAt = DateTime.UtcNow;
            await _playerRepo.UpdateTournamentPlayerAsync(restored, ct);
            tp = (await _playerRepo.GetTournamentPlayerAsync(tournamentId, restored.Id, ct))!;
        }
        else
        {
            tp = await _playerRepo.AddToTournamentAsync(new TournamentPlayer
            {
                TournamentId = tournamentId,
                PlayerId = player.Id,
                PlayerNumber = playerNumber,
                DeckName = request.DeckName
            }, ct);

            tp = (await _playerRepo.GetTournamentPlayerAsync(tournamentId, tp.Id, ct))!;
        }
        await _auditRepo.LogAsync("AddPlayer", "TournamentPlayer", tp.Id, player.Name, null, ct);
        return Map(tp);
    }

    public async Task<PlayerDto> UpdatePlayerAsync(int tournamentId, int tournamentPlayerId, UpdatePlayerRequest request, CancellationToken ct = default)
    {
        var tp = await _playerRepo.GetTournamentPlayerAsync(tournamentId, tournamentPlayerId, ct)
            ?? throw new InvalidOperationException("Player not found.");

        tp.Player.ExternalPlayerId = request.ExternalPlayerId;
        tp.Player.Name = request.Name;
        tp.Player.ContactNumber = request.ContactNumber;
        tp.DeckName = request.DeckName;
        tp.PlayerNumber = request.PlayerNumber;

        await _playerRepo.UpdatePlayerAsync(tp.Player, ct);
        await _playerRepo.UpdateTournamentPlayerAsync(tp, ct);
        return Map(tp);
    }

    public async Task RemovePlayerAsync(int tournamentId, int tournamentPlayerId, CancellationToken ct = default)
    {
        var tp = await _playerRepo.GetTournamentPlayerAsync(tournamentId, tournamentPlayerId, ct)
            ?? throw new InvalidOperationException("Player not found.");

        await _playerRepo.RemoveFromTournamentAsync(tournamentPlayerId, ct);
        await _auditRepo.LogAsync("RemovePlayer", "TournamentPlayer", tournamentPlayerId, $"SyncOperation=D; {tp.Player.Name}", null, ct);
    }

    public async Task<IReadOnlyList<PlayerDto>> GetPlayersAsync(int tournamentId, CancellationToken ct = default)
    {
        var players = await _playerRepo.GetTournamentPlayersAsync(tournamentId, ct);
        return players.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<PlayerDto>> SearchPlayersAsync(int tournamentId, string query, CancellationToken ct = default)
    {
        var players = await _playerRepo.SearchAsync(tournamentId, query, ct);
        return players.Select(Map).ToList();
    }

    private static PlayerDto Map(TournamentPlayer tp) => new(
        tp.PlayerId,
        tp.Id,
        tp.Player.ExternalPlayerId,
        tp.Player.Name,
        tp.Player.ContactNumber,
        tp.DeckName,
        tp.PlayerNumber,
        tp.IsDropped);
}