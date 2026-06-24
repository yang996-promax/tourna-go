using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/database")]
public class DatabaseController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public DatabaseController(IDatabaseService databaseService) => _databaseService = databaseService;

    [HttpPost("backup")]
    public async Task<ActionResult<object>> Backup(CancellationToken ct)
    {
        var path = await _databaseService.BackupAsync(ct);
        return Ok(new { path, message = "Backup completed successfully." });
    }

    [HttpPost("restore")]
    public async Task<ActionResult<object>> Restore([FromBody] RestoreRequest request, CancellationToken ct)
    {
        try
        {
            await _databaseService.RestoreAsync(request.BackupPath, ct);
            return Ok(new { message = "Restore completed successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public record RestoreRequest(string BackupPath);