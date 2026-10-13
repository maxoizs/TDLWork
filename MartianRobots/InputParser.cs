namespace MartianRobots;

/// <summary>A robot's starting state paired with its instruction string.</summary>
public record RobotInput(RobotState State, string Instructions);

/// <summary>Parses the challenge's text input format into typed domain objects. Never throws.</summary>
public static class InputParser
{
    private const int MaxCoordinate = 50;

    /// <summary>
    /// Parses the grid-size line (e.g. "5 3") into a <see cref="Grid"/>.
    /// Returns a failed result instead of throwing on malformed input.
    /// </summary>
    public static Result<Grid> ParseGrid(string line)
    {
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
            return Result<Grid>.Fail($"Grid line must have two integers '<maxX> <maxY>', got: '{line.Trim()}'.");

        if (!int.TryParse(parts[0], out var maxX))
            return Result<Grid>.Fail($"Grid x bound '{parts[0]}' is not a valid integer.");

        if (!int.TryParse(parts[1], out var maxY))
            return Result<Grid>.Fail($"Grid y bound '{parts[1]}' is not a valid integer.");

        if (maxX < 0 || maxX > MaxCoordinate)
            return Result<Grid>.Fail($"Grid x bound {maxX} is out of range (0–{MaxCoordinate}).");

        if (maxY < 0 || maxY > MaxCoordinate)
            return Result<Grid>.Fail($"Grid y bound {maxY} is out of range (0–{MaxCoordinate}).");

        return Result<Grid>.Ok(new Grid(maxX, maxY));
    }

    /// <summary>
    /// Parses a robot position line (e.g. "1 1 E") into a <see cref="RobotState"/>.
    /// Direction parsing is case-insensitive. Returns a failed result instead of throwing.
    /// </summary>
    public static Result<RobotState> ParseRobotState(string line)
    {
        var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            return Result<RobotState>.Fail(
                $"Robot position line must have format '<x> <y> <N|S|E|W>', got: '{line.Trim()}'.");

        if (!int.TryParse(parts[0], out var x))
            return Result<RobotState>.Fail($"Robot x coordinate '{parts[0]}' is not a valid integer.");

        if (!int.TryParse(parts[1], out var y))
            return Result<RobotState>.Fail($"Robot y coordinate '{parts[1]}' is not a valid integer.");

        if (!Enum.TryParse<Direction>(parts[2], ignoreCase: true, out var facing))
            return Result<RobotState>.Fail(
                $"Unknown direction '{parts[2]}'. Expected N, S, E, or W.");

        return Result<RobotState>.Ok(new RobotState(x, y, facing));
    }

    /// <summary>
    /// Parses the full input string into a grid and an ordered list of robot inputs.
    /// Format: first line is grid bounds, then pairs of lines per robot (position then instructions).
    /// Returns a failed result instead of throwing on any malformed input.
    /// </summary>
    public static Result<(Grid Grid, List<RobotInput> Robots)> Parse(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
            return Result<(Grid, List<RobotInput>)>.Fail("Input is empty.");

        var gridResult = ParseGrid(lines[0]);
        if (!gridResult.IsSuccess)
            return Result<(Grid, List<RobotInput>)>.Fail(gridResult.Error);

        var robots = new List<RobotInput>();

        for (var i = 1; i < lines.Length; i += 2)
        {
            if (i + 1 >= lines.Length)
                return Result<(Grid, List<RobotInput>)>.Fail(
                    $"Robot at line {i + 1} has a position line but no instruction line.");

            var stateResult = ParseRobotState(lines[i]);
            if (!stateResult.IsSuccess)
                return Result<(Grid, List<RobotInput>)>.Fail(stateResult.Error);

            robots.Add(new RobotInput(stateResult.Value, lines[i + 1].Trim()));
        }

        return Result<(Grid, List<RobotInput>)>.Ok((gridResult.Value, robots));
    }
}
