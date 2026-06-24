using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.DTOs;

public record CreateTournamentRequest(
    string Name,
    string GameTitle,
    DateTime EventDate,
    string Organizer,
    string Venue,
    int TotalSwissRounds,
    TopCutSize TopCutSize,
    FirstRoundPairingMode FirstRoundPairingMode = FirstRoundPairingMode.Random,
    MatchFormat MatchFormat = MatchFormat.BO3,
    bool HasElimination = false,
    int? EliminationLossCount = null);

public record UpdateTournamentRequest(
    string Name,
    string GameTitle,
    DateTime EventDate,
    string Organizer,
    string Venue,
    int TotalSwissRounds,
    TopCutSize TopCutSize,
    FirstRoundPairingMode FirstRoundPairingMode,
    MatchFormat MatchFormat = MatchFormat.BO3,
    bool HasElimination = false,
    int? EliminationLossCount = null);

public record TournamentDto(
    int Id,
    string Name,
    string GameTitle,
    DateTime EventDate,
    string Organizer,
    string Venue,
    int TotalSwissRounds,
    TopCutSize TopCutSize,
    FirstRoundPairingMode FirstRoundPairingMode,
    MatchFormat MatchFormat,
    bool HasElimination,
    int? EliminationLossCount,
    TournamentStatus Status,
    int CurrentRound,
    int PlayerCount,
    DateTime CreatedAt);

public record DashboardDto(
    TournamentDto? ActiveTournament,
    int TotalPlayers,
    int CurrentRound,
    int CompletedMatches,
    int TotalMatches,
    string Status);