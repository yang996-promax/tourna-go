using FluentAssertions;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Services;

namespace TcgTournamentManager.Tests;

public class SwissPairingServiceTests
{
    [Fact]
    public void RoundOne_Random_PairsAllPlayers()
    {
        var service = CreateService();
        var players = CreatePlayers(8);
        var context = new SwissPairingContext
        {
            RoundNumber = 1,
            FirstRoundMode = FirstRoundPairingMode.Random,
            Players = players
        };

        var pairings = service.ComputePairings(context);

        pairings.Should().HaveCount(4);
        pairings.Should().OnlyContain(p => !p.IsBye);
        GetPairedPlayerIds(pairings).Should().HaveCount(8);
    }

    [Fact]
    public void RoundOne_OddPlayers_AssignsBye()
    {
        var service = CreateService();
        var players = CreatePlayers(7);
        var context = new SwissPairingContext
        {
            RoundNumber = 1,
            FirstRoundMode = FirstRoundPairingMode.Seeded,
            Players = players
        };

        var pairings = service.ComputePairings(context);

        pairings.Should().HaveCount(4);
        pairings.Count(p => p.IsBye).Should().Be(1);
        GetPairedPlayerIds(pairings).Should().HaveCount(7);
    }

    [Fact]
    public void SubsequentRounds_PairsByMatchPoints()
    {
        var service = CreateService();
        var players = new List<SwissPlayerState>
        {
            new() { TournamentPlayerId = 1, PlayerNumber = 1, MatchPoints = 6 },
            new() { TournamentPlayerId = 2, PlayerNumber = 2, MatchPoints = 6 },
            new() { TournamentPlayerId = 3, PlayerNumber = 3, MatchPoints = 3 },
            new() { TournamentPlayerId = 4, PlayerNumber = 4, MatchPoints = 3 }
        };

        var context = new SwissPairingContext
        {
            RoundNumber = 2,
            Players = players,
            PreviousPairings = new HashSet<(int, int)>(),
            PlayersWithBye = new HashSet<int>()
        };

        var pairings = service.ComputePairings(context);

        pairings.Should().HaveCount(2);
        pairings.Should().Contain(p =>
            (p.PlayerAId == 1 && p.PlayerBId == 2) || (p.PlayerAId == 2 && p.PlayerBId == 1));
        pairings.Should().Contain(p =>
            (p.PlayerAId == 3 && p.PlayerBId == 4) || (p.PlayerAId == 4 && p.PlayerBId == 3));
    }

    [Fact]
    public void SubsequentRounds_AvoidsRematches()
    {
        var service = CreateService();
        var players = new List<SwissPlayerState>
        {
            new() { TournamentPlayerId = 1, PlayerNumber = 1, MatchPoints = 3 },
            new() { TournamentPlayerId = 2, PlayerNumber = 2, MatchPoints = 3 },
            new() { TournamentPlayerId = 3, PlayerNumber = 3, MatchPoints = 3 }
        };

        var context = new SwissPairingContext
        {
            RoundNumber = 2,
            Players = players,
            PreviousPairings = new HashSet<(int, int)> { (1, 2) },
            PlayersWithBye = new HashSet<int>()
        };

        var pairings = service.ComputePairings(context);

        pairings.Should().HaveCount(2);
        var nonBye = pairings.Where(p => !p.IsBye).ToList();
        nonBye.Should().NotContain(p =>
            (p.PlayerAId == 1 && p.PlayerBId == 2) || (p.PlayerAId == 2 && p.PlayerBId == 1));
    }

    [Fact]
    public void SubsequentRounds_OnlyOneByePerPlayer()
    {
        var service = CreateService();
        var players = new List<SwissPlayerState>
        {
            new() { TournamentPlayerId = 1, PlayerNumber = 1, MatchPoints = 0 },
            new() { TournamentPlayerId = 2, PlayerNumber = 2, MatchPoints = 0 },
            new() { TournamentPlayerId = 3, PlayerNumber = 3, MatchPoints = 0 }
        };

        var context = new SwissPairingContext
        {
            RoundNumber = 3,
            Players = players,
            PreviousPairings = new HashSet<(int, int)>(),
            PlayersWithBye = new HashSet<int> { 1 }
        };

        var pairings = service.ComputePairings(context);

        pairings.Count(p => p.IsBye).Should().Be(1);
        pairings.Single(p => p.IsBye).PlayerAId.Should().NotBe(1);
    }

    private static SwissPairingService CreateService() => new(
        null!, null!, null!, null!, null!, null!);

    private static List<SwissPlayerState> CreatePlayers(int count) =>
        Enumerable.Range(1, count).Select(i => new SwissPlayerState
        {
            TournamentPlayerId = i,
            PlayerNumber = i,
            MatchPoints = 0
        }).ToList();

    private static HashSet<int> GetPairedPlayerIds(IReadOnlyList<PairingProposal> pairings)
    {
        var ids = new HashSet<int>();
        foreach (var p in pairings)
        {
            if (p.PlayerAId.HasValue) ids.Add(p.PlayerAId.Value);
            if (p.PlayerBId.HasValue) ids.Add(p.PlayerBId.Value);
        }
        return ids;
    }
}