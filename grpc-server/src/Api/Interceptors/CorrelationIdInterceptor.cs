using Grpc.Core;
using Grpc.Core.Interceptors;
using System.Threading.Tasks;
using System;
using Api.Extensions;
using Domain.Constants;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.Interceptors;

public class CorrelationIdInterceptor : Interceptor
{
    private readonly ILogger<CorrelationIdInterceptor> _logger;

    public CorrelationIdInterceptor(ILogger<CorrelationIdInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
           TRequest request,
           ServerCallContext context,
           UnaryServerMethod<TRequest, TResponse> continuation
       )
    {
        Guid correlationId = context.GetHttpContext().Request.Headers.TryParseCorrelationFromHttpContextRequestHeaders();

        _logger.LogInformation("Hostname: {host}, Method: {method}, CorrelationId: {correlationId}, Headers: {headers}", context.Host, context.Method, correlationId.ToString(), JsonConvert.SerializeObject(context.RequestHeaders));

        await context.WriteResponseHeadersAsync(new Metadata
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, correlationId.ToString() }
        });

        return await continuation(request, context);
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation
    )
    {
        Guid correlationId = context.GetHttpContext().Request.Headers.TryParseCorrelationFromHttpContextRequestHeaders();

        _logger.LogInformation("Hostname: {host}, Method: {method}, CorrelationId: {correlationId}, Headers: {headers}", context.Host, context.Method, correlationId.ToString(), JsonConvert.SerializeObject(context.RequestHeaders));

        await context.WriteResponseHeadersAsync(new Metadata
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, correlationId.ToString() }
        });

        return await continuation(requestStream, context);
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation
    )
    {
        Guid correlationId = context.GetHttpContext().Request.Headers.TryParseCorrelationFromHttpContextRequestHeaders();

        _logger.LogInformation("Hostname: {host}, Method: {method}, CorrelationId: {correlationId}, Headers: {headers}", context.Host, context.Method, correlationId.ToString(), JsonConvert.SerializeObject(context.RequestHeaders));

        await context.WriteResponseHeadersAsync(new Metadata
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, correlationId.ToString() }
        });

        await continuation(request, responseStream, context);
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation
    )
    {
        Guid correlationId = context.GetHttpContext().Request.Headers.TryParseCorrelationFromHttpContextRequestHeaders();

        _logger.LogInformation("Hostname: {host}, Method: {method}, CorrelationId: {correlationId}, Headers: {headers}", context.Host, context.Method, correlationId.ToString(), JsonConvert.SerializeObject(context.RequestHeaders));

        await context.WriteResponseHeadersAsync(new Metadata
        {
            { CorrelationIdConstants.CORRELATIONID_HEADER, correlationId.ToString() }
        });

        await continuation(requestStream, responseStream, context);
    }
}
