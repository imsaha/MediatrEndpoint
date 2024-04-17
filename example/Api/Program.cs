using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Infrastructure;
using Example.Application;
using Example.Application.Employees.Commands;
using Example.Infrastructure;
using Example.SqlServerPersistence;
using FastEndpoints;
using FastEndpoints.MediatR.Attributes;
using FastEndpoints.Swagger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(options =>
{
    options.MaxEndpointVersion = 1;
    options.DocumentSettings = doc =>
    {
        doc.Title = "Example Api";
        doc.DocumentName = "v1.0";
        doc.Version = "v1.0";
    };

    options.SerializerSettings = json =>
    {
        json.Converters.Add(new JsonStringEnumConverter());
        json.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        json.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    };
});

builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

builder.Services.AddApplication<CurrentUser>(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSqlServerPersistence(builder.Configuration);

var app = builder.Build();

app.MapPost("/createEmployee", async ([FromServices]ISender sender, [Microsoft.AspNetCore.Mvc.FromBody]CreateEmployee command) =>
{
    var result = await sender.Send(command);
    return Results.Ok(result);
}).WithSummary("Using MediatR command");

// app.UseAuthorization();

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

app.UseMiddleware<ExceptionMiddleware>();


app.UseSwaggerGen(default, config =>
{
    config.ValidateSpecification = true;
    config.DocExpansion = "list";
    config.DefaultModelExpandDepth = -1;
});



app.Run();
