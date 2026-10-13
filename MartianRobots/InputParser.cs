namespace MartianRobots;

public record RobotInput(RobotState State, string Instructions);

public static class InputParser
{
    public static Grid ParseGrid(string line)
    {
        var parts = line.Trim().Split(' ');
        return new Grid(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    public static RobotState ParseRobotState(string line)
    {
        var parts = line.Trim().Split(' ');
        var x = int.Parse(parts[0]);
        var y = int.Parse(parts[1]);
        var facing = Enum.Parse<Direction>(parts[2]);
        return new RobotState(x, y, facing);
    }

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
