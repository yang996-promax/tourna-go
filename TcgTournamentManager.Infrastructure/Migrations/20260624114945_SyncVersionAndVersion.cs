using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncVersionAndVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "TopCutBrackets");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Standings");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "OrganizerUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "MatchResults");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "ByeHistories");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "AuditLogs");

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "TournamentPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TournamentPlayers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "TopCutBrackets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "TopCutBrackets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Standings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Standings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Rounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "OrganizerUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "OrganizerUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "MatchResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "MatchResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "ByeHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "ByeHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SyncVersion",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "TournamentPlayers");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "TopCutBrackets");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "TopCutBrackets");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Standings");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Standings");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Rounds");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "OrganizerUsers");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OrganizerUsers");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "MatchResults");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "MatchResults");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "ByeHistories");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "ByeHistories");

            migrationBuilder.DropColumn(
                name: "SyncVersion",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "AuditLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Tournaments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "TournamentPlayers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "TopCutBrackets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Standings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Rounds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Players",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "OrganizerUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "MatchResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "Matches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "ByeHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "AuditLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
