using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using NohitBot.Utilities;

namespace NohitBot.Commands.Info;

public class Help
{
    [Command(nameof(Help))]
    [Description("Lists the available commands.")]
    public async ValueTask HelpAsync(CommandContext ctx)
    {
        var message = new DiscordMessageBuilder()
            .EnableV2Components()
            .AddContainerComponent(
                new DiscordContainerComponent([
                    new DiscordTextDisplayComponent(""),
                    new DiscordActionRowComponent(
                        [new DiscordSelectComponent("commandName", "Help", [])]
                    ),
                    new DiscordActionRowComponent([
                        new DiscordButtonComponent(DiscordButtonStyle.Primary, "allhelp", "All"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "judgehelp", "Judge"),
                        new DiscordButtonComponent(DiscordButtonStyle.Secondary, "adminhelp", "Admin"),
                    ])
                ], false, null
            ));

        await ctx.RespondAsync(message, true);
    }
}