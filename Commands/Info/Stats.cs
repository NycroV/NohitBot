using DSharpPlus.Commands;

namespace NohitBot.Commands.Info;

public class Stats
{
    [Command(nameof(Stats))]
    public async ValueTask StatsAsync(CommandContext ctx)
    {
        // TODO:
        // Nohit stats command

        await ctx.RespondAsync("WIP!");
    }
}