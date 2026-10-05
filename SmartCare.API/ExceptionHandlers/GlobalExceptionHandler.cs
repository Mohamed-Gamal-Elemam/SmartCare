using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Shared.Responses;
using System.Net;

namespace SmartCare.API.ExceptionHandlers;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An exception occurred while processing request: {Path}", httpContext.Request.Path);

        var statusCode = exception switch
        {
            BadRequestException => HttpStatusCode.BadRequest,
            UnauthorizedException => HttpStatusCode.Unauthorized,
            ForbiddenException => HttpStatusCode.Forbidden,
            NotFoundException => HttpStatusCode.NotFound,
            ConflictException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        var message = GetDetail(exception, statusCode);
        var response = ApiResponse<object>.Fail(message);

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private string GetDetail(Exception exception, HttpStatusCode statusCode)
    {
        if (statusCode != HttpStatusCode.InternalServerError)
            return exception.Message;

        return _environment.IsDevelopment()
            ? exception.Message
            : "An unexpected internal server error occurred. Please try again later.";
    }
}