using MartianRobots;

namespace MartianRobots.Tests;

public class RunnerTests
{
    private const string SampleInput =
        "5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL\n";

    private const string SampleOutput = "1 1 E\n3 3 N LOST\n2 3 S\n";

    private static (int ExitCode, string Output, string Error) Run(string input)
    {
        var output = new StringWriter { NewLine = "\n" };
        var error = new StringWriter { NewLine = "\n" };
        var exitCode = Runner.Run(new StringReader(input), output, error);
        return (exitCode, output.ToString(), error.ToString());
    }

    // --- Happy path ---

    [Fact]
    public void SampleInput_MatchesExpectedOutput()
    {
        var (exitCode, output, error) = Run(SampleInput);

        Assert.Equal(0, exitCode);
        Assert.Equal(SampleOutput, output);
        Assert.Empty(error);
    }

    [Fact]
    public void SampleInput_WithoutTrailingNewline_Works() =>
        Assert.Equal(SampleOutput, Run(SampleInput.TrimEnd('\n')).Output);

    [Fact]
    public void SampleInput_WindowsLineEndings_Works() =>
        Assert.Equal(SampleOutput, Run(SampleInput.Replace("\n", "\r\n")).Output);

    [Fact]
    public void NoRobots_NoOutput_ExitsZero()
    {
        var (exitCode, output, _) = Run("5 3\n");
        Assert.Equal(0, exitCode);
        Assert.Empty(output);
    }

    [Fact]
    public void ScentIsSharedBetweenRobots() =>
        Assert.Equal("3 3 N LOST\n3 3 N\n", Run("5 3\n3 3 N\nF\n3 3 N\nF\n").Output);

    [Fact]
    public void RobotWithOnlyTurns_StaysPut() =>
        Assert.Equal("2 2 N\n", Run("5 5\n2 2 N\nLLLL\n").Output);

    [Fact]
    public void RobotFallsOffImmediately_IsLost() =>
        Assert.Equal("2 2 N LOST\n", Run("2 2\n2 2 N\nF\n").Output);

    [Fact]
    public void SinglePointGrid_Works() =>
        Assert.Equal("0 0 N LOST\n0 0 W\n", Run("0 0\n0 0 N\nF\n0 0 W\nF\n").Output);

    [Fact]
    public void LargestGrid_RobotWalksAcrossIt()
    {
        var (exitCode, output, _) = Run("50 50\n0 0 N\n" + new string('F', 50) + "\n");
        Assert.Equal(0, exitCode);
        Assert.Equal("0 50 N\n", output);
    }

    [Fact]
    public void LowercaseDirectionAndInstructions_AreAccepted() =>
        Assert.Equal("1 1 E\n", Run("5 3\n1 1 e\nrfrfrfrf\n").Output);

    [Fact]
    public void TabSeparatedValues_AreAccepted() =>
        Assert.Equal("1 2 N\n", Run("5\t3\n1\t1\tN\nF\n").Output);

    // --- Blank lines ---

    [Fact]
    public void BlankLinesBetweenRobots_AreSkipped() =>
        Assert.Equal(
            "1 1 E\n3 3 N LOST\n",
            Run("5 3\n\n1 1 E\nRFRFRFRF\n\n\n3 2 N\nFRRFLLFFRRFLL\n\n").Output);

    [Fact]
    public void BlankLinesBeforeGrid_AreSkipped() =>
        Assert.Equal("1 1 E\n", Run("\n\n5 3\n1 1 E\nRFRFRFRF\n").Output);

    [Fact]
    public void BlankInstructionLine_IsEmptyInstructions_NotDropped()
    {
        var (exitCode, output, _) = Run("5 3\n1 1 E\n\n2 2 N\nF\n");
        Assert.Equal(0, exitCode);
        Assert.Equal("1 1 E\n2 3 N\n", output);
    }

    // --- Bad input: abort, keep earlier results, error on stderr, exit code 1 ---

    [Fact]
    public void EmptyInput_Fails()
    {
        var (exitCode, output, error) = Run("");
        Assert.Equal(1, exitCode);
        Assert.Empty(output);
        Assert.Contains("empty", error);
    }

    [Fact]
    public void WhitespaceOnlyInput_Fails() =>
        Assert.Equal(1, Run("   \n \n").ExitCode);

    [Fact]
    public void BadGridLine_Fails_WithLineNumber()
    {
        var (exitCode, _, error) = Run("bad\n1 1 N\nF\n");
        Assert.Equal(1, exitCode);
        Assert.Contains("line 1", error);
    }

    [Fact]
    public void PositionLineWithoutInstructionLine_Fails()
    {
        var (exitCode, _, error) = Run("5 3\n1 1 N");
        Assert.Equal(1, exitCode);
        Assert.Contains("instruction", error);
    }

    [Fact]
    public void BadDirection_Fails_AndKeepsEarlierResults()
    {
        var (exitCode, output, error) = Run("5 3\n1 1 E\nRFRFRFRF\n1 1 X\nFFF\n3 2 N\nF\n");
        Assert.Equal(1, exitCode);
        Assert.Equal("1 1 E\n", output);
        Assert.Contains("line 4", error);
        Assert.Contains("X", error);
    }

    [Fact]
    public void NumericDirection_Fails()
    {
        var (exitCode, output, _) = Run("5 3\n1 1 5\nL\n");
        Assert.Equal(1, exitCode);
        Assert.Empty(output);
    }

    [Fact]
    public void StartOutsideGrid_Fails()
    {
        var (exitCode, output, error) = Run("5 3\n6 1 N\nF\n");
        Assert.Equal(1, exitCode);
        Assert.Empty(output);
        Assert.Contains("line 2", error);
    }

    [Fact]
    public void UnknownCommand_Fails_AndRobotDoesNotRun()
    {
        var (exitCode, output, error) = Run("5 3\n1 1 N\nFRX\n");
        Assert.Equal(1, exitCode);
        Assert.Empty(output);
        Assert.Contains("line 3", error);
        Assert.Contains("X", error);
    }

    [Fact]
    public void NonNumericRobotCoordinate_Fails() =>
        Assert.Equal(1, Run("5 3\nA 1 N\nF\n").ExitCode);

    [Fact]
    public void UnknownCommandAfterValidRobots_KeepsEarlierResults()
    {
        var (exitCode, output, _) = Run("5 3\n3 3 N\nF\n3 3 N\nFQ\n");
        Assert.Equal(1, exitCode);
        Assert.Equal("3 3 N LOST\n", output);
    }
}
