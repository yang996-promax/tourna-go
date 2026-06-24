using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class Standing : ISyncTrackable
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int TournamentPlayerId { get; set; }
    public int Rank { get; set; }
    public int MatchPoints { get; set; }
    public int GameWins { get; set; }
    public int GameLosses { get; set; }
    public decimal OMWPercent { get; set; }
    public decimal GWPercent { get; set; }
    public decimal OGWPercent { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int MatchesLost { get; set; }
    public int MatchesDrawn { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public Tournament Tournament { get; set; } = null!;
    public TournamentPlayer TournamentPlayer { get; set; } = null!;
}