using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using NohitBot.Database;

namespace NohitBot.Commands.Admin;

public class ResetConfig
{
    [Command(nameof(ResetConfig))]
    [RequireApplicationOwner]
    public static async Task DestroyConfigAsync(CommandContext ctx, string guildId)
    {
        if (!ulong.TryParse(guildId, out ulong id))
        {
            await ctx.RespondAsync("Please input the id of the server you want to reset the config for.");
            return;
        }

        if (DataBase.DiscordConfigs.Remove(id))
        {
            DataBase.Save();
            await ctx.RespondAsync("Server config reset.");
            return;
        }
        
        await ctx.RespondAsync("No server config found.");
    }
}