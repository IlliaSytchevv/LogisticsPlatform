using System.Diagnostics;
using LogisticsPlatform.Domain.DTO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsPlatform.Middleware;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ExceptionMapping mapping = Map(exception);

        logger.LogError(exception, "Unhandled exception on {Method} {Path} → {StatusCode}",
            httpContext.Request.Method, httpContext.Request.Path, mapping.StatusCode);

        var problem = new ProblemDetails
        {
            Status = mapping.StatusCode,
            Title = mapping.Title,
            Detail = mapping.Detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.com/{mapping.StatusCode}",
        };

        problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (environment.IsDevelopment())
        {
            problem.Extensions["exception"] = exception.GetType().FullName;
            problem.Extensions["stackTrace"] = exception.StackTrace;
        }

        httpContext.Response.StatusCode = mapping.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }

    private static ExceptionMapping Map(Exception exception) =>
        exception switch
        {
            ArgumentNullException or ArgumentException =>
                new ExceptionMapping(StatusCodes.Status400BadRequest, "Bad Request", exception.Message),

            UnauthorizedAccessException =>
                new ExceptionMapping(StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),

            KeyNotFoundException =>
                new ExceptionMapping(StatusCodes.Status404NotFound, "Not Found", exception.Message),

            InvalidOperationException =>
                new ExceptionMapping(StatusCodes.Status409Conflict, "Conflict", exception.Message),

            NotImplementedException =>
                new ExceptionMapping(StatusCodes.Status501NotImplemented, "Not Implemented", exception.Message),

            OperationCanceledException =>
                new ExceptionMapping(499, "Request Cancelled", "The request was cancelled."),

            _ => new ExceptionMapping(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.")
        };
}
