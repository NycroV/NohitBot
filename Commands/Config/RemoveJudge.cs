using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using NohitBot.Database;

namespace NohitBot.Commands.Config;

public class RemoveJudge
{
    [Command(nameof(RemoveJudge))]
    [Description("Removes a user from the server judges list.")]
    [RequireGuild]
    [RequirePermissions(DiscordPermission.Administrator)]
    public async ValueTask RemoveJudgeAsync(CommandContext ctx, DiscordMember user)
    {
        if (!DataBase.DiscordConfigurations.TryGetValue(ctx.Guild!.Id, out var config))
        {
            await ctx.RespondAsync("This server is not yet set up for configuration. Run `/setup` for setup!");
            return;
        }

        if (config.RemoveJudge(user.Id))
            await ctx.RespondAsync("Judge removed!");
        
        else
            await ctx.RespondAsync("Specified user is not a judge.");
    }
}