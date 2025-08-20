using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using NohitBot.Database;
using NohitBot.Hosting;

namespace NohitBot.Commands.Admin;

public class Port
{
    [Command(nameof(Port))]
    [RequireApplicationOwner]
    public async ValueTask PortAsync(CommandContext ctx, string oldId, string newId)
    {
        if (!ulong.TryParse(oldId, out _))
        {
            await ctx.RespondAsync($"Error: `{oldId}` is not a valid user ID.");
            return;
        }

        if (!ulong.TryParse(newId, out _))
        {
            await ctx.RespondAsync($"Error: `{newId}` is not a valid user ID.");
            return;
        }

        DataBase.Save();
        string text = DataBase.FileText;

        if (!text.Contains(oldId))
        {
            await ctx.RespondAsync($"Error: `{oldId}` not found in nohit database.");
            return;
        }

        int count = text.Split(oldId).Length - 1;
        string newText = text.Replace(oldId, newId);
        
        DataBase.FileText = newText;
        await ctx.RespondAsync($"**{count}** instances of {oldId} replaced with {newId}.\n" +
                               $"Reloading...");
        
        await DiscordBotService.Host.StopAsync();
    }
}