namespace FastEndpoints.MediatR.Models;

public interface IResult<out T> : IResult
{
    T? Data { get; }
}

public interface IResult
{
    bool IsSuccess { get; }
    string? ErrorMessage { get; }
    int StatusCode { get; }
    IDictionary<string, string> Errors { get; }
}
