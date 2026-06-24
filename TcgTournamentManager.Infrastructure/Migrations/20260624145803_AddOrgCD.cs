using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgCD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MST_PLAYER_ExternalPlayerId",
                table: "MST_PLAYER");

            migrationBuilder.DropIndex(
                name: "IX_MST_ORGANIZER_USER_Username",
                table: "MST_ORGANIZER_USER");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "TOURNAMENT_STANDING",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "TOURNAMENT_PLAYER",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "TOP_CUT_BRACKET",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AlterColumn<int>(
                name: "MatchFormat",
                table: "MST_TOURNAMENT",
                type: "int",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "MST_TOURNAMENT",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "MST_ROUND",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "MST_PLAYER",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "MST_ORGANIZER_USER",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "MST_MATCH",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "MATCH_RESULT",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "BYE_HISTORY",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.AddColumn<string>(
                name: "OrgCD",
                table: "AUDIT_LOG",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "DEFAULT");

            migrationBuilder.CreateIndex(
                name: "IX_TOURNAMENT_STANDING_OrgCD",
                table: "TOURNAMENT_STANDING",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_TOURNAMENT_PLAYER_OrgCD",
                table: "TOURNAMENT_PLAYER",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_TOP_CUT_BRACKET_OrgCD",
                table: "TOP_CUT_BRACKET",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_MST_TOURNAMENT_OrgCD",
                table: "MST_TOURNAMENT",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_MST_ROUND_OrgCD",
                table: "MST_ROUND",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_MST_PLAYER_OrgCD",
                table: "MST_PLAYER",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_MST_PLAYER_OrgCD_ExternalPlayerId",
                table: "MST_PLAYER",
                columns: new[] { "OrgCD", "ExternalPlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MST_ORGANIZER_USER_OrgCD",
                table: "MST_ORGANIZER_USER",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_MST_ORGANIZER_USER_OrgCD_Username",
                table: "MST_ORGANIZER_USER",
                columns: new[] { "OrgCD", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MST_MATCH_OrgCD",
                table: "MST_MATCH",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_MATCH_RESULT_OrgCD",
                table: "MATCH_RESULT",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_BYE_HISTORY_OrgCD",
                table: "BYE_HISTORY",
                column: "OrgCD");

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_LOG_OrgCD",
                table: "AUDIT_LOG",
                column: "OrgCD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TOURNAMENT_STANDING_OrgCD",
                table: "TOURNAMENT_STANDING");

            migrationBuilder.DropIndex(
                name: "IX_TOURNAMENT_PLAYER_OrgCD",
                table: "TOURNAMENT_PLAYER");

            migrationBuilder.DropIndex(
                name: "IX_TOP_CUT_BRACKET_OrgCD",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropIndex(
                name: "IX_MST_TOURNAMENT_OrgCD",
                table: "MST_TOURNAMENT");

            migrationBuilder.DropIndex(
                name: "IX_MST_ROUND_OrgCD",
                table: "MST_ROUND");

            migrationBuilder.DropIndex(
                name: "IX_MST_PLAYER_OrgCD",
                table: "MST_PLAYER");

            migrationBuilder.DropIndex(
                name: "IX_MST_PLAYER_OrgCD_ExternalPlayerId",
                table: "MST_PLAYER");

            migrationBuilder.DropIndex(
                name: "IX_MST_ORGANIZER_USER_OrgCD",
                table: "MST_ORGANIZER_USER");

            migrationBuilder.DropIndex(
                name: "IX_MST_ORGANIZER_USER_OrgCD_Username",
                table: "MST_ORGANIZER_USER");

            migrationBuilder.DropIndex(
                name: "IX_MST_MATCH_OrgCD",
                table: "MST_MATCH");

            migrationBuilder.DropIndex(
                name: "IX_MATCH_RESULT_OrgCD",
                table: "MATCH_RESULT");

            migrationBuilder.DropIndex(
                name: "IX_BYE_HISTORY_OrgCD",
                table: "BYE_HISTORY");

            migrationBuilder.DropIndex(
                name: "IX_AUDIT_LOG_OrgCD",
                table: "AUDIT_LOG");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "TOURNAMENT_STANDING");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "TOURNAMENT_PLAYER");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "MST_TOURNAMENT");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "MST_ROUND");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "MST_PLAYER");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "MST_ORGANIZER_USER");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "MST_MATCH");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "MATCH_RESULT");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "BYE_HISTORY");

            migrationBuilder.DropColumn(
                name: "OrgCD",
                table: "AUDIT_LOG");

            migrationBuilder.AlterColumn<int>(
                name: "MatchFormat",
                table: "MST_TOURNAMENT",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 3);

            migrationBuilder.CreateIndex(
                name: "IX_MST_PLAYER_ExternalPlayerId",
                table: "MST_PLAYER",
                column: "ExternalPlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MST_ORGANIZER_USER_Username",
                table: "MST_ORGANIZER_USER",
                column: "Username",
                unique: true);
        }
    }
}
