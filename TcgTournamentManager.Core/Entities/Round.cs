using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class Round : ISyncTrackable
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int RoundNumber { get; set; }
    public RoundType RoundType { get; set; } = RoundType.Swiss;
    public bool IsComplete { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public Tournament Tournament { get; set; } = null!;
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}