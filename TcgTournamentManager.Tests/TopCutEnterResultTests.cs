using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Infrastructure.Data;
using TcgTournamentManager.Infrastructure.Repositories;
using TcgTournamentManager.Infrastructure.Services;

namespace TcgTournamentManager.Tests;

public class TopCutEnterResultTests : IDisposable
{
    private readonly TournamentDbContext _db;
    private readonly TopCutService _service;

    public TopCutEnterResultTests()
    {
        var options = new DbContextOptionsBuilder<TournamentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new TournamentDbContext(options);
        var matchRepo = TestRepositories.Match(_db);
        _service = new TopCutService(
            TestRepositories.Tournament(_db),
            TestRepositories.TopCut(_db),
            new StandingsService(
                TestRepositories.Tournament(_db),
                TestRepositories.Player(_db),
                matchRepo,
                TestRepositories.Standing(_db)),
            matchRepo,
            TestRepositories.AuditLog(_db));
    }

    [Fact]
    public async Task EnterTopCutResult_AllowsMultipleMatchesInSameRound()
    {
        var tournamentId = await SeedTop8ReadyForTopCutAsync();
        await _service.GenerateTopCutAsync(tournamentId);

        var tree = await _service.GetBracketTreeAsync(tournamentId);
        var quarterFinals = tree.Brackets
            .Where(b => b.Round == TopCutRound.QuarterFinal)
            .OrderBy(b => b.MatchPosition)
            .ToList();

        quarterFinals.Should().HaveCount(4);
        quarterFinals.Should().OnlyContain(b => b.IsPlayable);

        await _service.EnterTopCutResultAsync(quarterFinals[0].Id, quarterFinals[0].PlayerAId!.Value);
        await _service.EnterTopCutResultAsync(quarterFinals[1].Id, quarterFinals[1].PlayerAId!.Value);

        var updated = await _service.GetBracketTreeAsync(tournamentId);
        var qf = updated.Brackets.Where(b => b.Round == TopCutRound.QuarterFinal).ToList();

        qf.Count(b => b.IsComplete).Should().Be(2);
        qf.Where(b => !b.IsComplete).Should().OnlyContain(b => b.IsPlayable);

        var topCutRounds = await _db.Rounds
            .Where(r => r.TournamentId == tournamentId && r.RoundType == RoundType.TopCut)
            .ToListAsync();

        topCutRounds.Count(r => r.RoundNumber == TopCutService.GetTopCutRoundNumber(TopCutRound.QuarterFinal, 8)).Should().Be(1);

        var qfRound = topCutRounds.Single(r => r.RoundNumber == TopCutService.GetTopCutRoundNumber(TopCutRound.QuarterFinal, 8));
        qfRound.IsComplete.Should().BeFalse();
    }

    [Fact]
    public async Task EnterTopCutResult_MarksMstRoundCompleteWhenAllBracketMatchesFinished()
    {
        var tournamentId = await SeedTop8ReadyForTopCutAsync();
        await _service.GenerateTopCutAsync(tournamentId);

        var tree = await _service.GetBracketTreeAsync(tournamentId);
        var quarterFinals = tree.Brackets
            .Where(b => b.Round == TopCutRound.QuarterFinal)
            .OrderBy(b => b.MatchPosition)
            .ToList();

        foreach (var bracket in quarterFinals)
            await _service.EnterTopCutResultAsync(bracket.Id, bracket.PlayerAId!.Value);

        var qfRoundNumber = TopCutService.GetTopCutRoundNumber(TopCutRound.QuarterFinal, 8);
        var qfRound = await _db.Rounds.SingleAsync(r =>
            r.TournamentId == tournamentId &&
            r.RoundType == RoundType.TopCut &&
            r.RoundNumber == qfRoundNumber);

        qfRound.IsComplete.Should().BeTrue();
        qfRound.CompletedAt.Should().NotBeNull();
    }

    private async Task<int> SeedTop8ReadyForTopCutAsync()
    {
        var tournament = new Tournament
        {
            Name = "Top Cut Test",
            GameTitle = "Pokémon TCG",
            EventDate = DateTime.UtcNow,
            Organizer = "Test",
            Venue = "Test",
            TotalSwissRounds = 1,
            TopCutSize = TopCutSize.Top8,
            Status = TournamentStatus.SwissComplete,
            CurrentRound = 1
        };
        _db.Tournaments.Add(tournament);
        await _db.SaveChangesAsync();

        var tpIds = new List<int>();
        for (var i = 0; i < 8; i++)
        {
            var player = new Player { ExternalPlayerId = $"TC{i}", Name = $"Player {i + 1}" };
            _db.Players.Add(player);
            await _db.SaveChangesAsync();

            var tp = new TournamentPlayer
            {
                TournamentId = tournament.Id,
                PlayerId = player.Id,
                PlayerNumber = i + 1
            };
            _db.TournamentPlayers.Add(tp);
            await _db.SaveChangesAsync();
            tpIds.Add(tp.Id);
        }

        var round = new Round
        {
            TournamentId = tournament.Id,
            RoundNumber = 1,
            RoundType = RoundType.Swiss,
            IsComplete = true,
            CompletedAt = DateTime.UtcNow
        };
        _db.Rounds.Add(round);
        await _db.SaveChangesAsync();

        for (var i = 0; i < 4; i++)
        {
            var match = new Match
            {
                RoundId = round.Id,
                TableNumber = i + 1,
                PlayerAId = tpIds[i * 2],
                PlayerBId = tpIds[i * 2 + 1],
                IsComplete = true,
                WinnerId = tpIds[i * 2]
            };
            _db.Matches.Add(match);
            await _db.SaveChangesAsync();

            _db.MatchResults.Add(new MatchResult
            {
                MatchId = match.Id,
                ResultType = MatchResultType.Win2_0,
                PlayerAGameWins = 2,
                PlayerBGameWins = 0,
                PlayerAMatchPoints = 3,
                PlayerBMatchPoints = 0
            });
            await _db.SaveChangesAsync();
        }

        return tournament.Id;
    }

    public void Dispose() => _db.Dispose();
}