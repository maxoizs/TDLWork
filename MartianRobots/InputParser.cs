namespace MartianRobots;

/// <summary>A robot's starting state paired with its instruction string.</summary>
public record RobotInput(RobotState State, string Instructions);

/// <summary>Parses the challenge's text input format into typed domain objects.</summary>
public static class InputParser
{
    /// <summary>
    /// Parses the grid-size line (e.g. "5 3") into a <see cref="Grid"/>.
    /// Lower-left is always (0, 0); the values are the upper-right bounds.
    /// </summary>
    public static Grid ParseGrid(string line)
    {
        var parts = line.Trim().Split(' ');
        return new Grid(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    /// <summary>
    /// Parses a robot position line (e.g. "1 1 E") into a <see cref="RobotState"/>.
    /// </summary>
    public static RobotState ParseRobotState(string line)
    {
        var parts = line.Trim().Split(' ');
        var x = int.Parse(parts[0]);
        var y = int.Parse(parts[1]);
        var facing = Enum.Parse<Direction>(parts[2]);
        return new RobotState(x, y, facing);
    }

    /// <summary>
    /// Parses the full input string into a grid and an ordered list of robot inputs.
    /// Format: first line is grid bounds, then pairs of lines per robot (position then instructions).
    /// </summary>
    public static (Grid Grid, List<RobotInput> Robots) Parse(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var grid = ParseGrid(lines[0]);
        var robots = new List<RobotInput>();

        for (var i = 1; i + 1 < lines.Length; i += 2)
            robots.Add(new RobotInput(ParseRobotState(lines[i]), lines[i + 1].Trim()));

        return (grid, robots);
    }
}
