using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using NohitBot.Commands.Info;
using NohitBot.Database;
using NohitBot.DataStructures;

namespace NohitBot.Commands.Judging;

public class Queue
{
    [Command(nameof(Queue))]
    [TextAlias("q", "journeyqueue", "journeyq")]
    [Description("Displays the completed journey queue for the current server.")]
    [Help.AllHelp]
    [RequireGuild]
    public static async Task QueueAsync(CommandContext ctx)
    {
        if (!DataBase.DiscordConfigs.TryGetValue(ctx.Guild!.Id, out DiscordConfig? config))
        {
            await ctx.RespondAsync("This server is not yet set up for configuration. Run `/setup` for setup!");
            return;
        }

        var journeyQueue = config.JourneyQueue;

        if (journeyQueue.Count == 0)
        {
            await ctx.RespondAsync("Completed journey queue is empty!");
            return;
        }

        DiscordEmbedBuilder embed = new DiscordEmbedBuilder().WithTitle($"Completed Journey Queue for {ctx.Guild.Name}:");
        var description = string.Empty;

        for (var i = 0; i < journeyQueue.Count; i++)
        {
            Journey journey = journeyQueue[i];
            description += $"{i + 1}. - <@!{journey.UserID}> / {journey.Difficulty}\n";
        }

        description = description.Remove(description.Length - 1);
        embed.WithDescription(description);

        await ctx.RespondAsync(embed);
    }
}