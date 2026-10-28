using MartianRobots;

namespace MartianRobots.Tests;

/// <summary>Runs the real parser, simulation and runner together on the challenge's sample.</summary>
public class IntegrationTests
{
    [Fact]
    public void ChallengeSample_ProducesExpectedOutput()
    {
        const string input = "5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n0 3 W\nLLFFFLFLFL\n";
        var output = new StringWriter { NewLine = "\n" };

        var exitCode = Runner.Run(new StringReader(input), output, TextWriter.Null);

        Assert.Equal(0, exitCode);
        Assert.Equal("1 1 E\n3 3 N LOST\n2 3 S\n", output.ToString());
    }
}
