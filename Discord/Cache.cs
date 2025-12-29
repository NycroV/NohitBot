using DSharpPlus;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace NohitBot.Discord;

[UsedImplicitly]
public class Cache : BackgroundService
{
    public const ulong NycroID = 262663471189983242uL;

    private static readonly TaskCompletionSource<DiscordClient> source = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public static readonly Random RNG = new();

    public Cache(DiscordClient client)
    {
        source.SetResult(client);
    }

    public static DiscordClient Client => Await(source.Task);

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Delay(-1, stoppingToken);
    }

    private static T Await<T>(Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }
}