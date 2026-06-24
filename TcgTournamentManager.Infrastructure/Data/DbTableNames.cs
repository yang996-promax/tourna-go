namespace TcgTournamentManager.Infrastructure.Data;

/// <summary>
/// SQL Server table names. Master/key tables use MST_ prefix; child/detail tables use descriptive names.
/// </summary>
public static class DbTableNames
{
    public const string Tournament = "MST_TOURNAMENT";
    public const string Player = "MST_PLAYER";
    public const string OrganizerUser = "MST_ORGANIZER_USER";
    public const string Round = "MST_ROUND";
    public const string Match = "MST_MATCH";

    public const string TournamentPlayer = "TOURNAMENT_PLAYER";
    public const string MatchResult = "MATCH_RESULT";
    public const string Standing = "TOURNAMENT_STANDING";
    public const string ByeHistory = "BYE_HISTORY";
    public const string TopCutBracket = "TOP_CUT_BRACKET";
    public const string AuditLog = "AUDIT_LOG";
}