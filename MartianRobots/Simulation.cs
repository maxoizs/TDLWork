using Command = System.Func<MartianRobots.RobotState, MartianRobots.Grid, MartianRobots.RobotState>;

namespace MartianRobots;

/// <summary>
/// Runs robots through their instruction sequences on a grid.
/// Commands are registered in a dictionary for extensibility — adding a new
/// command type requires one entry with no changes to the execution loop.
/// </summary>
public class Simulation
{
    private static readonly Dictionary<char, Command> _commands = new()
    {
        ['L'] = (state, _) => state with { Facing = state.Facing.TurnLeft() },
        ['R'] = (state, _) => state with { Facing = state.Facing.TurnRight() },
        ['F'] = MoveForward,
    };

    /// <summary>Returns true if <paramref name="command"/> is a registered command character.</summary>
    public static bool IsKnownCommand(char command) => _commands.ContainsKey(command);

    /// <summary>
    /// Executes an instruction string for a single robot and returns its final state.
    /// Unknown commands are silently ignored, allowing future command types without breaking existing runs.
    /// </summary>
    /// <param name="state">Starting position and orientation.</param>
    /// <param name="instructions">String of command characters (L, R, F).</param>
    /// <param name="grid">The grid — holds bounds and shared scent state across all robots.</param>
    /// <returns>Final robot state after all instructions; IsLost is true if the robot fell off the grid.</returns>
    public RobotState Execute(RobotState state, string instructions, Grid grid)
    {
        foreach (var cmd in instructions)
        {
            if (state.IsLost) break;
            if (_commands.TryGetValue(cmd, out var execute))
                state = execute(state, grid);
        }
        return state;
    }

    /// <summary>
    /// Attempts to move the robot one step forward. If the next position is out of bounds:
    /// - and a scent exists at the current position, the instruction is ignored (robot survives);
    /// - otherwise the robot is marked LOST and a scent is deposited at the last valid position.
    /// </summary>
    private static RobotState MoveForward(RobotState state, Grid grid)
    {
        var (dx, dy) = state.Facing.Delta();
        var nx = state.X + dx;
        var ny = state.Y + dy;

        if (grid.IsOutOfBounds(nx, ny))
            return grid.TryAddScent(state.X, state.Y)
                ? state with { IsLost = true }
                : state;

        return state with { X = nx, Y = ny };
    }
}
