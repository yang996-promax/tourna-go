using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Tests;

/// <summary>
/// EF-backed executor for in-memory tests where SQL Server stored procedures are unavailable.
/// Mirrors the sync semantics implemented in Sql/StoredProcedures.sql.
/// </summary>
public class EfStoredProcedureExecutor : StoredProcedureExecutor
{
    public EfStoredProcedureExecutor(TournamentDbContext db) : base(db) { }

    public override Task ExecuteAsync(string procedureName, CancellationToken ct = default, params SqlParameter[] parameters)
        => ExecuteEfAsync(procedureName, ct, parameters);

    public override async Task<int> ExecuteWithIntOutputAsync(
        string procedureName, string outputParameterName, CancellationToken ct = default, params SqlParameter[] parameters)
        => await ExecuteEfAsync(procedureName, ct, parameters);

    private async Task<int> ExecuteEfAsync(string procedureName, CancellationToken ct, SqlParameter[] parameters)
    {
        var p = parameters.ToDictionary(x => x.ParameterName.TrimStart('@'), x => x.Value == DBNull.Value ? null : x.Value);

        switch (procedureName)
        {
            case "usp_Tournament_Insert":
                return await InsertAsync(new Tournament
                {
                    Name = (string)p["Name"]!,
                    GameTitle = (string)p["GameTitle"]!,
                    EventDate = (DateTime)p["EventDate"]!,
                    Organizer = (string)p["Organizer"]!,
                    Venue = (string)p["Venue"]!,
                    TotalSwissRounds = (int)p["TotalSwissRounds"]!,
                    TopCutSize = (TopCutSize)(int)p["TopCutSize"]!,
                    FirstRoundPairingMode = (FirstRoundPairingMode)(int)p["FirstRoundPairingMode"]!,
                    MatchFormat = (MatchFormat)(int)p["MatchFormat"]!,
                    HasElimination = (bool)p["HasElimination"]!,
                    EliminationLossCount = p["EliminationLossCount"] as int?,
                    Status = (TournamentStatus)(int)p["Status"]!,
                    CurrentRound = (int)p["CurrentRound"]!
                }, ct);

            case "usp_Tournament_Update":
                await UpdateTournamentAsync((int)p["Id"]!, p, ct);
                return 0;

            case "usp_Tournament_SoftDelete":
                await SoftDeleteAsync<Tournament>((int)p["Id"]!, ct);
                return 0;

            case "usp_Player_Insert":
                return await InsertAsync(new Player
                {
                    ExternalPlayerId = (string)p["ExternalPlayerId"]!,
                    Name = (string)p["Name"]!,
                    ContactNumber = p["ContactNumber"] as string
                }, ct);

            case "usp_Player_Update":
                await UpdatePlayerAsync((int)p["Id"]!, p, ct);
                return 0;

            case "usp_TournamentPlayer_Insert":
                return await InsertAsync(new TournamentPlayer
                {
                    TournamentId = (int)p["TournamentId"]!,
                    PlayerId = (int)p["PlayerId"]!,
                    PlayerNumber = (int)p["PlayerNumber"]!,
                    DeckName = p["DeckName"] as string,
                    IsDropped = (bool)p["IsDropped"]!,
                    RegisteredAt = (DateTime)p["RegisteredAt"]!
                }, ct);

            case "usp_TournamentPlayer_Update":
                await UpdateTournamentPlayerAsync((int)p["Id"]!, p, ct);
                return 0;

            case "usp_TournamentPlayer_SoftDelete":
                await SoftDeleteAsync<TournamentPlayer>((int)p["Id"]!, ct);
                return 0;

            case "usp_Round_Insert":
                return await InsertAsync(new Round
                {
                    TournamentId = (int)p["TournamentId"]!,
                    RoundNumber = (int)p["RoundNumber"]!,
                    RoundType = (RoundType)(int)p["RoundType"]!,
                    IsComplete = (bool)p["IsComplete"]!,
                    CompletedAt = p["CompletedAt"] as DateTime?
                }, ct);

            case "usp_Round_Update":
                await UpdateRoundAsync((int)p["Id"]!, p, ct);
                return 0;

            case "usp_Match_Insert":
                return await InsertAsync(new Match
                {
                    RoundId = (int)p["RoundId"]!,
                    TableNumber = (int)p["TableNumber"]!,
                    PlayerAId = p["PlayerAId"] as int?,
                    PlayerBId = p["PlayerBId"] as int?,
                    IsBye = (bool)p["IsBye"]!,
                    IsComplete = (bool)p["IsComplete"]!,
                    WinnerId = p["WinnerId"] as int?,
                    TopCutBracketId = p["TopCutBracketId"] as int?
                }, ct);

            case "usp_Match_Update":
                await UpdateMatchAsync((int)p["Id"]!, p, ct);
                return 0;

            case "usp_MatchResult_Insert":
                return await InsertAsync(new MatchResult
                {
                    MatchId = (int)p["MatchId"]!,
                    ResultType = (MatchResultType)(int)p["ResultType"]!,
                    PlayerAGameWins = (int)p["PlayerAGameWins"]!,
                    PlayerBGameWins = (int)p["PlayerBGameWins"]!,
                    PlayerAMatchPoints = (int)p["PlayerAMatchPoints"]!,
                    PlayerBMatchPoints = (int)p["PlayerBMatchPoints"]!,
                    RecordedAt = (DateTime)p["RecordedAt"]!
                }, ct);

            case "usp_MatchResult_Update":
                await UpdateMatchResultAsync((int)p["Id"]!, p, ct);
                return 0;

            case "usp_Standing_DeleteByTournament":
                await DeleteStandingsAsync((int)p["TournamentId"]!, ct);
                return 0;

            case "usp_Standing_Insert":
                return await InsertAsync(new Standing
                {
                    TournamentId = (int)p["TournamentId"]!,
                    TournamentPlayerId = (int)p["TournamentPlayerId"]!,
                    Rank = (int)p["Rank"]!,
                    MatchPoints = (int)p["MatchPoints"]!,
                    GameWins = (int)p["GameWins"]!,
                    GameLosses = (int)p["GameLosses"]!,
                    OMWPercent = (decimal)p["OMWPercent"]!,
                    GWPercent = (decimal)p["GWPercent"]!,
                    OGWPercent = (decimal)p["OGWPercent"]!,
                    MatchesPlayed = (int)p["MatchesPlayed"]!,
                    MatchesWon = (int)p["MatchesWon"]!,
                    MatchesLost = (int)p["MatchesLost"]!,
                    MatchesDrawn = (int)p["MatchesDrawn"]!
                }, ct);

            case "usp_ByeHistory_Insert":
                return await InsertAsync(new ByeHistory
                {
                    TournamentId = (int)p["TournamentId"]!,
                    TournamentPlayerId = (int)p["TournamentPlayerId"]!,
                    RoundNumber = (int)p["RoundNumber"]!,
                    AssignedAt = (DateTime)p["AssignedAt"]!
                }, ct);

            case "usp_TopCutBracket_Insert":
                return await InsertAsync(new TopCutBracket
                {
                    TournamentId = (int)p["TournamentId"]!,
                    Round = (TopCutRound)(int)p["Round"]!,
                    MatchPosition = (int)p["MatchPosition"]!,
                    PlayerAId = p["PlayerAId"] as int?,
                    PlayerBId = p["PlayerBId"] as int?,
                    WinnerId = p["WinnerId"] as int?,
                    NextBracketId = p["NextBracketId"] as int?,
                    IsComplete = (bool)p["IsComplete"]!
                }, ct);

            case "usp_TopCutBracket_Update":
                await UpdateTopCutBracketAsync((int)p["Id"]!, p, ct);
                return 0;

            case "usp_TopCutBracket_DeleteByTournament":
                await DeleteTopCutBracketsAsync((int)p["TournamentId"]!, ct);
                return 0;

            case "usp_AuditLog_Insert":
                return await InsertAsync(new AuditLog
                {
                    Action = (string)p["Action"]!,
                    EntityType = (string)p["EntityType"]!,
                    EntityId = p["EntityId"] as int?,
                    Details = p["Details"] as string,
                    PerformedBy = p["PerformedBy"] as string
                }, ct);

            case "usp_OrganizerUser_Insert":
                return await InsertAsync(new OrganizerUser
                {
                    Username = (string)p["Username"]!,
                    PasswordHash = (string)p["PasswordHash"]!,
                    DisplayName = (string)p["DisplayName"]!,
                    IsActive = (bool)p["IsActive"]!
                }, ct);

            default:
                throw new NotSupportedException($"Procedure '{procedureName}' is not emulated for in-memory tests.");
        }
    }

    private async Task<int> InsertAsync<T>(T entity, CancellationToken ct) where T : class
    {
        Db.Set<T>().Add(entity);
        await Db.SaveChangesAsync(ct);
        return (int)typeof(T).GetProperty("Id")!.GetValue(entity)!;
    }

    private async Task SoftDeleteAsync<T>(int id, CancellationToken ct) where T : class, ISyncTrackable
    {
        var entity = await Db.Set<T>().IgnoreQueryFilters().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, ct);
        if (entity == null || entity.SyncOperation == SyncOperation.D) return;
        entity.MarkDeleted();
        await Db.SaveChangesAsync(ct);
    }

    private async Task UpdateTournamentAsync(int id, Dictionary<string, object?> p, CancellationToken ct)
    {
        var t = await Db.Tournaments.FindAsync([id], ct) ?? throw new InvalidOperationException("Tournament not found.");
        t.Name = (string)p["Name"]!;
        t.GameTitle = (string)p["GameTitle"]!;
        t.EventDate = (DateTime)p["EventDate"]!;
        t.Organizer = (string)p["Organizer"]!;
        t.Venue = (string)p["Venue"]!;
        t.TotalSwissRounds = (int)p["TotalSwissRounds"]!;
        t.TopCutSize = (TopCutSize)(int)p["TopCutSize"]!;
        t.FirstRoundPairingMode = (FirstRoundPairingMode)(int)p["FirstRoundPairingMode"]!;
        t.MatchFormat = (MatchFormat)(int)p["MatchFormat"]!;
        t.HasElimination = (bool)p["HasElimination"]!;
        t.EliminationLossCount = p["EliminationLossCount"] as int?;
        t.Status = (TournamentStatus)(int)p["Status"]!;
        t.CurrentRound = (int)p["CurrentRound"]!;
        await Db.SaveChangesAsync(ct);
    }

    private async Task UpdatePlayerAsync(int id, Dictionary<string, object?> p, CancellationToken ct)
    {
        var entity = await Db.Players.FindAsync([id], ct) ?? throw new InvalidOperationException("Player not found.");
        entity.ExternalPlayerId = (string)p["ExternalPlayerId"]!;
        entity.Name = (string)p["Name"]!;
        entity.ContactNumber = p["ContactNumber"] as string;
        await Db.SaveChangesAsync(ct);
    }

    private async Task UpdateTournamentPlayerAsync(int id, Dictionary<string, object?> p, CancellationToken ct)
    {
        var entity = await Db.TournamentPlayers.IgnoreQueryFilters().FirstOrDefaultAsync(tp => tp.Id == id, ct)
            ?? throw new InvalidOperationException("TournamentPlayer not found.");
        entity.PlayerNumber = (int)p["PlayerNumber"]!;
        entity.DeckName = p["DeckName"] as string;
        entity.IsDropped = (bool)p["IsDropped"]!;
        entity.RegisteredAt = (DateTime)p["RegisteredAt"]!;
        await Db.SaveChangesAsync(ct);
    }

    private async Task UpdateRoundAsync(int id, Dictionary<string, object?> p, CancellationToken ct)
    {
        var entity = await Db.Rounds.FindAsync([id], ct) ?? throw new InvalidOperationException("Round not found.");
        entity.IsComplete = (bool)p["IsComplete"]!;
        entity.CompletedAt = p["CompletedAt"] as DateTime?;
        await Db.SaveChangesAsync(ct);
    }

    private async Task UpdateMatchAsync(int id, Dictionary<string, object?> p, CancellationToken ct)
    {
        var entity = await Db.Matches.FindAsync([id], ct) ?? throw new InvalidOperationException("Match not found.");
        entity.TableNumber = (int)p["TableNumber"]!;
        entity.PlayerAId = p["PlayerAId"] as int?;
        entity.PlayerBId = p["PlayerBId"] as int?;
        entity.IsBye = (bool)p["IsBye"]!;
        entity.IsComplete = (bool)p["IsComplete"]!;
        entity.WinnerId = p["WinnerId"] as int?;
        entity.TopCutBracketId = p["TopCutBracketId"] as int?;
        await Db.SaveChangesAsync(ct);
    }

    private async Task UpdateMatchResultAsync(int id, Dictionary<string, object?> p, CancellationToken ct)
    {
        var entity = await Db.MatchResults.FindAsync([id], ct) ?? throw new InvalidOperationException("MatchResult not found.");
        entity.ResultType = (MatchResultType)(int)p["ResultType"]!;
        entity.PlayerAGameWins = (int)p["PlayerAGameWins"]!;
        entity.PlayerBGameWins = (int)p["PlayerBGameWins"]!;
        entity.PlayerAMatchPoints = (int)p["PlayerAMatchPoints"]!;
        entity.PlayerBMatchPoints = (int)p["PlayerBMatchPoints"]!;
        entity.RecordedAt = (DateTime)p["RecordedAt"]!;
        await Db.SaveChangesAsync(ct);
    }

    private async Task UpdateTopCutBracketAsync(int id, Dictionary<string, object?> p, CancellationToken ct)
    {
        var entity = await Db.TopCutBrackets.FindAsync([id], ct) ?? throw new InvalidOperationException("TopCutBracket not found.");
        entity.PlayerAId = p["PlayerAId"] as int?;
        entity.PlayerBId = p["PlayerBId"] as int?;
        entity.WinnerId = p["WinnerId"] as int?;
        entity.NextBracketId = p["NextBracketId"] as int?;
        entity.IsComplete = (bool)p["IsComplete"]!;
        await Db.SaveChangesAsync(ct);
    }

    private async Task DeleteStandingsAsync(int tournamentId, CancellationToken ct)
    {
        var existing = await Db.Standings.Where(s => s.TournamentId == tournamentId).ToListAsync(ct);
        Db.Standings.RemoveRange(existing);
        await Db.SaveChangesAsync(ct);
    }

    private async Task DeleteTopCutBracketsAsync(int tournamentId, CancellationToken ct)
    {
        var existing = await Db.TopCutBrackets.Where(b => b.TournamentId == tournamentId).ToListAsync(ct);
        Db.TopCutBrackets.RemoveRange(existing);
        await Db.SaveChangesAsync(ct);
    }
}