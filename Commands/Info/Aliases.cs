using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace NohitBot.Commands.Info;

public class Aliases
{
    [Command(nameof(Aliases))]
    [RequireGuild]
    public async ValueTask AliasesAsync(CommandContext ctx)
    {
        // TODO:
        // Alias display

        await ctx.RespondAsync("WIP!");
    }
}