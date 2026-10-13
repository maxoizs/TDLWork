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
}
