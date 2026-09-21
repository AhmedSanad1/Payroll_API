using Serilog.Context;

namespace PayRollApi.Logging
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private const string CorrelationIdProperty = "CorrelationId";

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetOrCreateCorrelationId(context);
            
            // Echo it back so the client can quote it when reporting a problem.
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
                return Task.CompletedTask;
            });

            // Everything logged inside this scope gets tagged with the id.
            using (LogContext.PushProperty(CorrelationIdProperty, correlationId))
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} started - CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    correlationId);

                await _next(context);

                _logger.LogInformation(
                    "HTTP {Method} {Path} completed with status code {StatusCode} - CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    correlationId);
            }
        }

        private string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationIdValue))
            {
                return correlationIdValue.ToString();
            }

            // Caller didn't send one, so fall back to ASP.NET's own id.
            var correlationId = context.TraceIdentifier;
            context.Items[CorrelationIdProperty] = correlationId;
            return correlationId;
        }
    }
}
