namespace FastEndpoints.MediatR.Models;

internal interface IResult<out T> : IResult
{
    T? Data { get; }
}

internal interface IResult
{
    bool IsSuccess { get; }
    string? ErrorMessage { get; }
    int StatusCode { get; }
    IDictionary<string, string> Errors { get; }
}
