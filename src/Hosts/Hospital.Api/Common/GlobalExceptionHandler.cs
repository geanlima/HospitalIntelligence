using Hospital.Patients.Domain.Patients;
using Microsoft.AspNetCore.Diagnostics;

namespace Hospital.Api.Common;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "An unhandled exception occurred.");

        var statusCode =
            exception switch
            {
                PatientDomainException =>
                    StatusCodes.Status400BadRequest,

                ArgumentOutOfRangeException =>
                    StatusCodes.Status400BadRequest,

                _ =>
                    StatusCodes.Status500InternalServerError
            };

        httpContext.Response.StatusCode =
            statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                Status = statusCode,
                Error = exception.GetType().Name,
                Message = exception.Message
            },
            cancellationToken);

        return true;
    }
}