using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace NohitBot.Logging
{
    internal class DiscordLoggerProvider(LogLevel minimum = LogLevel.Trace) : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, DiscordLogger> loggers = new(StringComparer.Ordinal);
        private readonly LogLevel minimum = minimum;

        /// <inheritdoc/>
        public ILogger CreateLogger(string categoryName)
        {
            if (loggers.TryGetValue(categoryName, out DiscordLogger? value))
                return value;

            else
            {
                DiscordLogger logger = new(categoryName, minimum);

                return loggers.AddOrUpdate
                (
                    categoryName,
                    logger,
                    (_, _) => logger
                );
            }
        }

        public void Dispose() { }
    }
}
