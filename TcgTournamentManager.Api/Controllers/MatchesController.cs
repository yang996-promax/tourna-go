using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/matches")]
public class MatchesController : ControllerBase
{
    private readonly IMatchResultService _matchResultService;
    private readonly ITopCutService _topCutService;

    public MatchesController(IMatchResultService matchResultService, ITopCutService topCutService)
    {
        _matchResultService = matchResultService;
        _topCutService = topCutService;
    }

    [HttpPost("{id:int}/result")]
    public async Task<ActionResult<MatchDto>> EnterResult(int id, [FromBody] EnterMatchResultRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _matchResultService.EnterResultAsync(id, request, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("topcut/{bracketId:int}/result")]
    public async Task<ActionResult<TopCutBracketDto>> EnterTopCutResult(int bracketId, [FromBody] TopCutResultRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _topCutService.EnterTopCutResultAsync(bracketId, request.WinnerId, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record TopCutResultRequest(int WinnerId);