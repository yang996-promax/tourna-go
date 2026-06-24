namespace TcgTournamentManager.Core.Interfaces;

public interface ICurrentOrgContext
{
    string OrgCD { get; }
    int? UserId { get; }
    string? Username { get; }
}