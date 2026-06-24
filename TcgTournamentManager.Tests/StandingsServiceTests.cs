using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Infrastructure.Data;
using TcgTournamentManager.Infrastructure.Repositories;
using TcgTournamentManager.Infrastructure.Services;

namespace TcgTournamentManager.Tests;

public class StandingsServiceTests : IDisposable
{
    private readonly TournamentDbContext _db;
    private readonly StandingsService _service;

    public StandingsServiceTests()
    {
        var options = new DbContextOptionsBuilder<TournamentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new TournamentDbContext(options);
        _service = new StandingsService(
            TestRepositories.Tournament(_db),
            TestRepositories.Player(_db),
            TestRepositories.Match(_db),
            TestRepositories.Standing(_db));
    }

    [Fact]
    public async Task CalculateStandings_SortsByMatchPointsAndTiebreakers()
    {
        var tournament = await SeedTournamentWithResults();

        var standings = await _service.CalculateStandingsAsync(tournament.Id);

        standings.Should().HaveCount(4);
        standings[0].MatchPoints.Should().BeGreaterThanOrEqualTo(standings[1].MatchPoints);
        standings[0].PlayerName.Should().Be("Alice");
    }

    [Fact]
    public async Task CalculateStandings_SwissPhase_ExcludesTopCutMatches()
    {
        var tournament = await SeedTournamentWithTopCutResult();

        var swiss = await _service.CalculateStandingsAsync(tournament.Id, phase: StandingPhase.Swiss);
        var overall = await _service.CalculateStandingsAsync(tournament.Id, phase: StandingPhase.Overall);

        swiss.First(s => s.PlayerName == "Alice").MatchPoints.Should().Be(3);
        swiss.First(s => s.PlayerName == "Bob").MatchPoints.Should().Be(0);
        overall.First(s => s.PlayerName == "Alice").MatchPoints.Should().Be(6);
        overall.First(s => s.PlayerName == "Bob").MatchPoints.Should().Be(0);
    }

    private async Task<Tournament> SeedTournamentWithResults()
    {
        var tournament = new Tournament
        {
            Name = "Test",
            GameTitle = "Pokémon TCG",
            EventDate = DateTime.UtcNow,
            Organizer = "Test",
            Venue = "Test",
            TotalSwissRounds = 3,
            Status = TournamentStatus.InProgress
        };
        _db.Tournaments.Add(tournament);
        await _db.SaveChangesAsync();

        var players = new[] { "Alice", "Bob", "Carol", "Dave" };
        var tpIds = new List<int>();
        for (var i = 0; i < players.Length; i++)
        {
            var p = new Player { ExternalPlayerId = $"P{i}", Name = players[i] };
            _db.Players.Add(p);
            await _db.SaveChangesAsync();
            var tp = new TournamentPlayer { TournamentId = tournament.Id, PlayerId = p.Id, PlayerNumber = i + 1 };
            _db.TournamentPlayers.Add(tp);
            await _db.SaveChangesAsync();
            tpIds.Add(tp.Id);
        }

        var round = new Round { TournamentId = tournament.Id, RoundNumber = 1, IsComplete = true };
        _db.Rounds.Add(round);
        await _db.SaveChangesAsync();

        var match1 = new Match { RoundId = round.Id, TableNumber = 1, PlayerAId = tpIds[0], PlayerBId = tpIds[1], IsComplete = true, WinnerId = tpIds[0] };
        var match2 = new Match { RoundId = round.Id, TableNumber = 2, PlayerAId = tpIds[2], PlayerBId = tpIds[3], IsComplete = true, WinnerId = tpIds[2] };
        _db.Matches.AddRange(match1, match2);
        await _db.SaveChangesAsync();

        _db.MatchResults.AddRange(
            new MatchResult { MatchId = match1.Id, ResultType = MatchResultType.Win2_0, PlayerAGameWins = 2, PlayerBGameWins = 0, PlayerAMatchPoints = 3, PlayerBMatchPoints = 0 },
            new MatchResult { MatchId = match2.Id, ResultType = MatchResultType.Win2_1, PlayerAGameWins = 2, PlayerBGameWins = 1, PlayerAMatchPoints = 3, PlayerBMatchPoints = 0 });
        await _db.SaveChangesAsync();

        return tournament;
    }

    private async Task<Tournament> SeedTournamentWithTopCutResult()
    {
        var tournament = await SeedTournamentWithResults();

        var tpAlice = await _db.TournamentPlayers.Include(tp => tp.Player)
            .FirstAsync(tp => tp.Player.Name == "Alice");
        var tpBob = await _db.TournamentPlayers.Include(tp => tp.Player)
            .FirstAsync(tp => tp.Player.Name == "Bob");

        var topCutRound = new Round
        {
            TournamentId = tournament.Id,
            RoundNumber = 1,
            RoundType = RoundType.TopCut,
            IsComplete = true
        };
        _db.Rounds.Add(topCutRound);
        await _db.SaveChangesAsync();

        var topCutMatch = new Match
        {
            RoundId = topCutRound.Id,
            TableNumber = 1,
            PlayerAId = tpAlice.Id,
            PlayerBId = tpBob.Id,
            IsComplete = true,
            WinnerId = tpAlice.Id
        };
        _db.Matches.Add(topCutMatch);
        await _db.SaveChangesAsync();

        _db.MatchResults.Add(new MatchResult
        {
            MatchId = topCutMatch.Id,
            ResultType = MatchResultType.Win2_0,
            PlayerAGameWins = 2,
            PlayerBGameWins = 0,
            PlayerAMatchPoints = 3,
            PlayerBMatchPoints = 0
        });
        await _db.SaveChangesAsync();

        return tournament;
    }

    public void Dispose() => _db.Dispose();
}