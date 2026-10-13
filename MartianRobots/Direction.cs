namespace MartianRobots;

/// <summary>Cardinal compass directions a robot can face.</summary>
public enum Direction { N, E, S, W }

public static class DirectionExtensions
{
    /// <summary>Returns the direction after a 90-degree left (counter-clockwise) turn.</summary>
    public static Direction TurnLeft(this Direction d) =>
        d switch
        {
            Direction.N => Direction.W,
            Direction.W => Direction.S,
            Direction.S => Direction.E,
            Direction.E => Direction.N,
            _ => throw new ArgumentOutOfRangeException(nameof(d))
        };

    /// <summary>Returns the direction after a 90-degree right (clockwise) turn.</summary>
    public static Direction TurnRight(this Direction d) =>
        d switch
        {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N,
            _ => throw new ArgumentOutOfRangeException(nameof(d))
        };

    /// <summary>Returns the (dx, dy) unit vector for one forward step in this direction.</summary>
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
