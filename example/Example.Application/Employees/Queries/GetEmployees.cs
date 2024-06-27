using FastEndpoints.MediatR;
using FastEndpoints.MediatR.Models;

namespace Example.Application.Employees.Queries;

public record EmployeeListItemDto(string Test);
public record GetEmployees : IMediatrEndpoint<List<EmployeeListItemDto>>;

internal sealed class GetEmployeesHandler : MediatrEndpointHandler<GetEmployees, List<EmployeeListItemDto>>
{
    public override Task<Result<List<EmployeeListItemDto>>> Handle(GetEmployees req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
    public override void Configure()
    {
        Get("");
        Group<EmployeesGroup>();
        Summary("Get all created employees");
        RequestExample(new GetEmployees());
        AllowAnonymous();
    }
}
