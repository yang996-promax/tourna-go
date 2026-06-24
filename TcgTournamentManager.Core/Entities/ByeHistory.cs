using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class ByeHistory : ISyncTrackable
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int TournamentPlayerId { get; set; }
    public int RoundNumber { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public Tournament Tournament { get; set; } = null!;
    public TournamentPlayer TournamentPlayer { get; set; } = null!;
}