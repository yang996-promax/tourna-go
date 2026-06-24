using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.DTOs;

public record TopCutBracketDto(
    int Id,
    TopCutRound Round,
    int MatchPosition,
    int? PlayerAId,
    string? PlayerAName,
    int? PlayerBId,
    string? PlayerBName,
    int? WinnerId,
    string? WinnerName,
    int? NextBracketId,
    bool IsComplete,
    bool IsPlayable,
    int? MatchId);

public record TopCutQualifiedPlayerDto(
    int Seed,
    int TournamentPlayerId,
    string PlayerName,
    int PlayerNumber,
    int MatchPoints);

public record TopCutBracketTreeDto(
    IReadOnlyList<TopCutBracketDto> Brackets,
    IReadOnlyList<TopCutQualifiedPlayerDto> QualifiedPlayers,
    string? ChampionName,
    int? ChampionPlayerId,
    bool CanGenerate,
    string? StatusMessage);