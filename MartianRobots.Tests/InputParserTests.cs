using MartianRobots;

namespace MartianRobots.Tests;

public class InputParserTests
{
    // --- Grid line ---

    [Fact]
    public void ParseGrid_ReturnsCorrectBounds()
    {
        var grid = InputParser.ParseGrid("5 3");
        Assert.False(grid.IsOutOfBounds(5, 3));
        Assert.True(grid.IsOutOfBounds(6, 3));
        Assert.True(grid.IsOutOfBounds(5, 4));
    }

    [Fact]
    public void ParseGrid_MinimalGrid()
    {
        var grid = InputParser.ParseGrid("0 0");
        Assert.False(grid.IsOutOfBounds(0, 0));
        Assert.True(grid.IsOutOfBounds(1, 0));
    }

    // --- Robot position line ---

    [Theory]
    [InlineData("1 1 E", 1, 1, Direction.E)]
    [InlineData("0 0 N", 0, 0, Direction.N)]
    [InlineData("5 3 S", 5, 3, Direction.S)]
    [InlineData("3 2 W", 3, 2, Direction.W)]
    public void ParseRobotState_ReturnsCorrectState(string line, int x, int y, Direction facing)
    {
        var state = InputParser.ParseRobotState(line);
        Assert.Equal(new RobotState(x, y, facing), state);
    }

    // --- Full input ---

    [Fact]
    public void ParseInput_ReturnsCorrectNumberOfRobots()
    {
        var (_, robots) = InputParser.Parse(SampleInput);
        Assert.Equal(3, robots.Count);
    }

    [Fact]
    public void ParseInput_FirstRobotCorrect()
    {
        var (_, robots) = InputParser.Parse(SampleInput);
        Assert.Equal(new RobotState(1, 1, Direction.E), robots[0].State);
        Assert.Equal("RFRFRFRF", robots[0].Instructions);
    }

    [Fact]
    public void ParseInput_SecondRobotCorrect()
    {
        var (_, robots) = InputParser.Parse(SampleInput);
        Assert.Equal(new RobotState(3, 2, Direction.N), robots[1].State);
        Assert.Equal("FRRFLLFFRRFLL", robots[1].Instructions);
    }

    [Fact]
    public void ParseInput_ThirdRobotCorrect()
    {
        var (_, robots) = InputParser.Parse(SampleInput);
        Assert.Equal(new RobotState(0, 3, Direction.W), robots[2].State);
        Assert.Equal("LLFFFLFLFL", robots[2].Instructions);
    }

    [Fact]
    public void ParseInput_GridBoundsCorrect()
    {
        var (grid, _) = InputParser.Parse(SampleInput);
        Assert.False(grid.IsOutOfBounds(5, 3));
        Assert.True(grid.IsOutOfBounds(6, 3));
    }

    [Fact]
    public void ParseInput_SingleRobot()
    {
        var (_, robots) = InputParser.Parse("2 2\n1 1 N\nF");
        Assert.Single(robots);
        Assert.Equal("F", robots[0].Instructions);
    }

    [Fact]
    public void ParseInput_WindowsLineEndings_ParsedCorrectly()
    {
        var input = "5 3\r\n1 1 E\r\nRFRFRFRF\r\n3 2 N\r\nFRRFLLFFRRFLL";
        var (_, robots) = InputParser.Parse(input);
        Assert.Equal(2, robots.Count);
        Assert.Equal(new RobotState(1, 1, Direction.E), robots[0].State);
        Assert.Equal("RFRFRFRF", robots[0].Instructions);
    }

    [Fact]
    public void ParseRobotState_WithTrailingWhitespace_ParsedCorrectly()
    {
        var state = InputParser.ParseRobotState("1 1 E   ");
        Assert.Equal(new RobotState(1, 1, Direction.E), state);
    }

    private const string SampleInput =
        "5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL";
}
