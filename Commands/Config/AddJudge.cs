using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using NohitBot.Database;

namespace NohitBot.Commands.Config;

public class AddJudge
{
    [Command(nameof(AddJudge))]
    [Description("Adds a user to the server judges list.")]
    [RequireGuild]
    public async ValueTask AddJudgeAsync(CommandContext ctx, DiscordMember user)
    {
        if (!ctx.Member!.Permissions.HasPermission(DiscordPermission.Administrator))
        {
            await ctx.RespondAsync("You don't have permission to do that.");
            return;
        }

        if (!DataBase.DiscordConfigurations.TryGetValue(ctx.Guild!.Id, out var config))
        {
            await ctx.RespondAsync("This server is not yet set up for configuration. Contact \\@nycro for setup!");
            return;
        }

        if (config.JudgeIDs.Contains(user.Id))
        {
            await ctx.RespondAsync("This user is already a judge.");
            return;
        }
        
        config.JudgeIDs.Add(user.Id);
        await ctx.RespondAsync("Judge added!");
    }
}