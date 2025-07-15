using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Utilities
{
    public static class LoggingHelper
    {
        public static void LogOperation(ILogger logger, string operation, int id)
        {
            logger.LogInformation("{Operation} performed with ID: {Id}", operation, id);
        }
    }
}
