using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using NohitBot.Commands.Info;
using NohitBot.Database;
using NohitBot.DataStructures;

namespace NohitBot.Commands.Judging;

public class IgnoreJourney
{
    [Command(nameof(IgnoreJourney))]
    [TextAlias("ignore", "ij", "ignorej")]
    [Description("Prevents the bot from automatically reporting a user's complete journey")]
    [Help.JudgeHelp]
    [RequireGuild]
    public static async Task IgnoreJourneyAsync(CommandContext ctx, DiscordMember member, string difficultyCode)
    {
        if (!DataBase.DiscordConfigs.TryGetValue(ctx.Guild!.Id, out DiscordConfig? config))
        {
            await ctx.RespondAsync("This server is not yet set up for configuration. Run `/setup` for setup!");
            return;
        }

        if (!config.JudgeIds.Contains(ctx.User.Id))
        {
            await ctx.RespondAsync("You are not a judge in this server!");
            return;
        }

        if (!Difficulty.TryParse(difficultyCode, ctx.Guild!.Id, out var difficulty, out string? error))
        {
            await ctx.RespondAsync(error);
            return;
        }

        var journeys = DataBase.Journeys[member.Id];

        if (!journeys.TryGetValue(difficulty.Value, out Journey? journey))
        {
            await ctx.RespondAsync($"`@{member.Username}` has not submitted any nohits on \"{difficulty}\".");
            return;
        }

        if (config.ToggleIgnoreJourney(member.Id, journey))
            await ctx.RespondAsync($"`@{member.Username}`'s journey on \"{difficulty}\" is now being ignored.");

        else
            await ctx.RespondAsync($"`@{member.Username}`'s journey on \"{difficulty}\" is no longer being ignored.");

        await config.UpdateJourneyTrackingInfoPin();
    }
}