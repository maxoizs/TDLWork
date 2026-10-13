using MartianRobots;

namespace MartianRobots.Tests;

public class InputParserTests
{
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

    // --- ParseGrid: failure ---

    [Fact]
    public void ParseGrid_NonNumericX_Fails()
    {
        var result = InputParser.ParseGrid("X 3");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseGrid_NonNumericY_Fails()
    {
        var result = InputParser.ParseGrid("5 Y");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseGrid_MissingParts_Fails()
    {
        var result = InputParser.ParseGrid("5");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseGrid_EmptyString_Fails()
    {
        var result = InputParser.ParseGrid("");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseGrid_NegativeCoordinateInvalid_Fails()
    {
        var result = InputParser.ParseGrid("-1 3");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseGrid_CoordinateExceedsMax_Fails()
    {
        var result = InputParser.ParseGrid("51 3");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    // --- ParseRobotState: success ---

    [Theory]
    [InlineData("1 1 E", 1, 1, Direction.E)]
    [InlineData("0 0 N", 0, 0, Direction.N)]
    [InlineData("5 3 S", 5, 3, Direction.S)]
    [InlineData("3 2 W", 3, 2, Direction.W)]
    public void ParseRobotState_ReturnsCorrectState(string line, int x, int y, Direction facing)
    {
        var result = InputParser.ParseRobotState(line);
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
        var result = InputParser.ParseRobotState(line);
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Value.Facing);
    }

    [Fact]
    public void ParseRobotState_WithTrailingWhitespace_ParsedCorrectly()
    {
        var result = InputParser.ParseRobotState("1 1 E   ");
        Assert.True(result.IsSuccess);
        Assert.Equal(new RobotState(1, 1, Direction.E), result.Value);
    }

    // --- ParseRobotState: failure ---

    [Fact]
    public void ParseRobotState_InvalidDirection_Fails()
    {
        var result = InputParser.ParseRobotState("1 1 X");
        Assert.False(result.IsSuccess);
        Assert.Contains("X", result.Error);
    }

    [Fact]
    public void ParseRobotState_NonNumericX_Fails()
    {
        var result = InputParser.ParseRobotState("A 1 N");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseRobotState_NonNumericY_Fails()
    {
        var result = InputParser.ParseRobotState("1 B N");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseRobotState_MissingParts_Fails()
    {
        var result = InputParser.ParseRobotState("1 1");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParseRobotState_EmptyString_Fails()
    {
        var result = InputParser.ParseRobotState("   ");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    // --- Parse: success ---

    [Fact]
    public void ParseInput_ReturnsCorrectNumberOfRobots()
    {
        var result = InputParser.Parse(SampleInput);
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Robots.Count);
    }

    [Fact]
    public void ParseInput_FirstRobotCorrect()
    {
        var result = InputParser.Parse(SampleInput);
        Assert.True(result.IsSuccess);
        Assert.Equal(new RobotState(1, 1, Direction.E), result.Value.Robots[0].State);
        Assert.Equal("RFRFRFRF", result.Value.Robots[0].Instructions);
    }

    [Fact]
    public void ParseInput_SecondRobotCorrect()
    {
        var result = InputParser.Parse(SampleInput);
        Assert.True(result.IsSuccess);
        Assert.Equal(new RobotState(3, 2, Direction.N), result.Value.Robots[1].State);
        Assert.Equal("FRRFLLFFRRFLL", result.Value.Robots[1].Instructions);
    }

    [Fact]
    public void ParseInput_ThirdRobotCorrect()
    {
        var result = InputParser.Parse(SampleInput);
        Assert.True(result.IsSuccess);
        Assert.Equal(new RobotState(0, 3, Direction.W), result.Value.Robots[2].State);
        Assert.Equal("LLFFFLFLFL", result.Value.Robots[2].Instructions);
    }

    [Fact]
    public void ParseInput_GridBoundsCorrect()
    {
        var result = InputParser.Parse(SampleInput);
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.Grid.IsOutOfBounds(5, 3));
        Assert.True(result.Value.Grid.IsOutOfBounds(6, 3));
    }

    [Fact]
    public void ParseInput_SingleRobot()
    {
        var result = InputParser.Parse("2 2\n1 1 N\nF");
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Robots);
        Assert.Equal("F", result.Value.Robots[0].Instructions);
    }

    [Fact]
    public void ParseInput_NoRobots_ReturnsEmptyList()
    {
        var result = InputParser.Parse("5 3");
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Robots);
    }

    [Fact]
    public void ParseInput_WindowsLineEndings_ParsedCorrectly()
    {
        var result = InputParser.Parse("5 3\r\n1 1 E\r\nRFRFRFRF\r\n3 2 N\r\nFRRFLLFFRRFLL");
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Robots.Count);
        Assert.Equal("RFRFRFRF", result.Value.Robots[0].Instructions);
    }

    // --- Parse: failure ---

    [Fact]
    public void Parse_EmptyInput_Fails()
    {
        var result = InputParser.Parse("");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void Parse_WhitespaceOnlyInput_Fails()
    {
        var result = InputParser.Parse("   \n  ");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void Parse_BadGridLine_Fails()
    {
        var result = InputParser.Parse("bad\n1 1 N\nF");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void Parse_PositionLineWithoutInstructionLine_Fails()
    {
        var result = InputParser.Parse("5 3\n1 1 N");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void Parse_InvalidRobotDirection_Fails()
    {
        var result = InputParser.Parse("5 3\n1 1 X\nFFF");
        Assert.False(result.IsSuccess);
        Assert.Contains("X", result.Error);
    }

    [Fact]
    public void Parse_NonNumericRobotCoord_Fails()
    {
        var result = InputParser.Parse("5 3\nA 1 N\nFFF");
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    private const string SampleInput =
        "5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL";
}
