using GlobalIdentityService.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace GlobalIdentityService
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            switch (exception)
            {
                case AlreadyExistsException:
                    await HandleAlreadyExistsExceptionAsync(httpContext, (AlreadyExistsException)exception, cancellationToken);
                    return true;

                case ValidationException:
                    await HandleValidationExceptionAsync(httpContext, (ValidationException)exception, cancellationToken);
                    return true;

                default:
                    return false;
            }
            
        }

        private async Task HandleAlreadyExistsExceptionAsync(HttpContext httpContext, AlreadyExistsException exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            var problem = new ProblemDetails
            {
                Title = $"{exception.Resource} already exists",
                Status = StatusCodes.Status409Conflict,
                Detail = "Enter another value"
            };
            problem.Extensions["fields"] = exception.Fields
                    .Select(field => new
                    {
                        name = field.Name,
                        value = field.Value
                    }).ToArray();
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            await httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json", cancellationToken);
        }

        private async Task HandleValidationExceptionAsync(HttpContext httpContext, ValidationException exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problem = new ProblemDetails
            {
                Title = $"Validation for {exception.FieldName} failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message
            };

            await httpContext.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json", cancellationToken);
        }
    }
}
