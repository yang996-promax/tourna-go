namespace TcgTournamentManager.Core.Enums;

/// <summary>A = Add, U = Update, D = Delete (soft delete for sync).</summary>
public enum SyncOperation
{
    A,
    U,
    D
}