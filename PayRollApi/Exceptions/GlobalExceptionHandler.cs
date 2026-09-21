using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PayRollApi.Application.Common;
using PayRollApi.Application.Interfaces;

namespace PayRollApi.Exceptions
{
    internal sealed class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception occured");

            var localizer = httpContext.RequestServices.GetService(typeof(ILocalizer)) as ILocalizer;
            string L(string key, string fallback) => localizer?.Get(key) ?? fallback;

            var statusCode = exception switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                DbUpdateException => HttpStatusCode.BadRequest,
                JsonException => HttpStatusCode.BadRequest,
                FormatException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            httpContext.Response.StatusCode = (int)statusCode;

            // Never leak exception details to the client — it's all in the logs.
            var response = new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = L("Error_Unexpected", "An error occurred."),
                Errors = null
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}
