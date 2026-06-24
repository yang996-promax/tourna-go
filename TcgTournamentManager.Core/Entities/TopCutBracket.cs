using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class TopCutBracket : ISyncTrackable, IOrgScoped
{
    public int Id { get; set; }
    public string OrgCD { get; set; } = OrgDefaults.DefaultOrgCD;
    public int TournamentId { get; set; }
    public TopCutRound Round { get; set; }
    public int MatchPosition { get; set; }
    public int? PlayerAId { get; set; }
    public int? PlayerBId { get; set; }
    public int? WinnerId { get; set; }
    public int? NextBracketId { get; set; }
    public bool IsComplete { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public Tournament Tournament { get; set; } = null!;
    public TournamentPlayer? PlayerA { get; set; }
    public TournamentPlayer? PlayerB { get; set; }
    public TournamentPlayer? Winner { get; set; }
    public TopCutBracket? NextBracket { get; set; }
    public Match? Match { get; set; }
}