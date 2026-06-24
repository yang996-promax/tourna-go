using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Services;

public class DatabaseService : IDatabaseService
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureDeployer _spDeployer;
    private readonly IConfiguration _configuration;

    public DatabaseService(
        TournamentDbContext db,
        StoredProcedureDeployer spDeployer,
        IConfiguration configuration)
    {
        _db = db;
        _spDeployer = spDeployer;
        _configuration = configuration;
    }

    public async Task MigrateAsync(CancellationToken ct = default)
    {
        await _db.Database.MigrateAsync(ct);
        await _spDeployer.DeployAsync(ct);
    }

    public async Task<string> BackupAsync(CancellationToken ct = default)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured.");

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;
        var backupDir = Path.Combine(AppContext.BaseDirectory, "Backups");
        Directory.CreateDirectory(backupDir);

        var backupPath = Path.Combine(backupDir, $"{databaseName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak");

        builder.InitialCatalog = "master";
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(ct);

        var sql = $"BACKUP DATABASE [{databaseName}] TO DISK = @path WITH FORMAT, INIT, NAME = 'TCG Tournament Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@path", backupPath);
        await command.ExecuteNonQueryAsync(ct);

        return backupPath;
    }

    public async Task RestoreAsync(string backupPath, CancellationToken ct = default)
    {
        if (!File.Exists(backupPath))
            throw new FileNotFoundException("Backup file not found.", backupPath);

        var connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured.");

        var builder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = builder.InitialCatalog;

        builder.InitialCatalog = "master";
        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync(ct);

        var killSql = $@"
            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{databaseName}] FROM DISK = @path WITH REPLACE;
            ALTER DATABASE [{databaseName}] SET MULTI_USER;";

        await using var command = new SqlCommand(killSql, connection);
        command.Parameters.AddWithValue("@path", backupPath);
        await command.ExecuteNonQueryAsync(ct);
    }
}