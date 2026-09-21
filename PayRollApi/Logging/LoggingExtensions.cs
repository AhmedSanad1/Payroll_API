using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;

namespace PayRollApi.Logging
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddStructuredLogging(this WebApplicationBuilder builder)
        {
            var isDevelopment = builder.Environment.IsDevelopment();
            var applicationName = builder.Environment.ApplicationName;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(isDevelopment ? LogEventLevel.Debug : LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    theme: isDevelopment ? Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code : null,
                    outputTemplate: isDevelopment
                        ? "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                        : "{Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine("Logs", applicationName, ".txt"),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30)
                .WriteTo.Async(a => a.File(
                    new ElasticsearchJsonFormatter(),
                    path: Path.Combine("Logs", applicationName, "structured-", ".json"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30))
                .CreateLogger();

            builder.Host.UseSerilog();
            builder.Services.AddSingleton(Log.Logger);

            return builder;
        }

        public static WebApplication UseCorrelationIdMiddleware(this WebApplication app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();
            return app;
        }
    }
}
