namespace MartianRobots;

/// <summary>Represents either a successful value or a failure message, with no exceptions in the flow.</summary>
public record Result<T>
{
    public bool IsSuccess { get; private init; }
    public T Value { get; private init; } = default!;
    public string Error { get; private init; } = string.Empty;

    public static Result<T> Ok(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(string error) => new() { IsSuccess = false, Error = error };
}
