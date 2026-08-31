using FluentValidation;
using HrSystem.Application.Exceptions;

namespace HrSystem.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled request failure. TraceId={TraceId} Path={Path}", context.TraceIdentifier, context.Request.Path);
            if (context.Response.HasStarted) throw;

            context.Response.Clear();
            context.Response.ContentType = "application/problem+json";
            var (status, title, detail) = ex switch
            {
                ValidationException validation => (400, "Validation failed", string.Join("; ", validation.Errors.Select(x => $"{x.PropertyName}: {x.ErrorMessage}"))),
                NotFoundException => (404, "Resource not found", ex.Message),
                ConcurrencyConflictException => (409, "Concurrency conflict", ex.Message),
                BusinessRuleException => (400, "Business rule violated", ex.Message),
                _ => (500, "Unexpected server error", "An unexpected server error occurred.")
            };
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { type = "about:blank", title, status, detail, traceId = context.TraceIdentifier });
        }
    }
}
