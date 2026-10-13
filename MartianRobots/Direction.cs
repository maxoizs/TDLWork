namespace MartianRobots;

public enum Direction { N, E, S, W }

public static class DirectionExtensions
{
    public static Direction TurnLeft(this Direction d) =>
        d switch
        {
            Direction.N => Direction.W,
            Direction.W => Direction.S,
            Direction.S => Direction.E,
            Direction.E => Direction.N,
            _ => throw new ArgumentOutOfRangeException(nameof(d))
        };

    public static Direction TurnRight(this Direction d) =>
        d switch
        {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            _ => throw new ArgumentOutOfRangeException(nameof(d))
        };
}
