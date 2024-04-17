namespace Example.Application.Infrastructure.Interfaces;

public interface ISmsService
{
    Task Send(string to, string message, CancellationToken ct = default);
}
