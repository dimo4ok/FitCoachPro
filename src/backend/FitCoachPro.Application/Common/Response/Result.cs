namespace FitCoachPro.Application.Common.Response;

public class Result
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public List<Error>? Errors { get; init; }

    public static Result Success(int statusCode = 200)
        => new()
        {
            IsSuccess = true,
            StatusCode = statusCode
        };

    public static Result Fail(List<Error> errors, int statusCode = 404)
        => new()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Errors = errors
        };

    public static Result Fail(Error error, int statusCode = 404)
        => new()
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
        => new()
        {
            IsSuccess = true,
            StatusCode = statusCode,
            Data = data
        };

    public static Result<T> Fail(List<Error> errors, int statusCode = 404)
        => new()
        {
            IsSuccess = false,
            StatusCode = statusCode,
            Errors = errors
        };

    public static Result<T> Fail(Error error, int statusCode = 404)
       => new()
       {
           IsSuccess = false,
           StatusCode = statusCode,
           Errors = new List<Error> { error }
       };
}
