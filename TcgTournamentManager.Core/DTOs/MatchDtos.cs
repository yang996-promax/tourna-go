using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.DTOs;

public record GenerateRoundRequest(
    FirstRoundPairingMode? FirstRoundPairingMode = null);

public record MatchDto(
    int Id,
    int RoundId,
    int RoundNumber,
    int TableNumber,
    int? PlayerAId,
    string? PlayerAName,
    int? PlayerBId,
    string? PlayerBName,
    bool IsBye,
    bool IsComplete,
    int? WinnerId,
    MatchResultDto? Result);

public record MatchResultDto(
    MatchResultType ResultType,
    int PlayerAGameWins,
    int PlayerBGameWins,
    int PlayerAMatchPoints,
    int PlayerBMatchPoints);

public record EnterMatchResultRequest(MatchResultType ResultType);

public record RoundDto(
    int Id,
    int RoundNumber,
    RoundType RoundType,
    bool IsComplete,
    IReadOnlyList<MatchDto> Matches);