namespace Application.Responses;

public interface IResult
{
    public abstract bool IsError { get; }
    public abstract List<Error>? Errors { get; }
}

public record Result<TValue> : IResult
{
    public TValue? Value { get; init; }
    public List<Error>? Errors { get; init; }
    public bool IsError => Errors?.Count > 0;

    private Result(List<Error> errors) => Errors = errors;

    private Result(TValue value) => Value = value;

    public static Result<TValue> Error(List<Error> errors) => new(errors);

    public static Result<TValue> Success(TValue value) => new(value);

    public static Result<Empty> Success() => Result<Empty>.Success(new Empty());

    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<List<Error>, TResult> onError
    ) => IsError ? onError(Errors!) : onSuccess(Value!);
}
