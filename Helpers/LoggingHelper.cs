using Microsoft.Extensions.Logging;
using System;

namespace MR_power.Helpers
{
    public static class LoggingHelper
    {
        public static void LogError(ILogger logger, Exception ex, string? message = null)
        {
            logger.LogError(ex, "{Message} - {StackTrace}", message ?? ex.Message, ex.StackTrace);
        }

        public static void LogWarning(ILogger logger, string message, params object[] args)
        {
            logger.LogWarning(message, args);
        }

        public static void LogInformation(ILogger logger, string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }

        public static void LogDebug(ILogger logger, string message, params object[] args)
        {
            logger.LogDebug(message, args);
        }

        public static void LogCritical(ILogger logger, Exception ex, string? message = null)
        {
            logger.LogCritical(ex, "{Message} - {StackTrace}", message ?? ex.Message, ex.StackTrace);
        }
    }
} 