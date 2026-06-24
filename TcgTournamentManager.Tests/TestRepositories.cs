using TcgTournamentManager.Core;
using TcgTournamentManager.Infrastructure.Data;
using TcgTournamentManager.Infrastructure.Repositories;

namespace TcgTournamentManager.Tests;

internal static class TestRepositories
{
    private static readonly TestCurrentOrgContext OrgContext = new();

    public static StoredProcedureExecutor Sp(TournamentDbContext db) => new EfStoredProcedureExecutor(db);

    public static TournamentRepository Tournament(TournamentDbContext db) => new(db, Sp(db), OrgContext);
    public static PlayerRepository Player(TournamentDbContext db) => new(db, Sp(db), OrgContext);
    public static MatchRepository Match(TournamentDbContext db) => new(db, Sp(db), OrgContext);
    public static StandingRepository Standing(TournamentDbContext db) => new(db, Sp(db), OrgContext);
    public static TopCutRepository TopCut(TournamentDbContext db) => new(db, Sp(db), OrgContext);
    public static AuditLogRepository AuditLog(TournamentDbContext db) => new(db, Sp(db), OrgContext);
}