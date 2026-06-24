using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tournaments")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly ISwissPairingService _pairingService;
    private readonly IStandingsService _standingsService;
    private readonly ITopCutService _topCutService;
    private readonly IMatchRepository _matchRepo;

    public TournamentsController(
        ITournamentService tournamentService,
        ISwissPairingService pairingService,
        IStandingsService standingsService,
        ITopCutService topCutService,
        IMatchRepository matchRepo)
    {
        _tournamentService = tournamentService;
        _pairingService = pairingService;
        _standingsService = standingsService;
        _topCutService = topCutService;
        _matchRepo = matchRepo;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TournamentDto>>> GetAll(CancellationToken ct) =>
        Ok(await _tournamentService.GetAllAsync(ct));

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(CancellationToken ct) =>
        Ok(await _tournamentService.GetDashboardAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TournamentDto>> GetById(int id, CancellationToken ct)
    {
        var tournament = await _tournamentService.GetByIdAsync(id, ct);
        return tournament == null ? NotFound() : Ok(tournament);
    }

    [HttpPost]
    public async Task<ActionResult<TournamentDto>> Create([FromBody] CreateTournamentRequest request, CancellationToken ct)
    {
        var tournament = await _tournamentService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = tournament.Id }, tournament);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TournamentDto>> Update(int id, [FromBody] UpdateTournamentRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _tournamentService.UpdateAsync(id, request, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _tournamentService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/start")]
    public async Task<IActionResult> Start(int id, CancellationToken ct)
    {
        try
        {
            await _tournamentService.StartTournamentAsync(id, ct);
            return Ok(new { message = "Tournament started." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/generate-round")]
    public async Task<ActionResult<RoundDto>> GenerateRound(int id, [FromBody] GenerateRoundRequest? request, CancellationToken ct)
    {
        try
        {
            return Ok(await _pairingService.GenerateRoundAsync(id, request?.FirstRoundPairingMode, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/rounds")]
    public async Task<ActionResult<IReadOnlyList<RoundDto>>> GetRounds(
        int id, [FromQuery] bool swissOnly = false, CancellationToken ct = default)
    {
        var rounds = await _matchRepo.GetRoundsAsync(id, swissOnly, ct);
        return Ok(rounds.Select(r => new RoundDto(
            r.Id, r.RoundNumber, r.RoundType, r.IsComplete,
            r.Matches.OrderBy(m => m.TableNumber).Select(m => new MatchDto(
                m.Id, r.Id, r.RoundNumber, m.TableNumber,
                m.PlayerAId, m.PlayerA?.Player.Name,
                m.PlayerBId, m.PlayerB?.Player.Name,
                m.IsBye, m.IsComplete, m.WinnerId,
                m.Result == null ? null : new MatchResultDto(
                    m.Result.ResultType, m.Result.PlayerAGameWins, m.Result.PlayerBGameWins,
                    m.Result.PlayerAMatchPoints, m.Result.PlayerBMatchPoints)
            )).ToList())).ToList());
    }

    [HttpGet("{id:int}/standings")]
    public async Task<ActionResult<IReadOnlyList<StandingDto>>> GetStandings(
        int id,
        [FromQuery] StandingSortBy sortBy = StandingSortBy.MatchPoints,
        [FromQuery] StandingPhase phase = StandingPhase.Overall,
        CancellationToken ct = default) =>
        Ok(await _standingsService.CalculateStandingsAsync(id, sortBy, phase, ct));

    [HttpPost("{id:int}/complete-swiss")]
    public async Task<IActionResult> CompleteSwiss(int id, CancellationToken ct)
    {
        try
        {
            await _topCutService.CompleteSwissAsync(id, ct);
            return Ok(new { message = "Swiss rounds complete. Ready for top cut." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/generate-topcut")]
    public async Task<ActionResult<TopCutBracketTreeDto>> GenerateTopCut(int id, CancellationToken ct)
    {
        try
        {
            return Ok(await _topCutService.GenerateTopCutAsync(id, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}/topcut")]
    public async Task<ActionResult<TopCutBracketTreeDto>> GetTopCut(int id, CancellationToken ct) =>
        Ok(await _topCutService.GetBracketTreeAsync(id, ct));
}