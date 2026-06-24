using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class MatchResult : ISyncTrackable, IOrgScoped
{
    public int Id { get; set; }
    public string OrgCD { get; set; } = OrgDefaults.DefaultOrgCD;
    public int MatchId { get; set; }
    public MatchResultType ResultType { get; set; }
    public int PlayerAGameWins { get; set; }
    public int PlayerBGameWins { get; set; }
    public int PlayerAMatchPoints { get; set; }
    public int PlayerBMatchPoints { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public Match Match { get; set; } = null!;
}