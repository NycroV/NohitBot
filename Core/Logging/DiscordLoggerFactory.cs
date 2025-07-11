using Microsoft.Extensions.Logging;

namespace NohitBot.Logging
{
    internal class DiscordLoggerFactory : ILoggerFactory
    {
        private List<ILoggerProvider> Providers { get; } = [];
        private bool isDisposed = false;

        public void AddProvider(ILoggerProvider provider) => Providers.Add(provider);

        public ILogger CreateLogger(string categoryName)
        {
            return isDisposed
                ? throw new InvalidOperationException("This logger factory is already disposed.")
                : new CompositeDiscordLogger(Providers);
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;

            foreach (ILoggerProvider provider in Providers)
                provider.Dispose();

            Providers.Clear();
        }
    }
}
