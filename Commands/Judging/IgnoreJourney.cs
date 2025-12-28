using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using NohitBot.Commands.Info;

namespace NohitBot.Commands.Judging;

public class IgnoreJourney
{
    [Command(nameof(IgnoreJourney))]
    [TextAlias("ignore", "ij", "ignorej")]
    [Description("Prevents the bot from automatically reporting a user's complete journey")]
    [Help.JudgeHelp]
    [RequireGuild]
    public static async Task IgnoreJourneyAsync(CommandContext ctx, DiscordMember member, string difficulty)
    {
        
    }
}