using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Interfaces;

public interface ITournamentRepository
{
    Task<Tournament?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Tournament?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Tournament>> GetAllAsync(CancellationToken ct = default);
    Task<Tournament?> GetActiveTournamentAsync(CancellationToken ct = default);
    Task<Tournament> CreateAsync(Tournament tournament, CancellationToken ct = default);
    Task UpdateAsync(Tournament tournament, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public interface IPlayerRepository
{
    Task<Player> CreatePlayerAsync(Player player, CancellationToken ct = default);
    Task UpdatePlayerAsync(Player player, CancellationToken ct = default);
    Task<Player?> GetByExternalIdAsync(string externalId, CancellationToken ct = default);
    Task<TournamentPlayer?> GetTournamentPlayerAsync(int tournamentId, int tournamentPlayerId, CancellationToken ct = default);
    Task<IReadOnlyList<TournamentPlayer>> GetTournamentPlayersAsync(int tournamentId, CancellationToken ct = default);
    Task<TournamentPlayer> AddToTournamentAsync(TournamentPlayer tp, CancellationToken ct = default);
    Task UpdateTournamentPlayerAsync(TournamentPlayer tp, CancellationToken ct = default);
    Task RemoveFromTournamentAsync(int tournamentPlayerId, CancellationToken ct = default);
    Task<TournamentPlayer?> GetDeletedTournamentPlayerAsync(int tournamentId, int playerId, CancellationToken ct = default);
    Task<IReadOnlyList<TournamentPlayer>> SearchAsync(int tournamentId, string query, CancellationToken ct = default);
}

public interface IMatchRepository
{
    Task<Round?> GetRoundAsync(int tournamentId, int roundNumber, CancellationToken ct = default, RoundType roundType = RoundType.Swiss);
    Task<Round?> GetRoundByIdAsync(int roundId, CancellationToken ct = default);
    Task<IReadOnlyList<Round>> GetRoundsAsync(int tournamentId, bool swissOnly = false, CancellationToken ct = default);
    Task<Match?> GetMatchByIdAsync(int matchId, CancellationToken ct = default);
    Task<Round> CreateRoundAsync(Round round, CancellationToken ct = default);
    Task CreateMatchesAsync(IEnumerable<Match> matches, CancellationToken ct = default);
    Task UpdateMatchAsync(Match match, CancellationToken ct = default);
    Task UpdateRoundAsync(Round round, CancellationToken ct = default);
    Task UpsertMatchResultAsync(MatchResult result, CancellationToken ct = default);
    Task<IReadOnlyList<Match>> GetMatchesForTournamentAsync(int tournamentId, CancellationToken ct = default);
    Task<bool> HavePlayedBeforeAsync(int tournamentId, int playerAId, int playerBId, CancellationToken ct = default);
}

public interface IStandingRepository
{
    Task<IReadOnlyList<Standing>> GetStandingsAsync(int tournamentId, CancellationToken ct = default);
    Task UpsertStandingsAsync(IEnumerable<Standing> standings, CancellationToken ct = default);
    Task DeleteForTournamentAsync(int tournamentId, CancellationToken ct = default);
}

public interface IByeHistoryRepository
{
    Task<bool> HasReceivedByeAsync(int tournamentId, int tournamentPlayerId, CancellationToken ct = default);
    Task RecordByeAsync(ByeHistory bye, CancellationToken ct = default);
    Task<IReadOnlyList<int>> GetPlayersWithByeAsync(int tournamentId, CancellationToken ct = default);
}

public interface ITopCutRepository
{
    Task<IReadOnlyList<TopCutBracket>> GetBracketsAsync(int tournamentId, CancellationToken ct = default);
    Task CreateBracketsAsync(IEnumerable<TopCutBracket> brackets, CancellationToken ct = default);
    Task<TopCutBracket?> GetBracketByIdAsync(int bracketId, CancellationToken ct = default);
    Task UpdateBracketAsync(TopCutBracket bracket, CancellationToken ct = default);
    Task DeleteForTournamentAsync(int tournamentId, CancellationToken ct = default);
}

public interface IAuditLogRepository
{
    Task LogAsync(string action, string entityType, int? entityId, string? details, string? performedBy, CancellationToken ct = default);
    Task<IReadOnlyList<AuditLog>> GetRecentAsync(int count = 50, CancellationToken ct = default);
}

public interface IOrganizerUserRepository
{
    Task<OrganizerUser?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<OrganizerUser> CreateAsync(OrganizerUser user, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}