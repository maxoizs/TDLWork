namespace MartianRobots;

public class Simulation
{
    private readonly Dictionary<char, Func<RobotState, Grid, RobotState>> _commands;

    public Simulation()
    {
        _commands = new Dictionary<char, Func<RobotState, Grid, RobotState>>
        {
            ['L'] = (state, _) => state with { Facing = state.Facing.TurnLeft() },
            ['R'] = (state, _) => state with { Facing = state.Facing.TurnRight() },
            ['F'] = MoveForward,
        };
    }

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

    private static RobotState MoveForward(RobotState state, Grid grid)
    {
        var (dx, dy) = state.Facing.Delta();
        var nx = state.X + dx;
        var ny = state.Y + dy;

        if (grid.IsOutOfBounds(nx, ny))
        {
            if (grid.HasScent(state.X, state.Y))
                return state;

            grid.AddScent(state.X, state.Y);
            return state with { IsLost = true };
        }

        return state with { X = nx, Y = ny };
    }
}
