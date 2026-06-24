using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Core.Entities;

public class Tournament : ISyncTrackable, IOrgScoped
{
    public int Id { get; set; }
    public string OrgCD { get; set; } = OrgDefaults.DefaultOrgCD;
    public string Name { get; set; } = string.Empty;
    public string GameTitle { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Organizer { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public int TotalSwissRounds { get; set; }
    public TopCutSize TopCutSize { get; set; }
    public FirstRoundPairingMode FirstRoundPairingMode { get; set; } = FirstRoundPairingMode.Random;
    public MatchFormat MatchFormat { get; set; } = MatchFormat.BO3;
    public bool HasElimination { get; set; }
    public int? EliminationLossCount { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Draft;
    public int CurrentRound { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SyncVersion { get; set; } = DateTime.UtcNow;
    public int Version { get; set; } = 1;
    public SyncOperation SyncOperation { get; set; } = SyncOperation.A;

    public ICollection<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();
    public ICollection<Round> Rounds { get; set; } = new List<Round>();
    public ICollection<Standing> Standings { get; set; } = new List<Standing>();
    public ICollection<TopCutBracket> TopCutBrackets { get; set; } = new List<TopCutBracket>();
}