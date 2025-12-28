using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using NohitBot.Commands.Info;
using NohitBot.Database;

namespace NohitBot.Commands.Judging;

public class JudgeRecomment
{
    [Command(nameof(JudgeRecomment))]
    [TextAlias("jrecomment", "jcomment", "jrc")]
    [Description("Updates the judge comment of a nohit without re-sending a DM to the nohitter")]
    [Help.JudgeHelp]
    [RequireGuild]
    public static async Task JudgeRecommentAsync(CommandContext ctx, uint nohitId, [RemainingText] string? comment = null)
    {
        if (!DataBase.DiscordConfigs.TryGetValue(ctx.Guild!.Id, out var config)) 
        {
            await ctx.RespondAsync("This server is not yet set up for configuration. Run `/setup` for setup!");
            return;
        }
        
        if (!config.JudgeIds.Contains(ctx.User.Id))
        {
            await ctx.RespondAsync("You are not a judge in this server!");
            return;
        }

        if (!DataBase.Nohits.TryGetValue(nohitId, out var nohit))
        {
            await ctx.RespondAsync("Invalid nohit ID!");
            return;
        }

        if (nohit.Verification.JudgeID != ctx.User.Id)
        {
            await ctx.RespondAsync("You did not review this nohit!");
            return;
        }

        if (string.IsNullOrWhiteSpace(comment))
            comment = "No judge comment";

        nohit.Verification.UpdateComment(comment);

        try
        {
            DiscordMember member = await ctx.Guild.GetMemberAsync(ctx.User.Id);
            DiscordDmChannel dmChannel = await member.CreateDmChannelAsync();

            var messages = dmChannel.GetMessagesAsync(100);
            await foreach (var message in messages)
            {
                if (message.Author!.Id != ctx.Client.CurrentUser.Id || !(message.Content?.StartsWith('[') ?? false))
                    continue;

                string reviewId = message.Content.Split('[')[1].Split(']')[0];
                uint messageNohitId = uint.Parse(reviewId);

                if (messageNohitId != nohit.ID)
                    continue;

                string updatedReviewMessage =
                    message.Content.Split('\n')[0] + (comment == "No judge comment" ? 
                        "No comment was left." : 
                        $"They also left a comment for you:\n\n\"{comment}\"");

                await message.ModifyAsync(content: updatedReviewMessage);
                await ctx.Channel.SendMessageAsync("DM message updated!");
                return;
            }

            await ctx.Channel.SendMessageAsync("This user has at least 100 messages after the initial review DM, so it cannot be updated.");
        }
        catch
        {
            await ctx.Channel.SendMessageAsync("Failed to update the user's DM notification.");
        }
    }
}