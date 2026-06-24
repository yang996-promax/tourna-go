using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;
    private readonly ICurrentOrgContext _orgContext;

    public AuditLogRepository(TournamentDbContext db, StoredProcedureExecutor sp, ICurrentOrgContext orgContext)
    {
        _db = db;
        _sp = sp;
        _orgContext = orgContext;
    }

    public async Task LogAsync(string action, string entityType, int? entityId, string? details, string? performedBy, CancellationToken ct = default)
    {
        await _sp.ExecuteWithIntOutputAsync("usp_AuditLog_Insert", "@Id", ct,
            StoredProcedureExecutor.Str("@Action", action, 100),
            StoredProcedureExecutor.Str("@EntityType", entityType, 100),
            StoredProcedureExecutor.IntNull("@EntityId", entityId),
            StoredProcedureExecutor.StrNull("@Details", details, -1),
            StoredProcedureExecutor.StrNull("@PerformedBy", performedBy, -1),
            StoredProcedureExecutor.Str("@OrgCD", _orgContext.OrgCD, 50),
            StoredProcedureExecutor.OutInt("@Id"));
    }

    public async Task<IReadOnlyList<AuditLog>> GetRecentAsync(int count = 50, CancellationToken ct = default) =>
        await _db.AuditLogs
            .Where(a => a.OrgCD == _orgContext.OrgCD)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync(ct);
}