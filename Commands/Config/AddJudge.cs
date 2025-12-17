using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using NohitBot.Data;
using NohitBot.Database;

namespace NohitBot.Commands.Config;

public class AddJudge
{
    [Command(nameof(AddJudge))]
    [Description("Adds a user to the server judges list.")]
    [RequireGuild]
    [RequirePermissions(DiscordPermission.Administrator)]
    public static async ValueTask AddJudgeAsync(CommandContext ctx, DiscordMember user)
    {
        if (!DataBase.DiscordConfigs.TryGetValue(ctx.Guild!.Id, out var config))
        {
            await ctx.RespondAsync("This server is not yet set up for configuration. Run `/setup` for setup!");
            return;
        }

        if (config.JudgeIds.Contains(user.Id))
        {
            await ctx.RespondAsync("This user is already a judge.");
            return;
        }
        
        config.AddJudge(user.Id);
        JudgeProfile.Make(user.Id, user.Username, "set up their judge profile");
        
        await ctx.RespondAsync("Judge added!");
        await config.UpdateJudgeInfoPin();
    }
}