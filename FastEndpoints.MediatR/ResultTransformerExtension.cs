using FastEndpoints.MediatR.Models;

namespace FastEndpoints.MediatR;

static internal class ResultTransformerExtension
{
    public static async Task Map(this Task<Result> result, Func<Result, Task> func)
    {
        var data = await result;
        await func(data);
    }

    public static async Task Map(this Task<Result> result, Func<Result, int, CancellationToken, Task> func)
    {
        var data = await result;
        await func(data, data.StatusCode, default);
    }

    public static async Task Map<T>(this Task<Result<T>> result, Func<Result<T>, Task> func)
    {
        var data = await result;
        await func(data);
    }

    public static async Task Map<T>(this Task<Result<T>> result, Func<Result<T>, int, CancellationToken, Task> func)
    {
        var data = await result;
        await func(data, data.StatusCode, default);
    }
}
