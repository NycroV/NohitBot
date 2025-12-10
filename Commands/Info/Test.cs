using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using NohitBot.Hosting;

namespace NohitBot.Commands.Info;

public class Test
{
    [Command(nameof(Test))]
    [RequireGuild]
    public async ValueTask TestAsync(CommandContext ctx)
    {
        TimeSpan uptime = DateTime.UtcNow - DiscordBotService.StartupTime;
        int ping = (int)Math.Round(ctx.Client.GetConnectionLatency(ctx.Guild!.Id).TotalMilliseconds);

        await ctx.RespondAsync(
            "NohitBot Stats:\n" +
                   "\n" +
                   $"Uptime: {uptime.Days}d, {uptime.Hours}h, {uptime.Minutes}m, {uptime.Seconds}s\n" +
                   $"Latency: {ping}ms");
    }
}