using Couchbase.Core.Exceptions.KeyValue;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TrainingProject.Api.Middleware
{
    public class ExceptionHandlingMiddleware : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = GetStatusCode(exception);

            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = GetStatusCode(exception),
                    Title = GetProblemDetailsTitle(exception),
                    Detail = GetProblemDetailsDetail(exception)
                },
                cancellationToken: cancellationToken);

            return true;
        }

        private static string GetProblemDetailsTitle(Exception exception) =>
            exception switch
            {
                ValidationException => "Validation Failed",
                DocumentNotFoundException => "Not Found",
                ArgumentException => "Bad Request",
                _ => "Internal Server Error"
            };

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                DocumentNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

        private static string? GetProblemDetailsDetail(Exception exception) =>
            exception switch
            {
                ValidationException validationException =>
                    string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage)),
                _ => null
            };
    }
}
