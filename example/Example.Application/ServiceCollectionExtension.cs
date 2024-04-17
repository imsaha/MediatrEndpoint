using Example.Application.Infrastructure.Behaviours;
using Example.Application.Infrastructure.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Application;

public static class ServiceCollectionExtension
{
    public static void AddApplication<T>(this IServiceCollection services, IConfiguration configuration)
        where T : class, ICurrentUser
    {
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtension).Assembly, includeInternalTypes: true);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddTransient(typeof(ICurrentUser), typeof(T));

    }
}
