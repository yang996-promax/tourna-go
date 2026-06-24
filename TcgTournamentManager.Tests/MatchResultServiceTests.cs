using FluentAssertions;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Infrastructure.Services;

namespace TcgTournamentManager.Tests;

public class MatchResultServiceTests
{
    private readonly MatchResultService _service = new(null!, null!, null!, null!, null!);

    [Theory]
    [InlineData(MatchResultType.Win2_0, 2, 0, 3, 0)]
    [InlineData(MatchResultType.Win2_1, 2, 1, 3, 0)]
    [InlineData(MatchResultType.Loss1_2, 1, 2, 0, 3)]
    [InlineData(MatchResultType.Loss0_2, 0, 2, 0, 3)]
    [InlineData(MatchResultType.Draw, 1, 1, 1, 1)]
    [InlineData(MatchResultType.ByeWin, 2, 0, 3, 0)]
    public void CalculatePoints_BO3_ReturnsCorrectValues(
        MatchResultType resultType, int aGw, int bGw, int aMp, int bMp)
    {
        var result = _service.CalculatePoints(resultType, false, MatchFormat.BO3);

        result.playerAGW.Should().Be(aGw);
        result.playerBGW.Should().Be(bGw);
        result.playerAMP.Should().Be(aMp);
        result.playerBMP.Should().Be(bMp);
    }

    [Theory]
    [InlineData(MatchResultType.Win2_0, 1, 0, 3, 0)]
    [InlineData(MatchResultType.Loss0_2, 0, 1, 0, 3)]
    [InlineData(MatchResultType.Draw, 0, 0, 1, 1)]
    public void CalculatePoints_BO1_ReturnsCorrectValues(
        MatchResultType resultType, int aGw, int bGw, int aMp, int bMp)
    {
        var result = _service.CalculatePoints(resultType, false, MatchFormat.BO1);

        result.playerAGW.Should().Be(aGw);
        result.playerBGW.Should().Be(bGw);
        result.playerAMP.Should().Be(aMp);
        result.playerBMP.Should().Be(bMp);
    }

    [Fact]
    public void CalculatePoints_ByeMatch_BO3_ReturnsThreePoints()
    {
        var result = _service.CalculatePoints(MatchResultType.ByeWin, true, MatchFormat.BO3);

        result.playerAMP.Should().Be(3);
        result.playerAGW.Should().Be(2);
        result.playerBMP.Should().Be(0);
    }

    [Fact]
    public void CalculatePoints_ByeMatch_BO1_ReturnsThreePoints()
    {
        var result = _service.CalculatePoints(MatchResultType.ByeWin, true, MatchFormat.BO1);

        result.playerAMP.Should().Be(3);
        result.playerAGW.Should().Be(1);
        result.playerBMP.Should().Be(0);
    }
}