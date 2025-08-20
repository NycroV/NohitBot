using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using NohitBot.Hosting;

namespace NohitBot.Commands.Admin;

public class Debug
{
    [Command(nameof(Debug))]
    [RequireApplicationOwner]
    public static async ValueTask DebugAsync(CommandContext ctx)
    {
        DiscordBotService.Debug = !DiscordBotService.Debug;
        await ctx.RespondAsync($"Debug Toggled: `{DiscordBotService.Debug}`");
    }
}