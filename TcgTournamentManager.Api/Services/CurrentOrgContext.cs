using System.Security.Claims;
using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Api.Services;

public class CurrentOrgContext : ICurrentOrgContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentOrgContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string OrgCD =>
        User?.FindFirst("org_cd")?.Value
        ?? OrgDefaults.DefaultOrgCD;

    public int? UserId
    {
        get
        {
            var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(id, out var userId) ? userId : null;
        }
    }

    public string? Username => User?.FindFirst(ClaimTypes.Name)?.Value;
}