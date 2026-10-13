using MartianRobots;

namespace MartianRobots.Tests;

public class GridTests
{
    private readonly Grid _grid = new(5, 3);

    // --- IsOutOfBounds ---

    [Fact]
    public void InBounds_Centre() =>
        Assert.False(_grid.IsOutOfBounds(2, 1));

    [Fact]
    public void InBounds_Origin() =>
        Assert.False(_grid.IsOutOfBounds(0, 0));

    [Fact]
    public void InBounds_TopRight() =>
        Assert.False(_grid.IsOutOfBounds(5, 3));

    [Fact]
    public void InBounds_BottomRight() =>
        Assert.False(_grid.IsOutOfBounds(5, 0));

    [Fact]
    public void InBounds_TopLeft() =>
        Assert.False(_grid.IsOutOfBounds(0, 3));

    [Fact]
    public void OutOfBounds_NegativeX() =>
        Assert.True(_grid.IsOutOfBounds(-1, 0));

    [Fact]
    public void OutOfBounds_NegativeY() =>
        Assert.True(_grid.IsOutOfBounds(0, -1));

    [Fact]
    public void OutOfBounds_ExceedsMaxX() =>
        Assert.True(_grid.IsOutOfBounds(6, 0));

    [Fact]
    public void OutOfBounds_ExceedsMaxY() =>
        Assert.True(_grid.IsOutOfBounds(0, 4));

    [Fact]
    public void OutOfBounds_BothAxesExceeded() =>
        Assert.True(_grid.IsOutOfBounds(6, 4));

    // --- Scent ---

    [Fact]
    public void NoScent_ByDefault() =>
        Assert.False(_grid.HasScent(3, 3));

    [Fact]
    public void HasScent_AfterAdd() =>
        Assert.True(AddAndCheck(3, 3));

    [Fact]
    public void HasScent_AtOrigin() =>
        Assert.True(AddAndCheck(0, 0));

    [Fact]
    public void HasScent_AtCorner() =>
        Assert.True(AddAndCheck(5, 3));

    [Fact]
    public void HasScent_DoesNotLeakToAdjacentCell()
    {
        _grid.AddScent(3, 3);
        Assert.False(_grid.HasScent(3, 2));
        Assert.False(_grid.HasScent(4, 3));
    }

    [Fact]
    public void AddScent_IsDeduplicated()
    {
        _grid.AddScent(1, 1);
        _grid.AddScent(1, 1);
        Assert.True(_grid.HasScent(1, 1));
    }

    private bool AddAndCheck(int x, int y)
    {
        _grid.AddScent(x, y);
        return _grid.HasScent(x, y);
    }
}
