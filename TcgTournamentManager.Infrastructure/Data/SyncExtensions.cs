using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Infrastructure.Data;

internal static class SyncExtensions
{
    public static void MarkDeleted(this ISyncTrackable entity)
    {
        entity.SyncOperation = SyncOperation.D;
    }

    public static void MarkRestored(this ISyncTrackable entity)
    {
        entity.SyncOperation = SyncOperation.U;
    }
}