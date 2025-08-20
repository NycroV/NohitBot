using DSharpPlus;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace NohitBot.Discord;

[UsedImplicitly]
public class Cache : BackgroundService
{
    public Cache(DiscordClient client) => source.SetResult(client);
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Delay(-1, stoppingToken);

    private static T Await<T>(Task<T> task) => task.GetAwaiter().GetResult();
    private static readonly TaskCompletionSource<DiscordClient> source = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public static readonly Random RNG = new();
    
    public static DiscordClient Client => Await(source.Task);

    public const ulong NycroID = 262663471189983242uL;
}