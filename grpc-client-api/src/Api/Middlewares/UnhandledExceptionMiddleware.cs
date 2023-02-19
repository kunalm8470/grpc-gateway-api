using Domain.Constants;
using Domain.Extensions;

namespace Api.Middlewares;

public class UnhandledExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public UnhandledExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IWebHostEnvironment hostingEnvironment, ILogger<UnhandledExceptionMiddleware> logger)
    {
        string correlationId = context.Request.Headers[CorrelationIdConstants.CORRELATIONID_HEADER].FirstOrDefault();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ex.HandleException(
                context,
                logger,
                hostingEnvironment.IsDevelopment(),
                correlationId
            );
        }
    }
}
