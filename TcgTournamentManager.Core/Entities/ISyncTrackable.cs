using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public interface ISyncTrackable
{
    DateTime CreatedAt { get; set; }
    DateTime SyncVersion { get; set; }
    int Version { get; set; }
    SyncOperation SyncOperation { get; set; }
}