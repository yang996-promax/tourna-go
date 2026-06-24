using FluentAssertions;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Infrastructure.Services;

namespace TcgTournamentManager.Tests;

public class TopCutServiceTests
{
    [Theory]
    [InlineData(4, 2, 1)]
    [InlineData(8, 4, 2, 1)]
    [InlineData(16, 8, 4, 2, 1)]
    public void BuildBracketTree_CreatesCorrectMatchCounts(int cutSize, params int[] expectedCounts)
    {
        var playerIds = Enumerable.Range(1, cutSize).ToList();
        var brackets = TopCutService.BuildBracketTree(1, cutSize, playerIds);

        var rounds = expectedCounts.Length switch
        {
            3 => new[] { TopCutRound.QuarterFinal, TopCutRound.SemiFinal, TopCutRound.Final },
            4 => new[] { TopCutRound.RoundOf16, TopCutRound.QuarterFinal, TopCutRound.SemiFinal, TopCutRound.Final },
            _ => new[] { TopCutRound.SemiFinal, TopCutRound.Final }
        };

        for (var i = 0; i < rounds.Length; i++)
            brackets.Count(b => b.Round == rounds[i]).Should().Be(expectedCounts[i]);
    }

    [Fact]
    public void BuildBracketTree_Top8_SeedsStandardMatchups()
    {
        var playerIds = Enumerable.Range(100, 8).ToList();
        var brackets = TopCutService.BuildBracketTree(1, 8, playerIds);
        var qf = brackets.Where(b => b.Round == TopCutRound.QuarterFinal).OrderBy(b => b.MatchPosition).ToList();

        qf[0].PlayerAId.Should().Be(100);
        qf[0].PlayerBId.Should().Be(107);
        qf[1].PlayerAId.Should().Be(103);
        qf[1].PlayerBId.Should().Be(104);
    }

    [Fact]
    public void BuildBracketTree_LinksNextBracketIds()
    {
        var brackets = TopCutService.BuildBracketTree(1, 4, [1, 2, 3, 4]);
        var semis = brackets.Where(b => b.Round == TopCutRound.SemiFinal).ToList();
        var final = brackets.Single(b => b.Round == TopCutRound.Final);

        semis.Should().AllSatisfy(s => s.NextBracket.Should().Be(final));
    }
}