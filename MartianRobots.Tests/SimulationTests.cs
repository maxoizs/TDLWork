using MartianRobots;

namespace MartianRobots.Tests;

public class SimulationTests
{
    private readonly Grid _grid = new(5, 3);
    private readonly Simulation _sim = new();

    // --- Turns ---

    [Fact]
    public void TurnLeft_ChangesOrientationOnly()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.N), "L", _grid);
        Assert.Equal(new RobotState(1, 1, Direction.W), result);
    }

    [Fact]
    public void TurnRight_ChangesOrientationOnly()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.N), "R", _grid);
        Assert.Equal(new RobotState(1, 1, Direction.E), result);
    }

    [Fact]
    public void FourLeftTurns_ReturnToOriginalState()
    {
        var start = new RobotState(2, 2, Direction.S);
        var result = _sim.Execute(start, "LLLL", _grid);
        Assert.Equal(start, result);
    }

    [Fact]
    public void FourRightTurns_ReturnToOriginalState()
    {
        var start = new RobotState(2, 2, Direction.E);
        var result = _sim.Execute(start, "RRRR", _grid);
        Assert.Equal(start, result);
    }

    // --- Forward movement ---

    [Fact]
    public void MoveNorth_IncrementsY()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.N), "F", _grid);
        Assert.Equal(new RobotState(1, 2, Direction.N), result);
    }

    [Fact]
    public void MoveSouth_DecrementsY()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.S), "F", _grid);
        Assert.Equal(new RobotState(1, 0, Direction.S), result);
    }

    [Fact]
    public void MoveEast_IncrementsX()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.E), "F", _grid);
        Assert.Equal(new RobotState(2, 1, Direction.E), result);
    }

    [Fact]
    public void MoveWest_DecrementsX()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.W), "F", _grid);
        Assert.Equal(new RobotState(0, 1, Direction.W), result);
    }

    // --- Combined ---

    [Fact]
    public void TurnThenMove_UpdatesBothPositionAndOrientation()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.N), "RF", _grid);
        Assert.Equal(new RobotState(2, 1, Direction.E), result);
    }

    [Fact]
    public void EmptyInstructions_ReturnsSameState()
    {
        var start = new RobotState(3, 2, Direction.W);
        var result = _sim.Execute(start, "", _grid);
        Assert.Equal(start, result);
    }

    [Fact]
    public void Sample_Robot1_CircularPath()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.E), "RFRFRFRF", _grid);
        Assert.Equal(new RobotState(1, 1, Direction.E), result);
    }
}
