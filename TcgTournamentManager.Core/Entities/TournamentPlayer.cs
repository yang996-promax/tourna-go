using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class TournamentPlayer : ISyncTrackable, IOrgScoped
{
    public int Id { get; set; }
    public string OrgCD { get; set; } = OrgDefaults.DefaultOrgCD;
    public int TournamentId { get; set; }
    public int PlayerId { get; set; }
    public int PlayerNumber { get; set; }
    public string? DeckName { get; set; }
    public bool IsDropped { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public Tournament Tournament { get; set; } = null!;
    public Player Player { get; set; } = null!;
    public ICollection<ByeHistory> ByeHistories { get; set; } = new List<ByeHistory>();
}