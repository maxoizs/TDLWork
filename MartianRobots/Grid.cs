namespace MartianRobots;

public class Grid(int maxX, int maxY)
{
    private readonly HashSet<(int, int)> _scents = new();

    public bool IsOutOfBounds(int x, int y) =>
        x < 0 || y < 0 || x > maxX || y > maxY;

    public bool HasScent(int x, int y) => _scents.Contains((x, y));

    public void AddScent(int x, int y) => _scents.Add((x, y));
}
