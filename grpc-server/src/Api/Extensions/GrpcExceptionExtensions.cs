using Domain.Constants;
using Domain.Exceptions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;

namespace Api.Extensions
{
    public static class GrpcExceptionExtensions
    {
        public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            return exception switch
            {
                TimeoutException => HandleTimeoutException((TimeoutException)exception, context, logger, correlationId),

                ProductNotFoundException => HandleProductNotFoundException((ProductNotFoundException)exception, context, logger, correlationId),

                RpcException => HandleRpcException((RpcException)exception, logger, correlationId),

                _ => HandleDefaultException(exception, context, logger, correlationId)
            };
        }

        private static RpcException HandleProductNotFoundException<T>(ProductNotFoundException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            logger.LogError(exception, "CorrelationId: {correlationId}, Error: {errorMessage}", correlationId, exception.Message);

            Status status = new(StatusCode.NotFound, exception.Message);

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleTimeoutException<T>(TimeoutException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            logger.LogError(exception, "CorrelationId: {correlationId} - A timeout occurred", correlationId);

            Status status = new(StatusCode.Internal, "An external resource did not answer within the time limit");

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleRpcException<T>(RpcException exception, ILogger<T> logger, Guid correlationId)
        {
            logger.LogError(exception, "CorrelationId: {correlationId} - An error occurred", correlationId);

            Metadata trailers = exception.Trailers;

            trailers.Add(CreateTrailers(correlationId)[0]);

            return new RpcException(new Status(exception.StatusCode, exception.Message), trailers);
        }

        private static RpcException HandleDefaultException<T>(Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            logger.LogError(exception, "CorrelationId: {correlationId} - An error occurred", correlationId);

            return new RpcException(new Status(StatusCode.Internal, exception.Message), CreateTrailers(correlationId));
        }

        /// <summary>
        ///  Adding the correlation to Response Trailers
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private static Metadata CreateTrailers(Guid correlationId)
        {
            Metadata trailers = new()
            {
                { "CorrelationId", correlationId.ToString() }
            };

            return trailers;
        }
    }
}
