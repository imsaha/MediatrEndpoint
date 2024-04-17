using Example.Core;
using Microsoft.EntityFrameworkCore;

namespace Example.Application.Infrastructure.Interfaces;

public interface IAppDbContext
{
    DbSet<Employee> Employees { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
