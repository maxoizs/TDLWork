namespace MartianRobots;

/// <summary>Bounded rectangular grid that tracks scent positions left by lost robots.</summary>
/// <param name="maxX">Upper-right x bound (inclusive). Lower-left is always 0,0.</param>
/// <param name="maxY">Upper-right y bound (inclusive).</param>
public class Grid(int maxX, int maxY)
{
    private readonly HashSet<(int, int)> _scents = [];

    /// <summary>Returns true if (x, y) lies outside the grid boundaries.</summary>
    public bool IsOutOfBounds(int x, int y) =>
        x < 0 || y < 0 || x > maxX || y > maxY;

    /// <summary>Returns true if a robot previously fell off the grid from (x, y).</summary>
    public bool HasScent(int x, int y) => _scents.Contains((x, y));

    /// <summary>
    /// Marks (x, y) as a scent position. Returns true if the scent is new (this robot is the first
    /// to be lost here) and false if one was already there. Check and add are a single step.
    /// </summary>
    public bool TryAddScent(int x, int y) => _scents.Add((x, y));
}
