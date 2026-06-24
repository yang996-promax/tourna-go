using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class AuditLog : ISyncTrackable, IOrgScoped
{
    public int Id { get; set; }
    public string OrgCD { get; set; } = OrgDefaults.DefaultOrgCD;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;
}