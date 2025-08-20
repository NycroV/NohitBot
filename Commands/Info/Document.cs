using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace NohitBot.Commands.Info;

public class Document
{
    [Command(nameof(Document))]
    [RequireGuild]
    public async ValueTask DocumentAsync(CommandContext ctx)
    {
        // TODO:
        // Document retrieval

        await ctx.RespondAsync("WIP!");
    }
}