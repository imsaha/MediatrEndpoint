using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FastEndpoints.MediatR.Models;
using FluentValidation;

namespace Api.Infrastructure;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                Converters =
                {
                    new JsonStringEnumConverter()
                },
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            switch (exception)
            {
                case ValidationException validationException:
                {
                    _logger.LogWarning(validationException, "{Message}", validationException.Message);
                    var failures = validationException.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(k => convertPath(k.Key), v => v.First().ErrorMessage);

                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    await context.Response.WriteAsJsonAsync(Result.Failed("One or more validation error has occurred!", failures, context.Response.StatusCode), serializerOptions);
                    break;
                }
                default:
                {
                    _logger.LogError(exception, "{Message}", exception.Message);
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsJsonAsync(Result.Failed(exception.Message, null, context.Response.StatusCode), serializerOptions);
                    break;
                }
            }
        }
    }


    private static string convertPath(string inputPath)
    {
        var regex = new Regex(@"\[(\d+)\]");
        var convertedPath = regex.Replace(inputPath, ".$1");

        var parts = convertedPath.Split('.');
        for (var i = 1; i < parts.Length; i++) // Skip the first part
        {
            parts[i] = char.ToLower(parts[i][0]) + parts[i][1..];
        }

        return string.Join(".", parts);
    }
}
