using System.Text.Json.Serialization;

namespace FastEndpoints.MediatR.Models;

public struct Result : IResult
{
    private Result(bool isSuccess, string? errorMessage, int statusCode, IDictionary<string, string> errors)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
        Errors = errors;
    }
    public bool IsSuccess { get; private init; }
    public string? ErrorMessage { get; private init; }

    [JsonIgnore]
    public int StatusCode { get; private init; }

    public IDictionary<string, string> Errors { get; private init; }

    public static Result SucceedIf(bool isSuccess, string? message = default, int statusCode = 0, IDictionary<string, string>? errors = default)
    {
        return new Result
        {
            IsSuccess = isSuccess,
            ErrorMessage = message,
            StatusCode = statusCode == 0 ? isSuccess ? 200 : 400 : statusCode,
            Errors = errors ?? new Dictionary<string, string>()
        };
    }

    public static Result Succeed(string? message = default, int statusCode = 200)
    {
        return SucceedIf(true, message, statusCode);
    }

    public static Result Failed(string? message = default, IDictionary<string, string>? errors = default, int statusCode = 400)
    {
        return SucceedIf(false, message, statusCode, errors);
    }

    public static Result<T> SucceedIf<T>(bool isSuccess, string? message = default, IDictionary<string, string>? errors = default, T? data = default, int statusCode = 400)
    {
        return Result<T>.SucceedIf(isSuccess, message, statusCode, errors, data);
    }

    public static Result<T> Succeed<T>(T data, int statusCode = 200)
    {
        return SucceedIf(true, null, null, data, statusCode);
    }

    public static Result<T> Failed<T>(string? message = default, IDictionary<string, string>? errors = default, int statusCode = 400)
    {
        return SucceedIf<T>(false, message, errors, default, statusCode);
    }
}

public struct Result<T> : IResult<T>
{
    private Result(bool isSuccess, string? errorMessage, int statusCode, IDictionary<string, string> errors, T? data)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
        Errors = errors;
        Data = data;
    }
    public bool IsSuccess { get; private init; }
    public string? ErrorMessage { get; private init; }
    public int StatusCode { get; set; }
    public IDictionary<string, string> Errors { get; private init; }
    public T? Data { get; private init; }


    public static Result<T> SucceedIf(bool isSuccess, string? errorMessage, int statusCode, IDictionary<string, string>? errors, T? data)
    {
        return new Result<T>
        {
            IsSuccess = isSuccess,
            ErrorMessage = errorMessage,
            Errors = errors ?? new Dictionary<string, string>(),
            Data = data

        };
    }
}
