using Example.Application.Infrastructure.Interfaces;
using Example.Core;
using Microsoft.EntityFrameworkCore;

namespace Example.SqlServerPersistence;

internal sealed class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Employees = Set<Employee>();
    }

    public DbSet<Employee> Employees { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
