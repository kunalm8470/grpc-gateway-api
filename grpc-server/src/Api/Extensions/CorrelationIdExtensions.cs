using Domain.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Api.Extensions;

public static class CorrelationIdExtensions
{
    public static Guid TryParseCorrelationFromHttpContextRequestHeaders(this IHeaderDictionary headers)
    {
        return headers.ContainsKey(CorrelationIdConstants.CORRELATIONID_HEADER) && headers[CorrelationIdConstants.CORRELATIONID_HEADER].Any()
        ? Guid.Parse(headers[CorrelationIdConstants.CORRELATIONID_HEADER].FirstOrDefault())
        : Guid.NewGuid();
    }
}
