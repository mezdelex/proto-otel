namespace Application.Responses;

public interface IResult<TValue>
{
    public TValue? Value { get; }
    public List<Error>? Errors { get; }
    public bool IsError { get; }
}

public record Result<TValue> : IResult<TValue>
{
    public TValue? Value { get; init; }
    public List<Error>? Errors { get; init; }
    public bool IsError => Errors?.Count > 0;

    private Result(TValue value) => Value = value;

    private Result(List<Error> errors) => Errors = errors;

    public static Result<TValue> Success(TValue value) => new(value);

    public static Result<Empty> Success() => Result<Empty>.Success(new Empty());

    public static Result<TValue> Error(List<Error> errors) => new(errors);

    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<List<Error>, TResult> onError
    ) => IsError ? onError(Errors!) : onSuccess(Value!);
}
