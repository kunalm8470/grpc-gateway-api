using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Domain.Extensions;

public static class UnhandledExceptionExtensions
{
    public static async Task HandleException<T>(
        this Exception ex,
        HttpContext context,
        ILogger<T> logger,
        bool isDevelopmentEnvironment,
        string correlationId
    )
    {
        Task handleExceptionTask = ex switch
        {
            ProductNotFoundException => HandleProductNotFoundException((ProductNotFoundException)ex, context, logger, isDevelopmentEnvironment, correlationId),

            _ => HandleDefaultException(ex, context, logger, isDevelopmentEnvironment, correlationId)
        };

        await handleExceptionTask;
    }

    private static async Task HandleProductNotFoundException<T>(ProductNotFoundException exception, HttpContext context, ILogger<T> logger, bool isDevelopmentEnvironment, string correlationId)
    {
        logger.LogError(exception, "Product not found exception, CorrelationId: {correlationId}, Error: {errorMessage}", correlationId, exception.ToString());

        await HandleErrorGeneric(
            httpContext: context,
            exceptionType: exception.GetType().ToString(),
            isDevelopmentEnvironment,
            actualErrorMessage: exception.Message,
            customErrorMessage: "Product not found",
            statusCode: StatusCodes.Status404NotFound
        );
    }

    private static async Task HandleDefaultException<T>(Exception exception, HttpContext context, ILogger<T> logger, bool isDevelopmentEnvironment, string correlationId)
    {
        logger.LogError(exception, "Unhandled exception, CorrelationId: {correlationId}, Error: {errorMessage}", correlationId, exception.ToString());

        await HandleErrorGeneric(
            httpContext: context,
            exceptionType: exception.GetType().ToString(),
            isDevelopmentEnvironment,
            actualErrorMessage: exception.Message,
            customErrorMessage: "An error has occured we are looking into it.",
            statusCode: StatusCodes.Status500InternalServerError
        );
    }

    private static async Task HandleErrorGeneric(
        HttpContext httpContext,
        string exceptionType,
        bool isDevelopmentEnvironment,
        string actualErrorMessage,
        string customErrorMessage,
        int statusCode
    )
    {
        ProblemDetails details;

        /*
         *  Inspect ASPNETCORE_ENVIRONMENT environment variable
         *  looking at its value it will decide which environment it is
        */
        if (isDevelopmentEnvironment)
        {
            details = new ProblemDetails
            {
                Type = exceptionType,
                Detail = actualErrorMessage,
                Status = statusCode
            };
        }
        else
        {
            details = new ProblemDetails
            {
                Type = exceptionType,
                Detail = customErrorMessage,
                Status = statusCode
            };
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(details, Formatting.Indented));
    }
}
