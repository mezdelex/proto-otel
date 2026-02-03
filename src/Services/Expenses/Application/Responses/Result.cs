namespace Application.Responses;

public interface IResult
{
    public Error? Error { get; }
    public bool IsError { get; }
}

public sealed record Result<TValue> : IResult
{
    public TValue? Value { get; init; }
    public Error? Error { get; init; }
    public bool IsError => Error != null;

    private Result(TValue value) => Value = value;

    private Result(Error error) => Error = error;

    public static Result<TValue> Success(TValue value) => new(value);

    public static Result<Empty> Success() => Result<Empty>.Success(new Empty());

    public static Result<TValue> Failure(Error error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onError) =>
        IsError ? onError(Error!) : onSuccess(Value!);
}
