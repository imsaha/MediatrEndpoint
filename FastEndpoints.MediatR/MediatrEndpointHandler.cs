using FastEndpoints.MediatR.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FastEndpoints.MediatR;

public abstract class MediatrEndpointHandler<TRequest> :
    Endpoint<TRequest, Result>,
    IRequestHandler<TRequest, Result> where TRequest : IMediatrEndpoint
{
    [Obsolete("Instead use as parameter HttpContext in the request type", true)]
    private new HttpContext HttpContext { get => throw new NotSupportedException(); }


    public abstract Task<Result> Handle(TRequest req, CancellationToken ct);

    public abstract override void Configure();

    protected virtual void RequestExample(TRequest example)
    {
        Summary(s =>
        {
            s.ExampleRequest = example;
        });
    }

    protected virtual void Ignore()
    {
        Get(typeof(TRequest).FullName!.Replace(".", "_"));
        Tags("Ignore", "Ignore");
    }

    protected virtual void Summary(string summary, string? description = default)
    {
        Description(b => b
            .WithName(typeof(TRequest).FullName!.Replace(".", "_"))
            .Produces<Result>()
            .Produces<Result>(StatusCodes.Status400BadRequest));

        Summary(s =>
            {
                s.Summary = summary;
                s.Description = description ?? "";
            }
        );
    }

    public sealed override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        var httpContextProperty = typeof(TRequest).GetProperties().FirstOrDefault(x => x.PropertyType == typeof(HttpContext));
        httpContextProperty?.SetValue(req, base.HttpContext);

        await Resolve<ISender>().Send(req, ct).Map(SendAsync);
    }
}

public abstract class MediatrEndpointHandler<TRequest, TResponse> :
    Endpoint<TRequest, Result<TResponse>>,
    IRequestHandler<TRequest, Result<TResponse>> where TRequest : IMediatrEndpoint<TResponse>
{
    [Obsolete("Instead use as parameter HttpContext in the request type", true)]
    private new HttpContext HttpContext { get => throw new NotSupportedException(); }


    public abstract Task<Result<TResponse>> Handle(TRequest req, CancellationToken ct);
    public abstract override void Configure();

    protected virtual void RequestExample(TRequest example)
    {
        Summary(s =>
        {
            s.ExampleRequest = example;
        });
    }

    protected virtual void Ignore()
    {
        Get(typeof(TRequest).FullName!.Replace(".", "_"));
        Tags("Ignore", "Ignore");
    }

    protected virtual void Summary(string summary, string? description = default)
    {
        Description(b => b
            .WithName(typeof(TRequest).FullName!.Replace(".", "_"))
            .Produces<Result<TResponse>>()
            .Produces<Result>(StatusCodes.Status400BadRequest));

        Summary(s =>
            {
                s.Summary = summary;
                s.Description = description ?? "";
            }
        );
    }

    public sealed override async Task HandleAsync(TRequest req, CancellationToken ct)
    {
        await Resolve<ISender>().Send(req, ct).Map(SendAsync);
    }
}
