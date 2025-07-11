using Microsoft.Extensions.Logging;

namespace NohitBot.Logging
{
    /// <summary>
    /// The DSharpPlus default logger, with some minor formatting tweaks
    /// </summary>
    internal sealed class DiscordLogger(string name, LogLevel minimumLogLevel) : ILogger
    {
        private readonly string name = name;
        private readonly LogLevel minimumLogLevel = minimumLogLevel;

        private const string logFile = "Log.txt";

        private static readonly FileStream Stream;
        public static readonly BinaryWriter Writer;
        private static readonly object logWriterLock;

        static DiscordLogger()
        {
            logWriterLock = new();
            Stream = File.Create(logFile);
            Writer = new(Stream);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= minimumLogLevel && logLevel != LogLevel.None;

        public void Log<TState>
        (
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            if (!IsEnabled(logLevel))
                return;

            lock (logWriterLock)
            {
                Console.ForegroundColor = logLevel switch
                {
                    LogLevel.Trace => ConsoleColor.Gray,
                    LogLevel.Debug => ConsoleColor.Green,
                    LogLevel.Information => ConsoleColor.Magenta,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    LogLevel.Critical => ConsoleColor.DarkRed,
                    _ => throw new ArgumentException("Invalid log level specified.", nameof(logLevel))
                };

                Write
                (
                    logLevel switch
                    {
                        LogLevel.Trace => "trace",
                        LogLevel.Debug => "debug",
                        LogLevel.Information => "info",
                        LogLevel.Warning => "warn",
                        LogLevel.Error => "error",
                        LogLevel.Critical => "crit",
                        _ => "This code path is unreachable."
                    },
                    logLevel
                );

                Console.ResetColor();

                Write($": [{name}]", logLevel);

                WriteLine("\n      " + formatter(state, exception), logLevel);

                if (exception != null)
                    WriteLine($"{exception} : {exception.Message}\n{exception.StackTrace}", logLevel);
            }
        }

        private static void Write(string content, LogLevel level)
        {
            Writer.Write(content);

            if (level > LogLevel.Trace)
                Console.Write(content);
        }

        private static void WriteLine(string content, LogLevel level)
        {
            Writer.Write(content + "\n");

            if (level > LogLevel.Trace)
                Console.WriteLine(content);
        }
    }
}