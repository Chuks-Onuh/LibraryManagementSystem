using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Logging
{
    public class CustomInMemoryLoggerProvider : ILoggerProvider
    {
        private readonly List<LogEntry> _logs = new();

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomInMemoryLogger(_logs);
        }

        public void Dispose() { }

        public IReadOnlyList<LogEntry> GetLogs() => _logs.AsReadOnly();
    }

    public class CustomInMemoryLogger : ILogger
    {
        private readonly List<LogEntry> _logs;
        private readonly string _categoryName;

        public CustomInMemoryLogger(List<LogEntry> logs)
        {
            _logs = logs;
        }

        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            _logs.Add(new LogEntry { Level = logLevel, Message = message, Timestamp = DateTime.UtcNow });
        }
    }

    public class LogEntry
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
