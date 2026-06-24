using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TcgTournamentManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ByeHistories_TournamentPlayers_TournamentPlayerId",
                table: "ByeHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ByeHistories_Tournaments_TournamentId",
                table: "ByeHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Rounds_RoundId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_TopCutBrackets_TopCutBracketId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_TournamentPlayers_PlayerAId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_TournamentPlayers_PlayerBId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_TournamentPlayers_WinnerId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchResults_Matches_MatchId",
                table: "MatchResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Rounds_Tournaments_TournamentId",
                table: "Rounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Standings_TournamentPlayers_TournamentPlayerId",
                table: "Standings");

            migrationBuilder.DropForeignKey(
                name: "FK_Standings_Tournaments_TournamentId",
                table: "Standings");

            migrationBuilder.DropForeignKey(
                name: "FK_TopCutBrackets_TopCutBrackets_NextBracketId",
                table: "TopCutBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TopCutBrackets_TournamentPlayers_PlayerAId",
                table: "TopCutBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TopCutBrackets_TournamentPlayers_PlayerBId",
                table: "TopCutBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TopCutBrackets_TournamentPlayers_WinnerId",
                table: "TopCutBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TopCutBrackets_Tournaments_TournamentId",
                table: "TopCutBrackets");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_Players_PlayerId",
                table: "TournamentPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tournaments",
                table: "Tournaments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TournamentPlayers",
                table: "TournamentPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TopCutBrackets",
                table: "TopCutBrackets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Standings",
                table: "Standings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rounds",
                table: "Rounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizerUsers",
                table: "OrganizerUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchResults",
                table: "MatchResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Matches",
                table: "Matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ByeHistories",
                table: "ByeHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "Tournaments",
                newName: "MST_TOURNAMENT");

            migrationBuilder.RenameTable(
                name: "TournamentPlayers",
                newName: "TOURNAMENT_PLAYER");

            migrationBuilder.RenameTable(
                name: "TopCutBrackets",
                newName: "TOP_CUT_BRACKET");

            migrationBuilder.RenameTable(
                name: "Standings",
                newName: "TOURNAMENT_STANDING");

            migrationBuilder.RenameTable(
                name: "Rounds",
                newName: "MST_ROUND");

            migrationBuilder.RenameTable(
                name: "Players",
                newName: "MST_PLAYER");

            migrationBuilder.RenameTable(
                name: "OrganizerUsers",
                newName: "MST_ORGANIZER_USER");

            migrationBuilder.RenameTable(
                name: "MatchResults",
                newName: "MATCH_RESULT");

            migrationBuilder.RenameTable(
                name: "Matches",
                newName: "MST_MATCH");

            migrationBuilder.RenameTable(
                name: "ByeHistories",
                newName: "BYE_HISTORY");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AUDIT_LOG");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_Status",
                table: "MST_TOURNAMENT",
                newName: "IX_MST_TOURNAMENT_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Tournaments_EventDate",
                table: "MST_TOURNAMENT",
                newName: "IX_MST_TOURNAMENT_EventDate");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlayers_TournamentId_PlayerNumber",
                table: "TOURNAMENT_PLAYER",
                newName: "IX_TOURNAMENT_PLAYER_TournamentId_PlayerNumber");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlayers_TournamentId_PlayerId",
                table: "TOURNAMENT_PLAYER",
                newName: "IX_TOURNAMENT_PLAYER_TournamentId_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TournamentPlayers_PlayerId",
                table: "TOURNAMENT_PLAYER",
                newName: "IX_TOURNAMENT_PLAYER_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TopCutBrackets_WinnerId",
                table: "TOP_CUT_BRACKET",
                newName: "IX_TOP_CUT_BRACKET_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_TopCutBrackets_TournamentId_Round_MatchPosition",
                table: "TOP_CUT_BRACKET",
                newName: "IX_TOP_CUT_BRACKET_TournamentId_Round_MatchPosition");

            migrationBuilder.RenameIndex(
                name: "IX_TopCutBrackets_PlayerBId",
                table: "TOP_CUT_BRACKET",
                newName: "IX_TOP_CUT_BRACKET_PlayerBId");

            migrationBuilder.RenameIndex(
                name: "IX_TopCutBrackets_PlayerAId",
                table: "TOP_CUT_BRACKET",
                newName: "IX_TOP_CUT_BRACKET_PlayerAId");

            migrationBuilder.RenameIndex(
                name: "IX_TopCutBrackets_NextBracketId",
                table: "TOP_CUT_BRACKET",
                newName: "IX_TOP_CUT_BRACKET_NextBracketId");

            migrationBuilder.RenameIndex(
                name: "IX_Standings_TournamentPlayerId",
                table: "TOURNAMENT_STANDING",
                newName: "IX_TOURNAMENT_STANDING_TournamentPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Standings_TournamentId_TournamentPlayerId",
                table: "TOURNAMENT_STANDING",
                newName: "IX_TOURNAMENT_STANDING_TournamentId_TournamentPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Standings_TournamentId_Rank",
                table: "TOURNAMENT_STANDING",
                newName: "IX_TOURNAMENT_STANDING_TournamentId_Rank");

            migrationBuilder.RenameIndex(
                name: "IX_Rounds_TournamentId_RoundNumber",
                table: "MST_ROUND",
                newName: "IX_MST_ROUND_TournamentId_RoundNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Players_Name",
                table: "MST_PLAYER",
                newName: "IX_MST_PLAYER_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Players_ExternalPlayerId",
                table: "MST_PLAYER",
                newName: "IX_MST_PLAYER_ExternalPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizerUsers_Username",
                table: "MST_ORGANIZER_USER",
                newName: "IX_MST_ORGANIZER_USER_Username");

            migrationBuilder.RenameIndex(
                name: "IX_MatchResults_MatchId",
                table: "MATCH_RESULT",
                newName: "IX_MATCH_RESULT_MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_WinnerId",
                table: "MST_MATCH",
                newName: "IX_MST_MATCH_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_TopCutBracketId",
                table: "MST_MATCH",
                newName: "IX_MST_MATCH_TopCutBracketId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_RoundId_TableNumber",
                table: "MST_MATCH",
                newName: "IX_MST_MATCH_RoundId_TableNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_PlayerBId",
                table: "MST_MATCH",
                newName: "IX_MST_MATCH_PlayerBId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_PlayerAId",
                table: "MST_MATCH",
                newName: "IX_MST_MATCH_PlayerAId");

            migrationBuilder.RenameIndex(
                name: "IX_ByeHistories_TournamentPlayerId",
                table: "BYE_HISTORY",
                newName: "IX_BYE_HISTORY_TournamentPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_ByeHistories_TournamentId_TournamentPlayerId_RoundNumber",
                table: "BYE_HISTORY",
                newName: "IX_BYE_HISTORY_TournamentId_TournamentPlayerId_RoundNumber");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AUDIT_LOG",
                newName: "IX_AUDIT_LOG_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MST_TOURNAMENT",
                table: "MST_TOURNAMENT",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TOURNAMENT_PLAYER",
                table: "TOURNAMENT_PLAYER",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TOP_CUT_BRACKET",
                table: "TOP_CUT_BRACKET",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TOURNAMENT_STANDING",
                table: "TOURNAMENT_STANDING",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MST_ROUND",
                table: "MST_ROUND",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MST_PLAYER",
                table: "MST_PLAYER",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MST_ORGANIZER_USER",
                table: "MST_ORGANIZER_USER",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MATCH_RESULT",
                table: "MATCH_RESULT",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MST_MATCH",
                table: "MST_MATCH",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BYE_HISTORY",
                table: "BYE_HISTORY",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_LOG",
                table: "AUDIT_LOG",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BYE_HISTORY_MST_TOURNAMENT_TournamentId",
                table: "BYE_HISTORY",
                column: "TournamentId",
                principalTable: "MST_TOURNAMENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BYE_HISTORY_TOURNAMENT_PLAYER_TournamentPlayerId",
                table: "BYE_HISTORY",
                column: "TournamentPlayerId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MATCH_RESULT_MST_MATCH_MatchId",
                table: "MATCH_RESULT",
                column: "MatchId",
                principalTable: "MST_MATCH",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MST_MATCH_MST_ROUND_RoundId",
                table: "MST_MATCH",
                column: "RoundId",
                principalTable: "MST_ROUND",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MST_MATCH_TOP_CUT_BRACKET_TopCutBracketId",
                table: "MST_MATCH",
                column: "TopCutBracketId",
                principalTable: "TOP_CUT_BRACKET",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MST_MATCH_TOURNAMENT_PLAYER_PlayerAId",
                table: "MST_MATCH",
                column: "PlayerAId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MST_MATCH_TOURNAMENT_PLAYER_PlayerBId",
                table: "MST_MATCH",
                column: "PlayerBId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MST_MATCH_TOURNAMENT_PLAYER_WinnerId",
                table: "MST_MATCH",
                column: "WinnerId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MST_ROUND_MST_TOURNAMENT_TournamentId",
                table: "MST_ROUND",
                column: "TournamentId",
                principalTable: "MST_TOURNAMENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TOP_CUT_BRACKET_MST_TOURNAMENT_TournamentId",
                table: "TOP_CUT_BRACKET",
                column: "TournamentId",
                principalTable: "MST_TOURNAMENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOP_CUT_BRACKET_NextBracketId",
                table: "TOP_CUT_BRACKET",
                column: "NextBracketId",
                principalTable: "TOP_CUT_BRACKET",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOURNAMENT_PLAYER_PlayerAId",
                table: "TOP_CUT_BRACKET",
                column: "PlayerAId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOURNAMENT_PLAYER_PlayerBId",
                table: "TOP_CUT_BRACKET",
                column: "PlayerBId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOURNAMENT_PLAYER_WinnerId",
                table: "TOP_CUT_BRACKET",
                column: "WinnerId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TOURNAMENT_PLAYER_MST_PLAYER_PlayerId",
                table: "TOURNAMENT_PLAYER",
                column: "PlayerId",
                principalTable: "MST_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TOURNAMENT_PLAYER_MST_TOURNAMENT_TournamentId",
                table: "TOURNAMENT_PLAYER",
                column: "TournamentId",
                principalTable: "MST_TOURNAMENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TOURNAMENT_STANDING_MST_TOURNAMENT_TournamentId",
                table: "TOURNAMENT_STANDING",
                column: "TournamentId",
                principalTable: "MST_TOURNAMENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TOURNAMENT_STANDING_TOURNAMENT_PLAYER_TournamentPlayerId",
                table: "TOURNAMENT_STANDING",
                column: "TournamentPlayerId",
                principalTable: "TOURNAMENT_PLAYER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BYE_HISTORY_MST_TOURNAMENT_TournamentId",
                table: "BYE_HISTORY");

            migrationBuilder.DropForeignKey(
                name: "FK_BYE_HISTORY_TOURNAMENT_PLAYER_TournamentPlayerId",
                table: "BYE_HISTORY");

            migrationBuilder.DropForeignKey(
                name: "FK_MATCH_RESULT_MST_MATCH_MatchId",
                table: "MATCH_RESULT");

            migrationBuilder.DropForeignKey(
                name: "FK_MST_MATCH_MST_ROUND_RoundId",
                table: "MST_MATCH");

            migrationBuilder.DropForeignKey(
                name: "FK_MST_MATCH_TOP_CUT_BRACKET_TopCutBracketId",
                table: "MST_MATCH");

            migrationBuilder.DropForeignKey(
                name: "FK_MST_MATCH_TOURNAMENT_PLAYER_PlayerAId",
                table: "MST_MATCH");

            migrationBuilder.DropForeignKey(
                name: "FK_MST_MATCH_TOURNAMENT_PLAYER_PlayerBId",
                table: "MST_MATCH");

            migrationBuilder.DropForeignKey(
                name: "FK_MST_MATCH_TOURNAMENT_PLAYER_WinnerId",
                table: "MST_MATCH");

            migrationBuilder.DropForeignKey(
                name: "FK_MST_ROUND_MST_TOURNAMENT_TournamentId",
                table: "MST_ROUND");

            migrationBuilder.DropForeignKey(
                name: "FK_TOP_CUT_BRACKET_MST_TOURNAMENT_TournamentId",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOP_CUT_BRACKET_NextBracketId",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOURNAMENT_PLAYER_PlayerAId",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOURNAMENT_PLAYER_PlayerBId",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropForeignKey(
                name: "FK_TOP_CUT_BRACKET_TOURNAMENT_PLAYER_WinnerId",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropForeignKey(
                name: "FK_TOURNAMENT_PLAYER_MST_PLAYER_PlayerId",
                table: "TOURNAMENT_PLAYER");

            migrationBuilder.DropForeignKey(
                name: "FK_TOURNAMENT_PLAYER_MST_TOURNAMENT_TournamentId",
                table: "TOURNAMENT_PLAYER");

            migrationBuilder.DropForeignKey(
                name: "FK_TOURNAMENT_STANDING_MST_TOURNAMENT_TournamentId",
                table: "TOURNAMENT_STANDING");

            migrationBuilder.DropForeignKey(
                name: "FK_TOURNAMENT_STANDING_TOURNAMENT_PLAYER_TournamentPlayerId",
                table: "TOURNAMENT_STANDING");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TOURNAMENT_STANDING",
                table: "TOURNAMENT_STANDING");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TOURNAMENT_PLAYER",
                table: "TOURNAMENT_PLAYER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TOP_CUT_BRACKET",
                table: "TOP_CUT_BRACKET");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MST_TOURNAMENT",
                table: "MST_TOURNAMENT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MST_ROUND",
                table: "MST_ROUND");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MST_PLAYER",
                table: "MST_PLAYER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MST_ORGANIZER_USER",
                table: "MST_ORGANIZER_USER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MST_MATCH",
                table: "MST_MATCH");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MATCH_RESULT",
                table: "MATCH_RESULT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BYE_HISTORY",
                table: "BYE_HISTORY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_LOG",
                table: "AUDIT_LOG");

            migrationBuilder.RenameTable(
                name: "TOURNAMENT_STANDING",
                newName: "Standings");

            migrationBuilder.RenameTable(
                name: "TOURNAMENT_PLAYER",
                newName: "TournamentPlayers");

            migrationBuilder.RenameTable(
                name: "TOP_CUT_BRACKET",
                newName: "TopCutBrackets");

            migrationBuilder.RenameTable(
                name: "MST_TOURNAMENT",
                newName: "Tournaments");

            migrationBuilder.RenameTable(
                name: "MST_ROUND",
                newName: "Rounds");

            migrationBuilder.RenameTable(
                name: "MST_PLAYER",
                newName: "Players");

            migrationBuilder.RenameTable(
                name: "MST_ORGANIZER_USER",
                newName: "OrganizerUsers");

            migrationBuilder.RenameTable(
                name: "MST_MATCH",
                newName: "Matches");

            migrationBuilder.RenameTable(
                name: "MATCH_RESULT",
                newName: "MatchResults");

            migrationBuilder.RenameTable(
                name: "BYE_HISTORY",
                newName: "ByeHistories");

            migrationBuilder.RenameTable(
                name: "AUDIT_LOG",
                newName: "AuditLogs");

            migrationBuilder.RenameIndex(
                name: "IX_TOURNAMENT_STANDING_TournamentPlayerId",
                table: "Standings",
                newName: "IX_Standings_TournamentPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TOURNAMENT_STANDING_TournamentId_TournamentPlayerId",
                table: "Standings",
                newName: "IX_Standings_TournamentId_TournamentPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TOURNAMENT_STANDING_TournamentId_Rank",
                table: "Standings",
                newName: "IX_Standings_TournamentId_Rank");

            migrationBuilder.RenameIndex(
                name: "IX_TOURNAMENT_PLAYER_TournamentId_PlayerNumber",
                table: "TournamentPlayers",
                newName: "IX_TournamentPlayers_TournamentId_PlayerNumber");

            migrationBuilder.RenameIndex(
                name: "IX_TOURNAMENT_PLAYER_TournamentId_PlayerId",
                table: "TournamentPlayers",
                newName: "IX_TournamentPlayers_TournamentId_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TOURNAMENT_PLAYER_PlayerId",
                table: "TournamentPlayers",
                newName: "IX_TournamentPlayers_PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_TOP_CUT_BRACKET_WinnerId",
                table: "TopCutBrackets",
                newName: "IX_TopCutBrackets_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_TOP_CUT_BRACKET_TournamentId_Round_MatchPosition",
                table: "TopCutBrackets",
                newName: "IX_TopCutBrackets_TournamentId_Round_MatchPosition");

            migrationBuilder.RenameIndex(
                name: "IX_TOP_CUT_BRACKET_PlayerBId",
                table: "TopCutBrackets",
                newName: "IX_TopCutBrackets_PlayerBId");

            migrationBuilder.RenameIndex(
                name: "IX_TOP_CUT_BRACKET_PlayerAId",
                table: "TopCutBrackets",
                newName: "IX_TopCutBrackets_PlayerAId");

            migrationBuilder.RenameIndex(
                name: "IX_TOP_CUT_BRACKET_NextBracketId",
                table: "TopCutBrackets",
                newName: "IX_TopCutBrackets_NextBracketId");

            migrationBuilder.RenameIndex(
                name: "IX_MST_TOURNAMENT_Status",
                table: "Tournaments",
                newName: "IX_Tournaments_Status");

            migrationBuilder.RenameIndex(
                name: "IX_MST_TOURNAMENT_EventDate",
                table: "Tournaments",
                newName: "IX_Tournaments_EventDate");

            migrationBuilder.RenameIndex(
                name: "IX_MST_ROUND_TournamentId_RoundNumber",
                table: "Rounds",
                newName: "IX_Rounds_TournamentId_RoundNumber");

            migrationBuilder.RenameIndex(
                name: "IX_MST_PLAYER_Name",
                table: "Players",
                newName: "IX_Players_Name");

            migrationBuilder.RenameIndex(
                name: "IX_MST_PLAYER_ExternalPlayerId",
                table: "Players",
                newName: "IX_Players_ExternalPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_MST_ORGANIZER_USER_Username",
                table: "OrganizerUsers",
                newName: "IX_OrganizerUsers_Username");

            migrationBuilder.RenameIndex(
                name: "IX_MST_MATCH_WinnerId",
                table: "Matches",
                newName: "IX_Matches_WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_MST_MATCH_TopCutBracketId",
                table: "Matches",
                newName: "IX_Matches_TopCutBracketId");

            migrationBuilder.RenameIndex(
                name: "IX_MST_MATCH_RoundId_TableNumber",
                table: "Matches",
                newName: "IX_Matches_RoundId_TableNumber");

            migrationBuilder.RenameIndex(
                name: "IX_MST_MATCH_PlayerBId",
                table: "Matches",
                newName: "IX_Matches_PlayerBId");

            migrationBuilder.RenameIndex(
                name: "IX_MST_MATCH_PlayerAId",
                table: "Matches",
                newName: "IX_Matches_PlayerAId");

            migrationBuilder.RenameIndex(
                name: "IX_MATCH_RESULT_MatchId",
                table: "MatchResults",
                newName: "IX_MatchResults_MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_BYE_HISTORY_TournamentPlayerId",
                table: "ByeHistories",
                newName: "IX_ByeHistories_TournamentPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_BYE_HISTORY_TournamentId_TournamentPlayerId_RoundNumber",
                table: "ByeHistories",
                newName: "IX_ByeHistories_TournamentId_TournamentPlayerId_RoundNumber");

            migrationBuilder.RenameIndex(
                name: "IX_AUDIT_LOG_CreatedAt",
                table: "AuditLogs",
                newName: "IX_AuditLogs_CreatedAt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Standings",
                table: "Standings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TournamentPlayers",
                table: "TournamentPlayers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TopCutBrackets",
                table: "TopCutBrackets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tournaments",
                table: "Tournaments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rounds",
                table: "Rounds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizerUsers",
                table: "OrganizerUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Matches",
                table: "Matches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchResults",
                table: "MatchResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ByeHistories",
                table: "ByeHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ByeHistories_TournamentPlayers_TournamentPlayerId",
                table: "ByeHistories",
                column: "TournamentPlayerId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ByeHistories_Tournaments_TournamentId",
                table: "ByeHistories",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Rounds_RoundId",
                table: "Matches",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_TopCutBrackets_TopCutBracketId",
                table: "Matches",
                column: "TopCutBracketId",
                principalTable: "TopCutBrackets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_TournamentPlayers_PlayerAId",
                table: "Matches",
                column: "PlayerAId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_TournamentPlayers_PlayerBId",
                table: "Matches",
                column: "PlayerBId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_TournamentPlayers_WinnerId",
                table: "Matches",
                column: "WinnerId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchResults_Matches_MatchId",
                table: "MatchResults",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rounds_Tournaments_TournamentId",
                table: "Rounds",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Standings_TournamentPlayers_TournamentPlayerId",
                table: "Standings",
                column: "TournamentPlayerId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Standings_Tournaments_TournamentId",
                table: "Standings",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopCutBrackets_TopCutBrackets_NextBracketId",
                table: "TopCutBrackets",
                column: "NextBracketId",
                principalTable: "TopCutBrackets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TopCutBrackets_TournamentPlayers_PlayerAId",
                table: "TopCutBrackets",
                column: "PlayerAId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TopCutBrackets_TournamentPlayers_PlayerBId",
                table: "TopCutBrackets",
                column: "PlayerBId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TopCutBrackets_TournamentPlayers_WinnerId",
                table: "TopCutBrackets",
                column: "WinnerId",
                principalTable: "TournamentPlayers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TopCutBrackets_Tournaments_TournamentId",
                table: "TopCutBrackets",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_Players_PlayerId",
                table: "TournamentPlayers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
