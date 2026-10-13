using MartianRobots;

namespace MartianRobots.Tests;

public class IntegrationTests
{
    private static List<string> Run(string input)
    {
        var (grid, robots) = InputParser.Parse(input);
        var sim = new Simulation();
        var results = new List<string>();

        foreach (var robot in robots)
        {
            var final = sim.Execute(robot.State, robot.Instructions, grid);
            var lost = final.IsLost ? " LOST" : "";
            results.Add($"{final.X} {final.Y} {final.Facing}{lost}");
        }

        return results;
    }

    [Fact]
    public void SampleInput_MatchesExpectedOutput()
    {
        const string input = "5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL";

        var output = Run(input);

        Assert.Equal("1 1 E", output[0]);
        Assert.Equal("3 3 N LOST", output[1]);
        Assert.Equal("2 3 S", output[2]);
    }

    [Fact]
    public void SingleRobot_NoOpInstructions_ReturnsStartPosition()
    {
        var output = Run("5 5\n2 2 N\nLLLL");
        Assert.Equal("2 2 N", output[0]);
    }

    [Fact]
    public void SingleRobot_FallsOff_Immediately()
    {
        var output = Run("2 2\n2 2 N\nF");
        Assert.Equal("2 2 N LOST", output[0]);
    }

    [Fact]
    public void MultipleRobots_ProcessedSequentially_ScentsShared()
    {
        const string input = "5 3\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL";
        var output = Run(input);
        Assert.Equal("3 3 N LOST", output[0]);
        Assert.Equal("2 3 S", output[1]);
    }
}
