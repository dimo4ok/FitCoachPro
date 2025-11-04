namespace FitCoachPro.Application.Common.Response;

public class Result
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public List<Error>? Errors { get; init; }

    public static Result Success(int statusCode = 200)
        => new Result
        {
            IsSuccess = true,
            StatusCode = statusCode
        };

    public static Result Fail(List<Error> errors, int statusCode = 404)
        => new Result
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Errors = errors
        };

    public static Result Fail(Error error, int statusCode = 404)
        => new Result
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Errors = new List<Error> { error }
        };
}

public class Result<T>
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public T? Data { get; init; }
    public List<Error>? Errors { get; init; }

    public static Result<T> Success(T data, int statusCode = 200)
        => new Result<T>
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Data = data
        };

    public static Result<T> Fail(List<Error> errors, int statusCode = 404)
        => new Result<T>
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Errors = errors
        };

    public static Result<T> Fail(Error error, int statusCode = 404)
       => new Result<T>
       {
           IsSuccess = false,
           StatusCode = statusCode,
           Errors = new List<Error> { error }
       };

    public static Result<T> FromResult<U>(Result<U> result)
        => result.IsSuccess
            ? Result<T>.Success(default!)
            : Result<T>.Fail(result.Errors!, result.StatusCode);

    public static Result<T> FromResult<U>(Result<U> result, T data)
        => result.IsSuccess
            ? Result<T>.Success(data)
            : Result<T>.Fail(result.Errors!, result.StatusCode);
}
