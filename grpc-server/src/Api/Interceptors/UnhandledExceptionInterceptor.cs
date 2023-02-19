using Api.Extensions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Api.Interceptors
{
    public class UnhandledExceptionInterceptor : Interceptor
    {
        private readonly ILogger<UnhandledExceptionInterceptor> _logger;

        public UnhandledExceptionInterceptor(ILogger<UnhandledExceptionInterceptor> logger)
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

            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex)
            {
                throw ex.Handle(context, _logger, correlationId);
            }
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation
        )
        {
            Guid correlationId = context.GetHttpContext().Request.Headers.TryParseCorrelationFromHttpContextRequestHeaders();

            try
            {
                return await continuation(requestStream, context);
            }
            catch (Exception ex)
            {
                throw ex.Handle(context, _logger, correlationId);
            }
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation
        )
        {
            Guid correlationId = context.GetHttpContext().Request.Headers.TryParseCorrelationFromHttpContextRequestHeaders();

            try
            {
                await continuation(request, responseStream, context);
            }
            catch (Exception ex)
            {
                throw ex.Handle(context, _logger, correlationId);
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation
        )
        {
            Guid correlationId = context.GetHttpContext().Request.Headers.TryParseCorrelationFromHttpContextRequestHeaders();

            try
            {
                await continuation(requestStream, responseStream, context);
            }
            catch (Exception ex)
            {
                throw ex.Handle(context, _logger, correlationId);
            }
        }
    }
}
