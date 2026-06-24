namespace TcgTournamentManager.Core.DTOs;

public record CreatePlayerRequest(
    string ExternalPlayerId,
    string Name,
    string? ContactNumber,
    string? DeckName,
    int? PlayerNumber);

public record UpdatePlayerRequest(
    string ExternalPlayerId,
    string Name,
    string? ContactNumber,
    string? DeckName,
    int PlayerNumber);

public record PlayerDto(
    int Id,
    int TournamentPlayerId,
    string ExternalPlayerId,
    string Name,
    string? ContactNumber,
    string? DeckName,
    int PlayerNumber,
    bool IsDropped);

public record PlayerCsvRow(
    string ExternalPlayerId,
    string Name,
    string? ContactNumber,
    string? DeckName,
    int PlayerNumber);