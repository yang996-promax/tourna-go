using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefineRoundStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MST_ROUND_TournamentId_RoundNumber",
                table: "MST_ROUND");

            migrationBuilder.Sql("""
                UPDATE r SET r.RoundNumber = CASE
                    WHEN r.RoundNumber = 116 THEN 1
                    WHEN r.RoundNumber = 108 THEN CASE WHEN t.TopCutSize = 16 THEN 2 ELSE 1 END
                    WHEN r.RoundNumber = 104 THEN CASE WHEN t.TopCutSize = 16 THEN 3 WHEN t.TopCutSize = 8 THEN 2 ELSE 1 END
                    WHEN r.RoundNumber = 102 THEN CASE WHEN t.TopCutSize = 16 THEN 4 WHEN t.TopCutSize = 8 THEN 3 WHEN t.TopCutSize = 4 THEN 2 ELSE 1 END
                    ELSE r.RoundNumber
                END
                FROM MST_ROUND r
                INNER JOIN MST_TOURNAMENT t ON t.Id = r.TournamentId
                WHERE r.RoundType = 1 AND r.RoundNumber >= 100;
                """);

            migrationBuilder.DropColumn(
                name: "IsTopCut",
                table: "MST_ROUND");

            migrationBuilder.CreateIndex(
                name: "IX_MST_ROUND_TournamentId_RoundNumber_RoundType",
                table: "MST_ROUND",
                columns: new[] { "TournamentId", "RoundNumber", "RoundType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MST_ROUND_TournamentId_RoundNumber_RoundType",
                table: "MST_ROUND");

            migrationBuilder.AddColumn<bool>(
                name: "IsTopCut",
                table: "MST_ROUND",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MST_ROUND_TournamentId_RoundNumber",
                table: "MST_ROUND",
                columns: new[] { "TournamentId", "RoundNumber" },
                unique: true);
        }
    }
}
