namespace TcgTournamentManager.Core.Enums;

public enum TournamentStatus
{
    Draft = 0,
    Registration = 1,
    InProgress = 2,
    SwissComplete = 3,
    TopCutInProgress = 4,
    Completed = 5
}

public enum TopCutSize
{
    None = 0,
    Top4 = 4,
    Top8 = 8,
    Top16 = 16
}

public enum RoundType
{
    Swiss = 0,
    TopCut = 1
}

public enum MatchResultType
{
    Win2_0 = 0,
    Win2_1 = 1,
    Loss1_2 = 2,
    Loss0_2 = 3,
    Draw = 4,
    ByeWin = 5
}

public enum FirstRoundPairingMode
{
    Random = 0,
    Seeded = 1
}

public enum MatchFormat
{
    BO1 = 1,
    BO3 = 3
}

public enum TopCutRound
{
    RoundOf16 = 16,
    QuarterFinal = 8,
    SemiFinal = 4,
    Final = 2
}