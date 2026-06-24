using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Interfaces;

public interface ISwissPairingService
{
    Task<RoundDto> GenerateRoundAsync(int tournamentId, FirstRoundPairingMode? overrideMode = null, CancellationToken ct = default);
    IReadOnlyList<PairingProposal> ComputePairings(SwissPairingContext context);
}

public interface IStandingsService
{
    Task<IReadOnlyList<StandingDto>> CalculateStandingsAsync(
        int tournamentId,
        StandingSortBy sortBy = StandingSortBy.MatchPoints,
        StandingPhase phase = StandingPhase.Overall,
        CancellationToken ct = default);
    Task RecalculateAndPersistAsync(int tournamentId, CancellationToken ct = default);
}

public interface IMatchResultService
{
    Task<MatchDto> EnterResultAsync(int matchId, EnterMatchResultRequest request, CancellationToken ct = default);
    (int playerAGW, int playerBGW, int playerAMP, int playerBMP) CalculatePoints(MatchResultType resultType, bool isBye, MatchFormat matchFormat);
}

public interface ITopCutService
{
    Task CompleteSwissAsync(int tournamentId, CancellationToken ct = default);
    Task<TopCutBracketTreeDto> GenerateTopCutAsync(int tournamentId, CancellationToken ct = default);
    Task<TopCutBracketTreeDto> GetBracketTreeAsync(int tournamentId, CancellationToken ct = default);
    Task<TopCutBracketDto> EnterTopCutResultAsync(int bracketId, int winnerId, CancellationToken ct = default);
}

public interface ICsvService
{
    Task<int> ImportPlayersAsync(int tournamentId, Stream csvStream, CancellationToken ct = default);
    Task<byte[]> ExportPlayersAsync(int tournamentId, CancellationToken ct = default);
}

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<LoginResponse> RegisterAsync(RegisterOrganizerRequest request, CancellationToken ct = default);
    Task EnsureDefaultUserAsync(CancellationToken ct = default);
}

public interface IDatabaseService
{
    Task MigrateAsync(CancellationToken ct = default);
    Task<string> BackupAsync(CancellationToken ct = default);
    Task RestoreAsync(string backupPath, CancellationToken ct = default);
}

public interface ITournamentService
{
    Task<TournamentDto> CreateAsync(CreateTournamentRequest request, CancellationToken ct = default);
    Task<TournamentDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TournamentDto>> GetAllAsync(CancellationToken ct = default);
    Task<TournamentDto> UpdateAsync(int id, UpdateTournamentRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<DashboardDto> GetDashboardAsync(CancellationToken ct = default);
    Task StartTournamentAsync(int id, CancellationToken ct = default);
}

public interface IPlayerService
{
    Task<PlayerDto> AddPlayerAsync(int tournamentId, CreatePlayerRequest request, CancellationToken ct = default);
    Task<PlayerDto> UpdatePlayerAsync(int tournamentId, int tournamentPlayerId, UpdatePlayerRequest request, CancellationToken ct = default);
    Task RemovePlayerAsync(int tournamentId, int tournamentPlayerId, CancellationToken ct = default);
    Task<IReadOnlyList<PlayerDto>> GetPlayersAsync(int tournamentId, CancellationToken ct = default);
    Task<IReadOnlyList<PlayerDto>> SearchPlayersAsync(int tournamentId, string query, CancellationToken ct = default);
}

public class SwissPairingContext
{
    public int TournamentId { get; init; }
    public int RoundNumber { get; init; }
    public bool IsLastRound { get; init; }
    public FirstRoundPairingMode FirstRoundMode { get; init; }
    public IReadOnlyList<SwissPlayerState> Players { get; init; } = Array.Empty<SwissPlayerState>();
    public IReadOnlySet<(int, int)> PreviousPairings { get; init; } = new HashSet<(int, int)>();
    public IReadOnlySet<int> PlayersWithBye { get; init; } = new HashSet<int>();
}

public class SwissPlayerState
{
    public int TournamentPlayerId { get; init; }
    public int PlayerNumber { get; init; }
    public int MatchPoints { get; init; }
    public bool IsDropped { get; init; }
}

public class PairingProposal
{
    public int TableNumber { get; set; }
    public int? PlayerAId { get; set; }
    public int? PlayerBId { get; set; }
    public bool IsBye { get; set; }
}