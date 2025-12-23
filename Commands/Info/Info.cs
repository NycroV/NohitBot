using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Entities;

namespace NohitBot.Commands.Info;

public class Info
{
    [Command(nameof(Info))]
    [Description("Info about the NohitBot.")]
    public static async ValueTask InfoAsync(CommandContext ctx)
    {
        var embed = new DiscordEmbedBuilder()
            .WithColor(DiscordColor.Aquamarine)
            .WithTitle("NohitBot v3")
            .WithDescription("Developed and maintained by @nycro.\nReach out for bug reports, feature requests, or questions!")
            .WithFooter()
            .WithThumbnail();
        
        await ctx.RespondAsync(embed);
    }
}