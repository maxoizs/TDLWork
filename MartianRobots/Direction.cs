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

    public static (int dx, int dy) Delta(this Direction d) =>
        d switch
        {
            Direction.N => (0, 1),
            Direction.E => (1, 0),
            Direction.S => (0, -1),
            Direction.W => (-1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(d))
        };
}
