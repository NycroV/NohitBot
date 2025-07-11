using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace NohitBot.Hosting
{
    [UsedImplicitly]
    public class DiscordBotService(DiscordClient client) : BackgroundService
    {
        public DiscordClient Client { get; init; } = client;

        public static DateTime StartupTime { get; private set; }

        public static DiscordIntents Intents => DiscordIntents.All;
        
        private const string TokenFile = "Token.txt";

        public static string Token => File.ReadAllText(TokenFile);

        public static readonly SemaphoreSlim CommandAccess = new(1, 1);

        [UsedImplicitly]
        public class OneCommandAtATime : IContextCheck<UnconditionalCheckAttribute>
        {
            // Ensure only one command can execute at a time to prevent over-writing submissions
            public async ValueTask<string?> ExecuteCheckAsync(UnconditionalCheckAttribute attribute, CommandContext context)
            {
                await context.DeferResponseAsync();
                await CommandAccess.WaitAsync();
                return null;
            }

            // Return semaphore access to re-open command execution
            public static Task CompleteCommandAsync()
            {
                CommandAccess.Release();
                return Task.CompletedTask;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Client.ConnectAsync();
            StartupTime = DateTime.UtcNow;
            await Task.Delay(-1, stoppingToken);
        }
    }
}
