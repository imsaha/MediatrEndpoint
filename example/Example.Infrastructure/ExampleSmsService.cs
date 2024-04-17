using Example.Application.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Example.Infrastructure;

internal sealed class ExampleSmsService : ISmsService
{
    private readonly ILogger<ExampleSmsService> _logger;
    public ExampleSmsService(ILogger<ExampleSmsService> logger)
    {
        _logger = logger;
    }

    public Task Send(string to, string message, CancellationToken ct = default)
    {
        _logger.LogInformation("SMS to: {T}, Message: {M}", to, message);
        return Task.CompletedTask;
    }
}
