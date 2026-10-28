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

    // --- LOST and scent deposit ---

    [Fact]
    public void FallOffNorthEdge_IsLost()
    {
        var result = _sim.Execute(new RobotState(0, 3, Direction.N), "F", _grid);
        Assert.True(result.IsLost);
        Assert.Equal(new RobotState(0, 3, Direction.N, IsLost: true), result);
    }

    [Fact]
    public void FallOffSouthEdge_IsLost()
    {
        var result = _sim.Execute(new RobotState(0, 0, Direction.S), "F", _grid);
        Assert.True(result.IsLost);
        Assert.Equal(new RobotState(0, 0, Direction.S, IsLost: true), result);
    }

    [Fact]
    public void FallOffEastEdge_IsLost()
    {
        var result = _sim.Execute(new RobotState(5, 1, Direction.E), "F", _grid);
        Assert.True(result.IsLost);
        Assert.Equal(new RobotState(5, 1, Direction.E, IsLost: true), result);
    }

    [Fact]
    public void FallOffWestEdge_IsLost()
    {
        var result = _sim.Execute(new RobotState(0, 1, Direction.W), "F", _grid);
        Assert.True(result.IsLost);
        Assert.Equal(new RobotState(0, 1, Direction.W, IsLost: true), result);
    }

    [Fact]
    public void FallOffCorner_IsLost()
    {
        var result = _sim.Execute(new RobotState(5, 3, Direction.N), "F", _grid);
        Assert.True(result.IsLost);
    }

    [Fact]
    public void InstructionsAfterLost_AreIgnored()
    {
        var result = _sim.Execute(new RobotState(0, 3, Direction.N), "FRRRFFF", _grid);
        Assert.True(result.IsLost);
        Assert.Equal(Direction.N, result.Facing);
        Assert.Equal(0, result.X);
        Assert.Equal(3, result.Y);
    }

    [Fact]
    public void FallenRobot_LeavesScent()
    {
        _sim.Execute(new RobotState(3, 3, Direction.N), "F", _grid);
        Assert.True(_grid.HasScent(3, 3));
    }

    [Fact]
    public void Sample_Robot2_Lost()
    {
        var result = _sim.Execute(new RobotState(3, 2, Direction.N), "FRRFLLFFRRFLL", _grid);
        Assert.Equal(new RobotState(3, 3, Direction.N, IsLost: true), result);
    }

    // --- Scent prevents falling ---

    [Fact]
    public void Scent_PreventsRobotFallingOffSameEdge()
    {
        _grid.TryAddScent(3, 3);
        var result = _sim.Execute(new RobotState(3, 3, Direction.N), "F", _grid);
        Assert.False(result.IsLost);
        Assert.Equal(new RobotState(3, 3, Direction.N), result);
    }

    [Fact]
    public void Scent_RobotContinuesAfterIgnoredInstruction()
    {
        _grid.TryAddScent(3, 3);
        var result = _sim.Execute(new RobotState(3, 3, Direction.N), "FLF", _grid);
        Assert.False(result.IsLost);
        Assert.Equal(new RobotState(2, 3, Direction.W), result);
    }

    [Fact]
    public void Scent_DoesNotProtectDifferentPosition()
    {
        _grid.TryAddScent(3, 3);
        var result = _sim.Execute(new RobotState(4, 3, Direction.N), "F", _grid);
        Assert.True(result.IsLost);
    }

    [Fact]
    public void Scent_DoesNotBlockSafeMoveFromSamePosition()
    {
        _grid.TryAddScent(5, 1);
        var result = _sim.Execute(new RobotState(5, 1, Direction.N), "F", _grid);
        Assert.False(result.IsLost);
        Assert.Equal(new RobotState(5, 2, Direction.N), result);
    }

    [Fact]
    public void Sample_Robot3_SavedByScent()
    {
        _sim.Execute(new RobotState(3, 2, Direction.N), "FRRFLLFFRRFLL", _grid);
        var result = _sim.Execute(new RobotState(0, 3, Direction.W), "LLFFFLFLFL", _grid);
        Assert.Equal(new RobotState(2, 3, Direction.S), result);
        Assert.False(result.IsLost);
    }

    // --- Unknown command characters ---

    [Fact]
    public void UnknownCommand_IsIgnored()
    {
        var start = new RobotState(1, 1, Direction.N);
        var result = _sim.Execute(start, "X", _grid);
        Assert.Equal(start, result);
    }

    [Fact]
    public void MixedUnknownAndKnownCommands_OnlyKnownApplied()
    {
        var result = _sim.Execute(new RobotState(1, 1, Direction.N), "XFZ", _grid);
        Assert.Equal(new RobotState(1, 2, Direction.N), result);
    }

    // --- Boundary-parallel movement ---

    [Fact]
    public void MoveAlongEastBoundary_StaysInBounds()
    {
        var result = _sim.Execute(new RobotState(5, 0, Direction.N), "F", _grid);
        Assert.False(result.IsLost);
        Assert.Equal(new RobotState(5, 1, Direction.N), result);
    }

    [Fact]
    public void MoveAlongSouthBoundary_StaysInBounds()
    {
        var result = _sim.Execute(new RobotState(0, 0, Direction.E), "F", _grid);
        Assert.False(result.IsLost);
        Assert.Equal(new RobotState(1, 0, Direction.E), result);
    }

    // --- Multiple robots depositing scent at same position ---

    [Fact]
    public void TwoRobots_FallOffSameEdge_SecondScent_IsNoOp()
    {
        var grid = new Grid(5, 3);
        var sim = new Simulation();
        sim.Execute(new RobotState(3, 3, Direction.N), "F", grid);
        sim.Execute(new RobotState(3, 3, Direction.N), "F", grid);
        Assert.True(grid.HasScent(3, 3));

        var survivor = sim.Execute(new RobotState(3, 3, Direction.N), "F", grid);
        Assert.False(survivor.IsLost);
    }

    // --- Corners: scent is position-only, so one lost robot protects both edges at a corner ---

    [Theory]
    [InlineData(5, 3, Direction.N, Direction.E)]
    [InlineData(5, 3, Direction.E, Direction.N)]
    [InlineData(0, 0, Direction.S, Direction.W)]
    [InlineData(0, 0, Direction.W, Direction.S)]
    [InlineData(0, 3, Direction.N, Direction.W)]
    [InlineData(5, 0, Direction.E, Direction.S)]
    public void Corner_ScentFromOneEdge_ProtectsTheOtherEdge(int x, int y, Direction first, Direction second)
    {
        var lost = _sim.Execute(new RobotState(x, y, first), "F", _grid);
        Assert.True(lost.IsLost);

        var saved = _sim.Execute(new RobotState(x, y, second), "F", _grid);
        Assert.Equal(new RobotState(x, y, second), saved);
    }

    [Fact]
    public void Corner_SecondRobotIgnoresBothExits()
    {
        _sim.Execute(new RobotState(5, 3, Direction.N), "FRF", _grid);
        var result = _sim.Execute(new RobotState(5, 3, Direction.N), "FRF", _grid);
        Assert.Equal(new RobotState(5, 3, Direction.E), result);
    }

    [Fact]
    public void Corner_ScentDoesNotBlockMovingInward()
    {
        _sim.Execute(new RobotState(5, 3, Direction.N), "F", _grid);
        var result = _sim.Execute(new RobotState(5, 3, Direction.S), "F", _grid);
        Assert.Equal(new RobotState(5, 2, Direction.S), result);
    }

    [Theory]
    [InlineData(Direction.N)]
    [InlineData(Direction.E)]
    [InlineData(Direction.S)]
    [InlineData(Direction.W)]
    public void SinglePointGrid_ScentProtectsAllFourExits(Direction facing)
    {
        var grid = new Grid(0, 0);
        _sim.Execute(new RobotState(0, 0, Direction.N), "F", grid);
        var result = _sim.Execute(new RobotState(0, 0, facing), "F", grid);
        Assert.False(result.IsLost);
    }

    [Fact]
    public void LostRobot_DoesNotLeaveScentAtOffGridCell()
    {
        _sim.Execute(new RobotState(5, 3, Direction.E), "F", _grid);
        Assert.False(_grid.HasScent(6, 3));
        Assert.True(_grid.HasScent(5, 3));
    }

    [Fact]
    public void RobotAlreadyLost_DoesNotExecuteAnything()
    {
        var start = new RobotState(2, 2, Direction.N, IsLost: true);
        Assert.Equal(start, _sim.Execute(start, "FFLR", _grid));
    }

    [Fact]
    public void IsKnownCommand_ReportsOnlyRegisteredCommands()
    {
        Assert.True(Simulation.IsKnownCommand('L'));
        Assert.True(Simulation.IsKnownCommand('R'));
        Assert.True(Simulation.IsKnownCommand('F'));
        Assert.False(Simulation.IsKnownCommand('X'));
        Assert.False(Simulation.IsKnownCommand('f'));
    }

    // --- Minimal grid simulation ---

    [Fact]
    public void MinimalGrid_RobotMovesOff_IsLost()
    {
        var grid = new Grid(0, 0);
        var result = new Simulation().Execute(new RobotState(0, 0, Direction.N), "F", grid);
        Assert.True(result.IsLost);
        Assert.Equal(new RobotState(0, 0, Direction.N, IsLost: true), result);
    }

    [Fact]
    public void MinimalGrid_TurnOnly_Survives()
    {
        var grid = new Grid(0, 0);
        var result = new Simulation().Execute(new RobotState(0, 0, Direction.N), "LLLL", grid);
        Assert.False(result.IsLost);
        Assert.Equal(new RobotState(0, 0, Direction.N), result);
    }
}
