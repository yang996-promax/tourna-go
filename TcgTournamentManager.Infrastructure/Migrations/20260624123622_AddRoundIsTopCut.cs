using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoundIsTopCut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTopCut",
                table: "MST_ROUND",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE MST_ROUND SET IsTopCut = 1 WHERE RoundType = 1 OR RoundNumber >= 100");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTopCut",
                table: "MST_ROUND");
        }
    }
}
