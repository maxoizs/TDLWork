using MartianRobots;

namespace MartianRobots.Tests;

public class InputParserTests
{
    private static readonly Grid SampleGrid = new(5, 3);

    // --- ParseGrid: success ---

    [Fact]
    public void ParseGrid_ReturnsCorrectBounds()
    {
        var result = InputParser.ParseGrid("5 3");
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsOutOfBounds(5, 3));
        Assert.True(result.Value.IsOutOfBounds(6, 3));
        Assert.True(result.Value.IsOutOfBounds(5, 4));
    }

    [Fact]
    public void ParseGrid_MinimalGrid()
    {
        var result = InputParser.ParseGrid("0 0");
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsOutOfBounds(0, 0));
        Assert.True(result.Value.IsOutOfBounds(1, 0));
    }

    [Fact]
    public void ParseGrid_MaxCoordinate_Succeeds()
    {
        var result = InputParser.ParseGrid("50 50");
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsOutOfBounds(50, 50));
    }

    [Fact]
    public void ParseGrid_TabSeparated_Succeeds()
    {
        var result = InputParser.ParseGrid("5\t3");
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsOutOfBounds(5, 3));
    }

    // --- ParseGrid: failure ---

    [Theory]
    [InlineData("X 3")]
    [InlineData("5 Y")]
    [InlineData("5")]
    [InlineData("")]
    [InlineData("-1 3")]
    [InlineData("5 -1")]
    [InlineData("51 3")]
    [InlineData("5 51")]
    [InlineData("5 3 7")]
    [InlineData("5.5 3")]
    [InlineData("99999999999 3")]
    public void ParseGrid_Invalid_Fails(string line)
    {
        var result = InputParser.ParseGrid(line);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    // --- ParseRobotState: success ---

    [Theory]
    [InlineData("1 1 E", 1, 1, Direction.E)]
    [InlineData("0 0 N", 0, 0, Direction.N)]
    [InlineData("5 3 S", 5, 3, Direction.S)]
    [InlineData("3 2 W", 3, 2, Direction.W)]
    [InlineData("0 3 N", 0, 3, Direction.N)]
    [InlineData("5 0 N", 5, 0, Direction.N)]
    public void ParseRobotState_ReturnsCorrectState(string line, int x, int y, Direction facing)
    {
        var result = InputParser.ParseRobotState(line, SampleGrid);
        Assert.True(result.IsSuccess);
        Assert.Equal(new RobotState(x, y, facing), result.Value);
    }

    [Theory]
    [InlineData("1 1 n", Direction.N)]
    [InlineData("1 1 e", Direction.E)]
    [InlineData("1 1 s", Direction.S)]
    [InlineData("1 1 w", Direction.W)]
    public void ParseRobotState_LowercaseDirection_ParsedCorrectly(string line, Direction expected)
    {
        var result = InputParser.ParseRobotState(line, SampleGrid);
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Facing);
    }

    [Fact]
    public void ParseRobotState_WithTrailingWhitespace_ParsedCorrectly()
    {
        var result = InputParser.ParseRobotState("1 1 E   ", SampleGrid);
        Assert.True(result.IsSuccess);
        Assert.Equal(new RobotState(1, 1, Direction.E), result.Value);
    }

    [Fact]
    public void ParseRobotState_TabSeparated_ParsedCorrectly()
    {
        var result = InputParser.ParseRobotState("1\t1\tE", SampleGrid);
        Assert.True(result.IsSuccess);
        Assert.Equal(new RobotState(1, 1, Direction.E), result.Value);
    }

    // --- ParseRobotState: failure ---

    [Fact]
    public void ParseRobotState_InvalidDirection_Fails()
    {
        var result = InputParser.ParseRobotState("1 1 X", SampleGrid);
        Assert.False(result.IsSuccess);
        Assert.Contains("X", result.Error);
    }

    [Theory]
    [InlineData("1 1 0")]
    [InlineData("1 1 1")]
    [InlineData("1 1 5")]
    [InlineData("1 1 -1")]
    [InlineData("1 1 NE")]
    [InlineData("1 1 N,E")]
    public void ParseRobotState_NumericOrMultiLetterDirection_Fails(string line)
    {
        var result = InputParser.ParseRobotState(line, SampleGrid);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Theory]
    [InlineData("A 1 N")]
    [InlineData("1 B N")]
    [InlineData("1 1")]
    [InlineData("1 1 E junk")]
    [InlineData("   ")]
    [InlineData("")]
    public void ParseRobotState_MalformedLine_Fails(string line)
    {
        var result = InputParser.ParseRobotState(line, SampleGrid);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Theory]
    [InlineData("6 1 N")]
    [InlineData("1 4 N")]
    [InlineData("-1 1 N")]
    [InlineData("1 -1 N")]
    [InlineData("51 1 N")]
    [InlineData("99999999999 1 N")]
    public void ParseRobotState_StartOutsideGrid_Fails(string line)
    {
        var result = InputParser.ParseRobotState(line, SampleGrid);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    // --- ParseInstructions ---

    [Theory]
    [InlineData("RFRFRFRF")]
    [InlineData("L")]
    [InlineData("")]
    public void ParseInstructions_ValidCommands_Succeeds(string line)
    {
        var result = InputParser.ParseInstructions(line);
        Assert.True(result.IsSuccess);
        Assert.Equal(line, result.Value);
    }

    [Fact]
    public void ParseInstructions_Lowercase_NormalisedToUppercase()
    {
        var result = InputParser.ParseInstructions("lrf");
        Assert.True(result.IsSuccess);
        Assert.Equal("LRF", result.Value);
    }

    [Fact]
    public void ParseInstructions_TrimsSurroundingWhitespace()
    {
        var result = InputParser.ParseInstructions("  FRF \r");
        Assert.True(result.IsSuccess);
        Assert.Equal("FRF", result.Value);
    }

    [Theory]
    [InlineData("FRX")]
    [InlineData("F R")]
    [InlineData("F1")]
    [InlineData("F\0")]
    [InlineData("FF,")]
    public void ParseInstructions_UnknownCharacter_Fails(string line)
    {
        var result = InputParser.ParseInstructions(line);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseInstructions_Over100Characters_StillAccepted()
    {
        var result = InputParser.ParseInstructions(new string('F', 500));
        Assert.True(result.IsSuccess);
    }
}
