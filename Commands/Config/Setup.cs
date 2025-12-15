using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NohitBot.Database;
using NohitBot.Hosting;

namespace NohitBot.Commands.Config;

public class Setup
{
    [Command(nameof(Setup))]
    [Description("Sets up/configures settings for the server.")]
    [RequireGuild]
    [RequirePermissions(DiscordPermission.Administrator)]
    public async ValueTask SetupAsync(CommandContext ctx)
    {
        if (!DataBase.DiscordConfigurations.TryGetValue(ctx.Guild!.Id, out var config))
        {
            var setupMessage = new DiscordMessageBuilder()
                .AddContainerComponent(new DiscordContainerComponent([
                    new DiscordTextDisplayComponent("## NohitBot v3"),
                    new DiscordMediaGalleryComponent([]),
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "INITIAL_SETUP", "Run Setup")
                ]));

            await ctx.RespondAsync(setupMessage);
        }
    }

    [InteractionResponse("INITIAL_SETUP")]
    public static async Task DeliverModalAsync(ComponentInteractionCreatedEventArgs args)
    {
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, null!);
    }

    [InteractionResponse("SETUP_RESPONSE")]
    public static async Task ReceiveModalAsync(ModalSubmittedEventArgs args)
    {
        await args.Interaction.Message!.ModifyAsync();
    }
}