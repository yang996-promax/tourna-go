using TcgTournamentManager.Infrastructure.Data;
using TcgTournamentManager.Infrastructure.Repositories;

namespace TcgTournamentManager.Tests;

internal static class TestRepositories
{
    public static StoredProcedureExecutor Sp(TournamentDbContext db) => new EfStoredProcedureExecutor(db);

    public static TournamentRepository Tournament(TournamentDbContext db) => new(db, Sp(db));
    public static PlayerRepository Player(TournamentDbContext db) => new(db, Sp(db));
    public static MatchRepository Match(TournamentDbContext db) => new(db, Sp(db));
    public static StandingRepository Standing(TournamentDbContext db) => new(db, Sp(db));
    public static TopCutRepository TopCut(TournamentDbContext db) => new(db, Sp(db));
    public static AuditLogRepository AuditLog(TournamentDbContext db) => new(db, Sp(db));
}