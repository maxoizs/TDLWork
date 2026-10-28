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
        _grid.TryAddScent(3, 3);
        Assert.False(_grid.HasScent(3, 2));
        Assert.False(_grid.HasScent(4, 3));
    }

    [Fact]
    public void TryAddScent_IsDeduplicated()
    {
        _grid.TryAddScent(1, 1);
        _grid.TryAddScent(1, 1);
        Assert.True(_grid.HasScent(1, 1));
    }

    [Fact]
    public void TryAddScent_FirstCall_ReturnsTrue() =>
        Assert.True(_grid.TryAddScent(2, 2));

    [Fact]
    public void TryAddScent_SecondCallSamePosition_ReturnsFalse()
    {
        _grid.TryAddScent(2, 2);
        Assert.False(_grid.TryAddScent(2, 2));
    }

    [Fact]
    public void TryAddScent_DifferentPosition_ReturnsTrue()
    {
        _grid.TryAddScent(2, 2);
        Assert.True(_grid.TryAddScent(2, 3));
    }

    private bool AddAndCheck(int x, int y)
    {
        _grid.TryAddScent(x, y);
        return _grid.HasScent(x, y);
    }
}
