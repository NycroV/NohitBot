using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NohitBot.DataStructures;
using NohitBot.Database;
using NohitBot.Hosting;

namespace NohitBot.Commands.Config;

public class MyProfile(DiscordClient client) : DiscordEventHandler<ClientStartedEventArgs>
{
    private static DiscordComponentEmoji? JudgeEmoji { get; set; } = null;
    
    public override Task HandleAsync(ClientStartedEventArgs args)
    {
        JudgeEmoji ??= new(DiscordEmoji.FromName(client, ""));
        return Task.CompletedTask;
    }
    
    [Command(nameof(MyProfile))]
    [Description("Allows you to edit your nohit judge presence.")]
    [RequireGuild]
    public static async ValueTask MyProfileAsync(CommandContext ctx)
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

        var message = new DiscordMessageBuilder()
            .EnableV2Components()
            .AddContainerComponent(new([
                new DiscordTextDisplayComponent("## Configure Judge Profile"),
                new DiscordTextDisplayComponent("Use the button below to configure your \"judge name,\" aliases, and journey completion message."),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "judge_profile", "", false, JudgeEmoji!)
            ]));
        
        await ctx.RespondAsync(message);
    }

    [InteractionResponse("judge_profile")]
    public static async ValueTask DeliverModalAsync(ComponentInteractionCreatedEventArgs args)
    {
        var config = DataBase.DiscordConfigs[args.Guild.Id];
        
        if (!config.JudgeIds.Contains(args.User.Id))
        {
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
            return;
        }
        
        JudgeProfile judge = DataBase.Judges[args.User.Id];

        var modal = new DiscordModalBuilder()
            .WithTitle("Judge Profile")
            .WithCustomId("judge_modal")
            .AddTextInput(new(
                    "name", "username", judge.Name),
                "Name",
                "The default name to use for things like requesting reviews.")
            .AddTextInput(new(
                "journeyMessage", "do something cool", judge.JourneyMessage),
                "Journey Completion Message",
                $"The text that displays when you are randomly chosen for someone's journey completion message.\n\"Now to wait 10 years for {judge.Name} to...\"")
            .AddTextInput(new(
                    "aliases", "name,nickname,codename", string.Join(',', judge.Aliases)),
                "Aliases",
                "The alternative aliases to use for things like requesting reviews, separate by commas. ( , )");

        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, modal);
    }

    [InteractionResponse("judge_modal")]
    public static async ValueTask ReceiveModalAsync(ModalSubmittedEventArgs args)
    {
        string Response(string key) => (args.Values[key] as TextInputModalSubmission)!.Value;
        
        JudgeProfile judge = DataBase.Judges[args.Interaction.User.Id];
        judge.Update(
            Response("name"),
            Response("journeyMessage"),
            Response("aliases").Split(',').Select(s => s.Trim()));
        
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
    }
}