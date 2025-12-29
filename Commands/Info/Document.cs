using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using NohitBot.Database;
using NohitBot.DataStructures;

namespace NohitBot.Commands.Info;

public class Document
{
    [Command(nameof(Document))]
    [Description("Links to this server's nohit rules document.")]
    [Help.AllHelp]
    [RequireGuild]
    public static async ValueTask DocumentAsync(CommandContext ctx)
    {
        if (!DataBase.DiscordConfigs.TryGetValue(ctx.Guild!.Id, out DiscordConfig? config))
        {
            await ctx.RespondAsync("This server is not yet set up for configuration. Run `/setup` for setup!");
            return;
        }

        if (config.DocMessage is null)
        {
            await ctx.RespondAsync("No doc message configured. Ping an admin!");
            return;
        }

        await ctx.RespondAsync(config.DocMessage);
    }
}