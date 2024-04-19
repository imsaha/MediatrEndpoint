# FastEndpoints.MediatR

Combines MediatR and FastEndpoint for quickly generating API endpoints from MediatR Requests

## Request

Mark class or record as  `IMediatrEndpoint` or  `IMediatrEndpoint<T>` accordingly. This is an EndpointRequest Type
in `FastEndpoints` and IRequest in `MediatR`

```cs
public record CreateEmployee(string Code, string Name) : IMediatrEndpoint;
```

## Request handler

Impletement `MediatrEndpointHandler<TRequest>` or MediatrEndpointHandler<TRequest, TResponse> accordingly. This
implements Endpoint<T> or Endpoint<T, TResponse> accordingly in `FastEndpoints`
and `IRequestHandler<TRequest, TResponse>` in MediatR

```cs
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
```

## Validator

Optionally you can use FluentValidation to generate validation for a request

```cs
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
```

## Program

```cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();

var app = builder.Build();

// Not require if you don't want to use FastEndpoints
app.UseFastEndpoints(config =>
{
    config.Endpoints.RoutePrefix = "api";
    config.Versioning.Prefix = "v";
    config.Versioning.PrependToRoute = true;
    config.Versioning.DefaultVersion = 1;
    config.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
    config.Serializer.Options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    config.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    config.Endpoints.IgnoreIsNotEndpoints();
});

//Use as minimal API if you wish
app.MapPost("/createEmployee", async ([FromServices]ISender sender, [FromBody]CreateEmployee command) =>
{
    var result = await sender.Send(command);
    return Results.Ok(result);
}).WithSummary("Using MediatR command");


app.UseSwaggerGen(default, config =>
{
    config.ValidateSpecification = true;
    config.DocExpansion = "list";
    config.DefaultModelExpandDepth = -1;
});


app.Run();


```
