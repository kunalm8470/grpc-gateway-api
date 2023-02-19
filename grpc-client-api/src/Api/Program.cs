using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Api.ResponseProviders;
using Application.Products.v1.Queries;
using FluentValidation;
using Domain.Models.Requests.v1;
using Api.Middlewares;
using Grpc.Net.Client;
using Domain.Interfaces.RemoteProcedureServices;
using Infrastructure.RemoteProcedureServices;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

// Add versioning to our application
builder.Services.AddApiVersioning(options =>
{
    // Default version
    options.DefaultApiVersion = new ApiVersion(1, 0);

    /* 
     * Specify the default version, .NET 6 will attempt to
     * redirect all request to this version, if no
     * version is specified.
    */
    options.AssumeDefaultVersionWhenUnspecified = true;

    /*
     * Broadcast supported version in "api-supported-versions" response header, and
     * broadcast deprecated version in "api-deprecated-versions" response header.
    */
    options.ReportApiVersions = true;

    /* Versioning Strategy
     * 
     * API now supports Query Parameter versioning, Custom header api versioning
     * Accept Header versioning and Path based versioning
    */
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("v"),
        new MediaTypeApiVersionReader("version")
    );

    options.ErrorResponses = new ApiVersioningErrorResponseProvider();
});

// Register MediatR handlers
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblyContaining<GetProductsWithPaginationQuery>();
});

// Register IHttpContextAccessor for accessing HTTP context
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddSingleton<GrpcChannel>((serviceProvider) =>
{
    string channelUrl = builder.Configuration.GetValue<string>("ProductsServer:ChannelUrl");

    int maxSendMessageSize = builder.Configuration.GetValue<int>("ProductsServer:MaxSendMessageSize"); ;
    int maxRecieveMessageSize = builder.Configuration.GetValue<int>("ProductsServer:MaxRecieveMessageSize");

    int poolConnectionTimeoutMinutes = builder.Configuration.GetValue<int>("ProductsServer:PoolConnectionTimeoutMinutes");

    int maxRetryAttempts = builder.Configuration.GetValue<int>("ProductsServer:MaxRetryAttempts");
    int maxReconnectBackoffSeconds = builder.Configuration.GetValue<int>("ProductsServer:MaxReconnectBackoffSeconds");

    SocketsHttpHandler handler = new()
    {
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(poolConnectionTimeoutMinutes),
        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        EnableMultipleHttp2Connections = true
    };

    GrpcChannel channel = GrpcChannel.ForAddress(channelUrl, new GrpcChannelOptions
    {
        MaxReceiveMessageSize = maxRecieveMessageSize, // 5 MB
        MaxSendMessageSize = maxSendMessageSize, // 2 MB

        HttpHandler = handler,

        MaxRetryAttempts = maxRetryAttempts,
        MaxReconnectBackoff = TimeSpan.FromSeconds(maxReconnectBackoffSeconds)
    });

    return channel;
});

/*
 * Scan the assemblies via reflection and give entry point
 * Entry point here is GetProductsPaginationQuery
 * This helper method is found in FluentValidation.DependencyInjectionExtensions nuget package
*/
builder.Services.AddValidatorsFromAssemblyContaining<GetProductsPaginationQuery>(ServiceLifetime.Transient);

// Register AutoMapper mapping profiles
builder.Services.AddAutoMapper(typeof(ProductService).Assembly);

// Register gRPC service handler
builder.Services.AddTransient<IProductService, ProductService>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseMiddleware<UnhandledExceptionMiddleware>();

app.Run();
