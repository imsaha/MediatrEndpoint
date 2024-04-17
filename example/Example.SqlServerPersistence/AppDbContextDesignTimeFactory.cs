using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Example.SqlServerPersistence;

internal sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var dbContextBuilder = new DbContextOptionsBuilder<AppDbContext>();
        const string connectionString = "Data Source=.;Initial Catalog=DEV_ExampleDB;Integrated Security=False;User Id=sa;Password=P@ssword23;MultipleActiveResultSets=false;TrustServerCertificate=True;";
        dbContextBuilder.UseSqlServer(connectionString, builder =>
        {
            builder.MigrationsAssembly(typeof(AppDbContextDesignTimeFactory).Assembly.FullName);
        });
        return new AppDbContext(dbContextBuilder.Options);
    }
}
