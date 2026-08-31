using HrSystem.Application.Exceptions;

namespace HrSystem.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled request failure. TraceId={TraceId} Path={Path}", context.TraceIdentifier, context.Request.Path);
            if (context.Response.HasStarted) throw;
            context.Response.Clear(); context.Response.ContentType = "application/problem+json";
            var (status, title) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                BusinessRuleException => (StatusCodes.Status400BadRequest, "Business rule violated"),
                _ => (StatusCodes.Status500InternalServerError, "Unexpected server error")
            };
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { type = "about:blank", title, status, detail = status == 500 ? "An unexpected error occurred." : ex.Message, traceId = context.TraceIdentifier });
        }
    }
}
