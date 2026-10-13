using MartianRobots;

namespace MartianRobots.Tests;

public class ResultTests
{
    [Fact]
    public void Ok_IsSuccess() =>
        Assert.True(Result<int>.Ok(1).IsSuccess);

    [Fact]
    public void Ok_HasValue() =>
        Assert.Equal(42, Result<int>.Ok(42).Value);

    [Fact]
    public void Ok_ErrorIsEmpty() =>
        Assert.Equal(string.Empty, Result<int>.Ok(1).Error);

    [Fact]
    public void Fail_IsNotSuccess() =>
        Assert.False(Result<int>.Fail("bad").IsSuccess);

    [Fact]
    public void Fail_HasError() =>
        Assert.Equal("something went wrong", Result<int>.Fail("something went wrong").Error);

    [Fact]
    public void Fail_ValueIsDefault() =>
        Assert.Equal(0, Result<int>.Fail("bad").Value);

    [Fact]
    public void Ok_WithReferenceType_HasValue()
    {
        var result = Result<string>.Ok("hello");
        Assert.True(result.IsSuccess);
        Assert.Equal("hello", result.Value);
    }

    [Fact]
    public void Fail_WithReferenceType_HasError()
    {
        var result = Result<string>.Fail("oops");
        Assert.False(result.IsSuccess);
        Assert.Equal("oops", result.Error);
    }
}
