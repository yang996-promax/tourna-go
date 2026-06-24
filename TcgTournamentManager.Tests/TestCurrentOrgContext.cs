using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Tests;

internal sealed class TestCurrentOrgContext : ICurrentOrgContext
{
    public TestCurrentOrgContext(string orgCd = OrgDefaults.DefaultOrgCD)
    {
        OrgCD = orgCd;
    }

    public string OrgCD { get; set; }
    public int? UserId => null;
    public string? Username => null;
}