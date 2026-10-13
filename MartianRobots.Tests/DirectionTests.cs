using MartianRobots;

namespace MartianRobots.Tests;

public class DirectionTests
{
    [Theory]
    [InlineData(Direction.N, Direction.W)]
    [InlineData(Direction.W, Direction.S)]
    [InlineData(Direction.S, Direction.E)]
    [InlineData(Direction.E, Direction.N)]
    public void TurnLeft_RotatesCorrectly(Direction from, Direction expected) =>
        Assert.Equal(expected, from.TurnLeft());

    [Theory]
    [InlineData(Direction.N, Direction.E)]
    [InlineData(Direction.E, Direction.S)]
    [InlineData(Direction.S, Direction.W)]
    [InlineData(Direction.W, Direction.N)]
    public void TurnRight_RotatesCorrectly(Direction from, Direction expected) =>
        Assert.Equal(expected, from.TurnRight());

    [Theory]
    [InlineData(Direction.N, 0, 1)]
    [InlineData(Direction.E, 1, 0)]
    [InlineData(Direction.S, 0, -1)]
    [InlineData(Direction.W, -1, 0)]
    public void Delta_ReturnsCorrectOffset(Direction d, int dx, int dy) =>
        Assert.Equal((dx, dy), d.Delta());

    [Fact]
    public void FourLeftTurns_ReturnToStart() =>
        Assert.Equal(Direction.N, Direction.N.TurnLeft().TurnLeft().TurnLeft().TurnLeft());

    [Fact]
    public void FourRightTurns_ReturnToStart() =>
        Assert.Equal(Direction.N, Direction.N.TurnRight().TurnRight().TurnRight().TurnRight());

    [Fact]
    public void LeftThenRight_ReturnToStart() =>
        Assert.Equal(Direction.N, Direction.N.TurnLeft().TurnRight());

    [Fact]
    public void TwoLeftTurns_EqualsTwoRightTurns()
    {
        var viaLeft = Direction.E.TurnLeft().TurnLeft();
        var viaRight = Direction.E.TurnRight().TurnRight();
        Assert.Equal(viaLeft, viaRight);
    }
}
