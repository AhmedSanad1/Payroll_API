using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Context;

namespace PayRollApi.Application.Logging
{
    public interface IApplicationLogger
    {
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
        void LogError(string message, Exception? ex = null, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogWithProperty(string propertyName, object propertyValue, string message, params object[] args);
    }

    public class ApplicationLogger(ILogger<ApplicationLogger> logger) : IApplicationLogger
    {
        public void LogInformation(string message, params object[] args)
        {
            Log.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            Log.Warning(message, args);
        }

        public void LogError(string message, Exception? ex = null, params object[] args)
        {
            Log.Error(ex, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            Log.Debug(message, args);
        }

        public void LogWithProperty(string propertyName, object propertyValue, string message, params object[] args)
        {
            using (LogContext.PushProperty(propertyName, propertyValue))
            {
                logger.LogInformation(message, args);
            }
        }
    }
}
