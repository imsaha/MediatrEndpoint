using FastEndpoints.MediatR.Models;
using MediatR;

namespace FastEndpoints.MediatR;

public interface IMediatrEndpoint<TOut> : IRequest<Result<TOut>>
{
}

public interface IMediatrEndpoint : IRequest<Result>
{
}
