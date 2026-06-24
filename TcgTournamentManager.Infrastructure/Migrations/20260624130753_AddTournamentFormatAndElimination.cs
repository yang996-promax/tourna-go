using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentFormatAndElimination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EliminationLossCount",
                table: "MST_TOURNAMENT",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasElimination",
                table: "MST_TOURNAMENT",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MatchFormat",
                table: "MST_TOURNAMENT",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EliminationLossCount",
                table: "MST_TOURNAMENT");

            migrationBuilder.DropColumn(
                name: "HasElimination",
                table: "MST_TOURNAMENT");

            migrationBuilder.DropColumn(
                name: "MatchFormat",
                table: "MST_TOURNAMENT");
        }
    }
}
