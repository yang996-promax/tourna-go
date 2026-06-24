using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class Player : ISyncTrackable, IOrgScoped
{
    public int Id { get; set; }
    public string OrgCD { get; set; } = OrgDefaults.DefaultOrgCD;
    public string ExternalPlayerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public ICollection<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();
}