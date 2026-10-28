namespace MartianRobots;

/// <summary>Parses the challenge's text input one line at a time into typed domain objects. Never throws.</summary>
public static class InputParser
{
    private const int MaxCoordinate = 50;

    /// <summary>
    /// Parses the grid-size line (e.g. "5 3") into a <see cref="Grid"/>.
    /// Returns a failed result instead of throwing on malformed input.
    /// </summary>
    public static Result<Grid> ParseGrid(string line)
    {
        var parts = Split(line);

        if (parts.Length != 2)
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
    /// Parses a robot position line (e.g. "1 1 E") into a <see cref="RobotState"/> and checks
    /// the start lies on <paramref name="grid"/>. Direction parsing is case-insensitive and accepts
    /// only N, E, S or W. Returns a failed result instead of throwing.
    /// </summary>
    public static Result<RobotState> ParseRobotState(string line, Grid grid)
    {
        var parts = Split(line);

        if (parts.Length != 3)
            return Result<RobotState>.Fail(
                $"Robot position line must have format '<x> <y> <N|S|E|W>', got: '{line.Trim()}'.");

        if (!int.TryParse(parts[0], out var x))
            return Result<RobotState>.Fail($"Robot x coordinate '{parts[0]}' is not a valid integer.");

        if (!int.TryParse(parts[1], out var y))
            return Result<RobotState>.Fail($"Robot y coordinate '{parts[1]}' is not a valid integer.");

        if (parts[2].Length != 1 || !"NESW".Contains(char.ToUpperInvariant(parts[2][0])))
            return Result<RobotState>.Fail($"Unknown direction '{parts[2]}'. Expected N, S, E, or W.");

        if (grid.IsOutOfBounds(x, y))
            return Result<RobotState>.Fail($"Robot start ({x}, {y}) is outside the grid.");

        var facing = Enum.Parse<Direction>(parts[2], ignoreCase: true);
        return Result<RobotState>.Ok(new RobotState(x, y, facing));
    }

    /// <summary>
    /// Parses an instruction line (e.g. "RFRF"). Case-insensitive; the result is uppercase.
    /// Fails on any character that is not a registered command. An empty line is valid (no commands).
    /// </summary>
    public static Result<string> ParseInstructions(string line)
    {
        var instructions = line.Trim().ToUpperInvariant();

        foreach (var command in instructions)
            if (!Simulation.IsKnownCommand(command))
                return Result<string>.Fail($"Unknown command '{command}' in instructions '{instructions}'.");

        return Result<string>.Ok(instructions);
    }

    private static string[] Split(string line) =>
        line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
}
