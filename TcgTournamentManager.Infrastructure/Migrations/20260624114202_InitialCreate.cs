using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizerUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizerUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalPlayerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GameTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Organizer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TotalSwissRounds = table.Column<int>(type: "int", nullable: false),
                    TopCutSize = table.Column<int>(type: "int", nullable: false),
                    FirstRoundPairingMode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentRound = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    RoundType = table.Column<int>(type: "int", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rounds_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TournamentPlayers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    PlayerNumber = table.Column<int>(type: "int", nullable: false),
                    DeckName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDropped = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TournamentPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TournamentPlayers_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ByeHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    TournamentPlayerId = table.Column<int>(type: "int", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ByeHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ByeHistories_TournamentPlayers_TournamentPlayerId",
                        column: x => x.TournamentPlayerId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ByeHistories_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Standings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    TournamentPlayerId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    MatchPoints = table.Column<int>(type: "int", nullable: false),
                    GameWins = table.Column<int>(type: "int", nullable: false),
                    GameLosses = table.Column<int>(type: "int", nullable: false),
                    OMWPercent = table.Column<decimal>(type: "decimal(8,4)", precision: 8, scale: 4, nullable: false),
                    GWPercent = table.Column<decimal>(type: "decimal(8,4)", precision: 8, scale: 4, nullable: false),
                    OGWPercent = table.Column<decimal>(type: "decimal(8,4)", precision: 8, scale: 4, nullable: false),
                    MatchesPlayed = table.Column<int>(type: "int", nullable: false),
                    MatchesWon = table.Column<int>(type: "int", nullable: false),
                    MatchesLost = table.Column<int>(type: "int", nullable: false),
                    MatchesDrawn = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Standings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Standings_TournamentPlayers_TournamentPlayerId",
                        column: x => x.TournamentPlayerId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Standings_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopCutBrackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    Round = table.Column<int>(type: "int", nullable: false),
                    MatchPosition = table.Column<int>(type: "int", nullable: false),
                    PlayerAId = table.Column<int>(type: "int", nullable: true),
                    PlayerBId = table.Column<int>(type: "int", nullable: true),
                    WinnerId = table.Column<int>(type: "int", nullable: true),
                    NextBracketId = table.Column<int>(type: "int", nullable: true),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopCutBrackets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopCutBrackets_TopCutBrackets_NextBracketId",
                        column: x => x.NextBracketId,
                        principalTable: "TopCutBrackets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopCutBrackets_TournamentPlayers_PlayerAId",
                        column: x => x.PlayerAId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopCutBrackets_TournamentPlayers_PlayerBId",
                        column: x => x.PlayerBId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopCutBrackets_TournamentPlayers_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopCutBrackets_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoundId = table.Column<int>(type: "int", nullable: false),
                    TableNumber = table.Column<int>(type: "int", nullable: false),
                    PlayerAId = table.Column<int>(type: "int", nullable: true),
                    PlayerBId = table.Column<int>(type: "int", nullable: true),
                    IsBye = table.Column<bool>(type: "bit", nullable: false),
                    IsComplete = table.Column<bool>(type: "bit", nullable: false),
                    WinnerId = table.Column<int>(type: "int", nullable: true),
                    TopCutBracketId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_TopCutBrackets_TopCutBracketId",
                        column: x => x.TopCutBracketId,
                        principalTable: "TopCutBrackets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Matches_TournamentPlayers_PlayerAId",
                        column: x => x.PlayerAId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_TournamentPlayers_PlayerBId",
                        column: x => x.PlayerBId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_TournamentPlayers_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "TournamentPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    ResultType = table.Column<int>(type: "int", nullable: false),
                    PlayerAGameWins = table.Column<int>(type: "int", nullable: false),
                    PlayerBGameWins = table.Column<int>(type: "int", nullable: false),
                    PlayerAMatchPoints = table.Column<int>(type: "int", nullable: false),
                    PlayerBMatchPoints = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SyncOperation = table.Column<string>(type: "varchar(1)", unicode: false, maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchResults_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ByeHistories_TournamentId_TournamentPlayerId_RoundNumber",
                table: "ByeHistories",
                columns: new[] { "TournamentId", "TournamentPlayerId", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ByeHistories_TournamentPlayerId",
                table: "ByeHistories",
                column: "TournamentPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_PlayerAId",
                table: "Matches",
                column: "PlayerAId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_PlayerBId",
                table: "Matches",
                column: "PlayerBId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_RoundId_TableNumber",
                table: "Matches",
                columns: new[] { "RoundId", "TableNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_TopCutBracketId",
                table: "Matches",
                column: "TopCutBracketId",
                unique: true,
                filter: "[TopCutBracketId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_WinnerId",
                table: "Matches",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResults_MatchId",
                table: "MatchResults",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizerUsers_Username",
                table: "OrganizerUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ExternalPlayerId",
                table: "Players",
                column: "ExternalPlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Name",
                table: "Players",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Rounds_TournamentId_RoundNumber",
                table: "Rounds",
                columns: new[] { "TournamentId", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Standings_TournamentId_Rank",
                table: "Standings",
                columns: new[] { "TournamentId", "Rank" });

            migrationBuilder.CreateIndex(
                name: "IX_Standings_TournamentId_TournamentPlayerId",
                table: "Standings",
                columns: new[] { "TournamentId", "TournamentPlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Standings_TournamentPlayerId",
                table: "Standings",
                column: "TournamentPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TopCutBrackets_NextBracketId",
                table: "TopCutBrackets",
                column: "NextBracketId");

            migrationBuilder.CreateIndex(
                name: "IX_TopCutBrackets_PlayerAId",
                table: "TopCutBrackets",
                column: "PlayerAId");

            migrationBuilder.CreateIndex(
                name: "IX_TopCutBrackets_PlayerBId",
                table: "TopCutBrackets",
                column: "PlayerBId");

            migrationBuilder.CreateIndex(
                name: "IX_TopCutBrackets_TournamentId_Round_MatchPosition",
                table: "TopCutBrackets",
                columns: new[] { "TournamentId", "Round", "MatchPosition" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TopCutBrackets_WinnerId",
                table: "TopCutBrackets",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentPlayers_PlayerId",
                table: "TournamentPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentPlayers_TournamentId_PlayerId",
                table: "TournamentPlayers",
                columns: new[] { "TournamentId", "PlayerId" },
                unique: true,
                filter: "[SyncOperation] <> 'D'");

            migrationBuilder.CreateIndex(
                name: "IX_TournamentPlayers_TournamentId_PlayerNumber",
                table: "TournamentPlayers",
                columns: new[] { "TournamentId", "PlayerNumber" },
                unique: true,
                filter: "[SyncOperation] <> 'D'");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_EventDate",
                table: "Tournaments",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_Status",
                table: "Tournaments",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ByeHistories");

            migrationBuilder.DropTable(
                name: "MatchResults");

            migrationBuilder.DropTable(
                name: "OrganizerUsers");

            migrationBuilder.DropTable(
                name: "Standings");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropTable(
                name: "TopCutBrackets");

            migrationBuilder.DropTable(
                name: "TournamentPlayers");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Tournaments");
        }
    }
}
