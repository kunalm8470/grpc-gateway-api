using Api.Interceptors;
using Api.Services;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register services to the Dependency injection container

// Register AutoMapper mapping profiles
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register Npgsql connection
builder.Services.AddTransient(implementationFactory =>
{
    string connectionString = builder.Configuration.GetConnectionString("Default");

    return new PostgresConnectionFactory(connectionString);
});

// Register repositories
builder.Services.AddTransient<IProductsRepository, ProductsRepository>();

builder.Services.AddGrpc(options => 
{
    // Register custom UnhandledExceptionInterceptor interceptor
    options.Interceptors.Add<UnhandledExceptionInterceptor>();
});

WebApplication app = builder.Build();

// Register gRPC services
app.MapGrpcService<GreeterService>();
app.MapGrpcService<ProductsService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
