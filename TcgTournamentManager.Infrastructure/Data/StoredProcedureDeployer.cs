using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TcgTournamentManager.Infrastructure.Data;

public class StoredProcedureDeployer
{
    private readonly TournamentDbContext _db;

    public StoredProcedureDeployer(TournamentDbContext db) => _db = db;

    public async Task DeployAsync(CancellationToken ct = default)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Sql", "StoredProcedures.sql");
        if (!File.Exists(path))
            throw new FileNotFoundException("Stored procedure script not found.", path);

        var script = await File.ReadAllTextAsync(path, ct);
        var batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Select(b => b.Trim())
            .Where(b => !string.IsNullOrWhiteSpace(b));

        var connection = (SqlConnection)_db.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync(ct);

        foreach (var batch in batches)
        {
            await using var command = new SqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync(ct);
        }
    }
}