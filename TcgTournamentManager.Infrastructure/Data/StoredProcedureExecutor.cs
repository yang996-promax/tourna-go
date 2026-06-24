using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TcgTournamentManager.Infrastructure.Data;

public class StoredProcedureExecutor
{
    protected readonly TournamentDbContext Db;

    public StoredProcedureExecutor(TournamentDbContext db) => Db = db;

    public virtual async Task ExecuteAsync(string procedureName, CancellationToken ct = default, params SqlParameter[] parameters)
    {
        var connection = await OpenConnectionAsync(ct);
        await using var command = CreateCommand(connection, procedureName, parameters);
        await command.ExecuteNonQueryAsync(ct);
    }

    public virtual async Task<int> ExecuteWithIntOutputAsync(
        string procedureName, string outputParameterName, CancellationToken ct = default, params SqlParameter[] parameters)
    {
        var connection = await OpenConnectionAsync(ct);
        await using var command = CreateCommand(connection, procedureName, parameters);
        await command.ExecuteNonQueryAsync(ct);
        return Convert.ToInt32(command.Parameters[outputParameterName].Value);
    }

    public static SqlParameter Int(string name, int value) => new(name, value);
    public static SqlParameter IntNull(string name, int? value) => new(name, (object?)value ?? DBNull.Value);
    public static SqlParameter Bit(string name, bool value) => new(name, value);
    public static SqlParameter Str(string name, string value, int size) => new(name, SqlDbType.NVarChar, size) { Value = value };
    public static SqlParameter StrNull(string name, string? value, int size) => new(name, SqlDbType.NVarChar, size) { Value = (object?)value ?? DBNull.Value };
    public static SqlParameter DateTimeParam(string name, DateTime value) => new(name, value);
    public static SqlParameter DateTimeNull(string name, DateTime? value) => new(name, (object?)value ?? DBNull.Value);
    public static SqlParameter DecimalParam(string name, decimal value) => new(name, value);
    public static SqlParameter OutInt(string name) => new(name, SqlDbType.Int) { Direction = ParameterDirection.Output };

    private async Task<SqlConnection> OpenConnectionAsync(CancellationToken ct)
    {
        var connection = (SqlConnection)Db.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(ct);
        return connection;
    }

    private static SqlCommand CreateCommand(SqlConnection connection, string procedureName, SqlParameter[] parameters)
    {
        var command = new SqlCommand(procedureName, connection) { CommandType = CommandType.StoredProcedure };
        if (parameters.Length > 0)
            command.Parameters.AddRange(parameters);
        return command;
    }
}