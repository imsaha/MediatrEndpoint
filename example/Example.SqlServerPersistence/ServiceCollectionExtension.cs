using Example.Application.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Example.SqlServerPersistence;

public static class ServiceCollectionExtension
{
    public static void AddSqlServerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("db")!;

        services.AddScoped<IAppDbContext>(x => x.GetService<AppDbContext>()!);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options
                .UseSqlServer(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(AppDbContextDesignTimeFactory).Assembly.FullName);
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        });
    }
}
