using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;
    public MatchRepository(TournamentDbContext db, StoredProcedureExecutor sp, ICurrentOrgContext _)
    {
        _db = db;
        _sp = sp;
    }

    public async Task<Round?> GetRoundAsync(int tournamentId, int roundNumber, CancellationToken ct = default, RoundType roundType = RoundType.Swiss) =>
        await _db.Rounds
            .Include(r => r.Matches).ThenInclude(m => m.PlayerA!).ThenInclude(tp => tp.Player)
            .Include(r => r.Matches).ThenInclude(m => m.PlayerB!).ThenInclude(tp => tp.Player)
            .Include(r => r.Matches).ThenInclude(m => m.Result)
            .FirstOrDefaultAsync(r => r.TournamentId == tournamentId && r.RoundNumber == roundNumber && r.RoundType == roundType, ct);

    public async Task<Round?> GetRoundByIdAsync(int roundId, CancellationToken ct = default) =>
        await _db.Rounds
            .Include(r => r.Matches).ThenInclude(m => m.PlayerA!).ThenInclude(tp => tp.Player)
            .Include(r => r.Matches).ThenInclude(m => m.PlayerB!).ThenInclude(tp => tp.Player)
            .Include(r => r.Matches).ThenInclude(m => m.Result)
            .FirstOrDefaultAsync(r => r.Id == roundId, ct);

    public async Task<IReadOnlyList<Round>> GetRoundsAsync(int tournamentId, bool swissOnly = false, CancellationToken ct = default)
    {
        var query = _db.Rounds
            .Include(r => r.Matches).ThenInclude(m => m.PlayerA!).ThenInclude(tp => tp.Player)
            .Include(r => r.Matches).ThenInclude(m => m.PlayerB!).ThenInclude(tp => tp.Player)
            .Include(r => r.Matches).ThenInclude(m => m.Result)
            .Where(r => r.TournamentId == tournamentId);

        if (swissOnly)
            query = query.Where(r => r.RoundType == RoundType.Swiss);

        return await query.OrderBy(r => r.RoundNumber).ToListAsync(ct);
    }

    public async Task<Match?> GetMatchByIdAsync(int matchId, CancellationToken ct = default) =>
        await _db.Matches
            .Include(m => m.Round)
            .Include(m => m.PlayerA!).ThenInclude(tp => tp.Player)
            .Include(m => m.PlayerB!).ThenInclude(tp => tp.Player)
            .Include(m => m.Result)
            .FirstOrDefaultAsync(m => m.Id == matchId, ct);

    public async Task<Round> CreateRoundAsync(Round round, CancellationToken ct = default)
    {
        var id = await _sp.ExecuteWithIntOutputAsync("usp_Round_Insert", "@Id", ct,
            StoredProcedureExecutor.Int("@TournamentId", round.TournamentId),
            StoredProcedureExecutor.Int("@RoundNumber", round.RoundNumber),
            StoredProcedureExecutor.Int("@RoundType", (int)round.RoundType),
            StoredProcedureExecutor.Bit("@IsComplete", round.IsComplete),
            StoredProcedureExecutor.DateTimeNull("@CompletedAt", round.CompletedAt),
            StoredProcedureExecutor.Str("@OrgCD", round.OrgCD, 50),
            StoredProcedureExecutor.OutInt("@Id"));

        round.Id = id;
        return (await GetRoundByIdAsync(id, ct)) ?? round;
    }

    public async Task CreateMatchesAsync(IEnumerable<Match> matches, CancellationToken ct = default)
    {
        foreach (var match in matches)
        {
            var id = await _sp.ExecuteWithIntOutputAsync("usp_Match_Insert", "@Id", ct,
                StoredProcedureExecutor.Int("@RoundId", match.RoundId),
                StoredProcedureExecutor.Int("@TableNumber", match.TableNumber),
                StoredProcedureExecutor.IntNull("@PlayerAId", match.PlayerAId),
                StoredProcedureExecutor.IntNull("@PlayerBId", match.PlayerBId),
                StoredProcedureExecutor.Bit("@IsBye", match.IsBye),
                StoredProcedureExecutor.Bit("@IsComplete", match.IsComplete),
                StoredProcedureExecutor.IntNull("@WinnerId", match.WinnerId),
                StoredProcedureExecutor.IntNull("@TopCutBracketId", match.TopCutBracketId),
                StoredProcedureExecutor.Str("@OrgCD", match.OrgCD, 50),
                StoredProcedureExecutor.OutInt("@Id"));

            match.Id = id;
        }
    }

    public async Task UpdateMatchAsync(Match match, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_Match_Update", ct,
            StoredProcedureExecutor.Int("@Id", match.Id),
            StoredProcedureExecutor.Int("@TableNumber", match.TableNumber),
            StoredProcedureExecutor.IntNull("@PlayerAId", match.PlayerAId),
            StoredProcedureExecutor.IntNull("@PlayerBId", match.PlayerBId),
            StoredProcedureExecutor.Bit("@IsBye", match.IsBye),
            StoredProcedureExecutor.Bit("@IsComplete", match.IsComplete),
            StoredProcedureExecutor.IntNull("@WinnerId", match.WinnerId),
            StoredProcedureExecutor.IntNull("@TopCutBracketId", match.TopCutBracketId));
    }

    public async Task UpdateRoundAsync(Round round, CancellationToken ct = default)
    {
        await _sp.ExecuteAsync("usp_Round_Update", ct,
            StoredProcedureExecutor.Int("@Id", round.Id),
            StoredProcedureExecutor.Bit("@IsComplete", round.IsComplete),
            StoredProcedureExecutor.DateTimeNull("@CompletedAt", round.CompletedAt));
    }

    public async Task UpsertMatchResultAsync(MatchResult result, CancellationToken ct = default)
    {
        if (result.Id == 0)
        {
            var id = await _sp.ExecuteWithIntOutputAsync("usp_MatchResult_Insert", "@Id", ct,
                StoredProcedureExecutor.Int("@MatchId", result.MatchId),
                StoredProcedureExecutor.Int("@ResultType", (int)result.ResultType),
                StoredProcedureExecutor.Int("@PlayerAGameWins", result.PlayerAGameWins),
                StoredProcedureExecutor.Int("@PlayerBGameWins", result.PlayerBGameWins),
                StoredProcedureExecutor.Int("@PlayerAMatchPoints", result.PlayerAMatchPoints),
                StoredProcedureExecutor.Int("@PlayerBMatchPoints", result.PlayerBMatchPoints),
                StoredProcedureExecutor.DateTimeParam("@RecordedAt", result.RecordedAt),
                StoredProcedureExecutor.Str("@OrgCD", result.OrgCD, 50),
                StoredProcedureExecutor.OutInt("@Id"));

            result.Id = id;
        }
        else
        {
            await _sp.ExecuteAsync("usp_MatchResult_Update", ct,
                StoredProcedureExecutor.Int("@Id", result.Id),
                StoredProcedureExecutor.Int("@ResultType", (int)result.ResultType),
                StoredProcedureExecutor.Int("@PlayerAGameWins", result.PlayerAGameWins),
                StoredProcedureExecutor.Int("@PlayerBGameWins", result.PlayerBGameWins),
                StoredProcedureExecutor.Int("@PlayerAMatchPoints", result.PlayerAMatchPoints),
                StoredProcedureExecutor.Int("@PlayerBMatchPoints", result.PlayerBMatchPoints),
                StoredProcedureExecutor.DateTimeParam("@RecordedAt", result.RecordedAt));
        }
    }

    public async Task<IReadOnlyList<Match>> GetMatchesForTournamentAsync(int tournamentId, CancellationToken ct = default) =>
        await _db.Matches
            .Include(m => m.Round)
            .Include(m => m.Result)
            .Include(m => m.PlayerA)
            .Include(m => m.PlayerB)
            .Where(m => m.Round.TournamentId == tournamentId)
            .ToListAsync(ct);

    public async Task<bool> HavePlayedBeforeAsync(int tournamentId, int playerAId, int playerBId, CancellationToken ct = default)
    {
        var (a, b) = playerAId < playerBId ? (playerAId, playerBId) : (playerBId, playerAId);
        return await _db.Matches
            .AnyAsync(m => m.Round.TournamentId == tournamentId &&
                           !m.IsBye &&
                           ((m.PlayerAId == a && m.PlayerBId == b) || (m.PlayerAId == b && m.PlayerBId == a)), ct);
    }
}