namespace TcgTournamentManager.Core.DTOs;

public record StandingDto(
    int Rank,
    int TournamentPlayerId,
    string PlayerName,
    int PlayerNumber,
    int MatchPoints,
    int GameWins,
    int GameLosses,
    decimal OMWPercent,
    decimal GWPercent,
    decimal OGWPercent,
    int MatchesPlayed,
    int MatchesWon,
    int MatchesLost,
    int MatchesDrawn);

public enum StandingSortBy
{
    MatchPoints,
    OMWPercent,
    GWPercent,
    OGWPercent
}

public enum StandingPhase
{
    Swiss,
    Overall
}