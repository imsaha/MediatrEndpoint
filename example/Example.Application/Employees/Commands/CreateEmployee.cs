using Example.Application.Infrastructure.Interfaces;
using Example.Core;
using FastEndpoints.MediatR;
using FastEndpoints.MediatR.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Example.Application.Employees.Commands;

public record CreateEmployee(string Code, string Name) : IMediatrEndpoint;

internal sealed class CreateEmployeeValidator : AbstractValidator<CreateEmployee>
{
    private readonly IAppDbContext _dbContext;
    public CreateEmployeeValidator(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
        RuleFor(x => x.Code).NotNull().MustAsync(beUniqueAsync).WithMessage("Code number already exists!");
        RuleFor(x => x.Name).NotNull();
    }

    private async Task<bool> beUniqueAsync(string code, CancellationToken ct)
    {
        return !await _dbContext.Employees.AnyAsync(x => x.Code == code, ct);
    }
}

internal sealed class CreateEmployeeHandler : MediatrEndpointHandler<CreateEmployee>
{
    private readonly IAppDbContext _dbContext;
    public CreateEmployeeHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<Result> Handle(CreateEmployee req, CancellationToken ct)
    {
        var employee = new Employee
        {
            Code = req.Code,
            Name = req.Name
        };

        await _dbContext.Employees.AddAsync(employee, ct);
        await _dbContext.SaveChangesAsync(ct);
        return Result.Succeed();
    }

    public override void Configure()
    {
        Post("create");
        Group<EmployeesGroup>();
        Summary("Create a new employee");
        RequestExample(new CreateEmployee("TEST0254", "Jone doe"));
        AllowAnonymous();
    }
}
