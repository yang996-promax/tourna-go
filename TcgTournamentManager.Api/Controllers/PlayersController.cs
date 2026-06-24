using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tournaments/{tournamentId:int}/players")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly ICsvService _csvService;

    public PlayersController(IPlayerService playerService, ICsvService csvService)
    {
        _playerService = playerService;
        _csvService = csvService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlayerDto>>> GetAll(int tournamentId, CancellationToken ct) =>
        Ok(await _playerService.GetPlayersAsync(tournamentId, ct));

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<PlayerDto>>> Search(int tournamentId, [FromQuery] string q, CancellationToken ct) =>
        Ok(await _playerService.SearchPlayersAsync(tournamentId, q, ct));

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Add(int tournamentId, [FromBody] CreatePlayerRequest request, CancellationToken ct)
    {
        try
        {
            var player = await _playerService.AddPlayerAsync(tournamentId, request, ct);
            return CreatedAtAction(nameof(GetAll), new { tournamentId }, player);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{tournamentPlayerId:int}")]
    public async Task<ActionResult<PlayerDto>> Update(int tournamentId, int tournamentPlayerId, [FromBody] UpdatePlayerRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await _playerService.UpdatePlayerAsync(tournamentId, tournamentPlayerId, request, ct));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{tournamentPlayerId:int}")]
    public async Task<IActionResult> Remove(int tournamentId, int tournamentPlayerId, CancellationToken ct)
    {
        try
        {
            await _playerService.RemovePlayerAsync(tournamentId, tournamentPlayerId, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("import")]
    public async Task<ActionResult<object>> Import(int tournamentId, IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "CSV file is required." });

        await using var stream = file.OpenReadStream();
        var count = await _csvService.ImportPlayersAsync(tournamentId, stream, ct);
        return Ok(new { imported = count });
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export(int tournamentId, CancellationToken ct)
    {
        var bytes = await _csvService.ExportPlayersAsync(tournamentId, ct);
        return File(bytes, "text/csv", $"players_tournament_{tournamentId}.csv");
    }
}