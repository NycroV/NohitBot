using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using NohitBot.Hosting;

namespace NohitBot.Commands.Admin;

public class Reload
{
    [Command(nameof(Reload))]
    [RequireApplicationOwner]
    public async ValueTask ReloadAsync(CommandContext ctx)
    {
        await ctx.RespondAsync("Reloading...");
        await DiscordBotService.Host.StopAsync();
    }
}