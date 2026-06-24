using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class Match : ISyncTrackable
{
    public int Id { get; set; }
    public int RoundId { get; set; }
    public int TableNumber { get; set; }
    public int? PlayerAId { get; set; }
    public int? PlayerBId { get; set; }
    public bool IsBye { get; set; }
    public bool IsComplete { get; set; }
    public int? WinnerId { get; set; }
    public int? TopCutBracketId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public Round Round { get; set; } = null!;
    public TournamentPlayer? PlayerA { get; set; }
    public TournamentPlayer? PlayerB { get; set; }
    public TournamentPlayer? Winner { get; set; }
    public TopCutBracket? TopCutBracket { get; set; }
    public MatchResult? Result { get; set; }
}