using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Api.ResponseProviders;

public class ApiVersioningErrorResponseProvider : DefaultErrorResponseProvider
{
    public override IActionResult CreateResponse(ErrorResponseContext context)
    {
        ProblemDetails problem = new()
        {
            Title = context.ErrorCode,
            Detail = context.Message,
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400",
            Instance = "https://github.com/microsoft/aspnet-api-versioning/wiki/Error-Response-Provider"
        };

        return new ObjectResult(problem)
        {
            ContentTypes =
            {
                "application/problem+json"
            },
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}
