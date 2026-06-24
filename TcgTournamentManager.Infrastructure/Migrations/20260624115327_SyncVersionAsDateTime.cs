using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations;

/// <inheritdoc />
public partial class SyncVersionAsDateTime : Migration
{
    private static readonly string[] Tables =
    [
        "AuditLogs",
        "ByeHistories",
        "Matches",
        "MatchResults",
        "OrganizerUsers",
        "Players",
        "Rounds",
        "Standings",
        "TopCutBrackets",
        "TournamentPlayers",
        "Tournaments"
    ];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        foreach (var table in Tables)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SyncVersionNew",
                table: table,
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.Sql($"UPDATE [{table}] SET [SyncVersionNew] = [CreatedAt];");

            migrationBuilder.DropColumn(name: "SyncVersion", table: table);

            migrationBuilder.RenameColumn(
                name: "SyncVersionNew",
                table: table,
                newName: "SyncVersion");
        }
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        foreach (var table in Tables)
        {
            migrationBuilder.AddColumn<int>(
                name: "SyncVersionOld",
                table: table,
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.DropColumn(name: "SyncVersion", table: table);

            migrationBuilder.RenameColumn(
                name: "SyncVersionOld",
                table: table,
                newName: "SyncVersion");
        }
    }
}