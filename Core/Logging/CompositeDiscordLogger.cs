using DSharpPlus;
using Microsoft.Extensions.Logging;

namespace NohitBot.Logging
{
    internal class CompositeDiscordLogger(IEnumerable<ILoggerProvider> providers) : ILogger
    {
        private IEnumerable<ILogger> Loggers { get; } = providers.Select(x => x.CreateLogger(typeof(BaseDiscordClient).FullName!)).ToList();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>
        (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            foreach (ILogger logger in Loggers)
                logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
            => throw new NotImplementedException();
    }
}
