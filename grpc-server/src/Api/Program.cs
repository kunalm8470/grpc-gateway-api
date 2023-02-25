using Api.Interceptors;
using Api.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register services to the Dependency injection container

// Register AutoMapper mapping profiles
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register Npgsql connection
string connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddTransient(implementationFactory =>
{
    return new PostgresConnectionFactory(connectionString);
});

// Register repositories
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();

builder.Services.AddGrpc(options => 
{
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors = true;
    }

    options.Interceptors.Add<CorrelationIdInterceptor>();

    options.Interceptors.Add<UnhandledExceptionInterceptor>();
});

builder.Services.AddGrpcReflection();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddGrpcHealthChecks()
.AddNpgSql(connectionString)
.AddCheck("gRPC health check", () => HealthCheckResult.Healthy());

builder.Services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.Zero;
    options.Period = TimeSpan.FromSeconds(10);
});

WebApplication app = builder.Build();

// Register gRPC services
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ProductsService>();

// Setup server reflection
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGrpcHealthChecksService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
