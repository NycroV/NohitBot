using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NohitBot.Data;
using NohitBot.Database;
using NohitBot.Hosting;
using NohitBot.Utilities;

namespace NohitBot.Commands.Config;

public class Setup
{
    [Command(nameof(Setup))]
    [Description("Sets up/configures settings for the server.")]
    [RequireGuild]
    [RequirePermissions(DiscordPermission.Administrator)]
    public async ValueTask SetupAsync(CommandContext ctx)
    {
        if (!DataBase.DiscordConfigs.TryGetValue(ctx.Guild!.Id, out var config))
        {
            var setupMessage = new DiscordMessageBuilder()
                .EnableV2Components()
                .AddContainerComponent(new([
                    new DiscordTextDisplayComponent("## NohitBot v3"),
                    new DiscordTextDisplayComponent("Developed by @nycro"),
                    //new DiscordMediaGalleryComponent([]),
                    new DiscordActionRowComponent([new DiscordButtonComponent(DiscordButtonStyle.Primary, "INITIAL_SETUP", "Run Setup")])
                ]));

            await ctx.RespondAsync(setupMessage);
            return;
        }

        DiscordMessageBuilder message = await CreateSetupMessageAsync(ctx.Guild, config);
        await ctx.RespondAsync(message);
    }

    [InteractionResponse("INITIAL_SETUP")]
    public static async Task DeliverModalAsync(ComponentInteractionCreatedEventArgs args)
    {
        var member = args.User as DiscordMember;

        if (!member!.Permissions.HasPermission(DiscordPermission.Administrator))
        {
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
            return;
        }

        var modal = new DiscordModalBuilder()
            .WithTitle("Server Setup")
            .WithCustomId($"SETUP_RESPONSE_{args.Channel.Id}/{args.Message.Id}")
            .AddSelectMenu(
                new DiscordChannelSelectComponent("setup_submission", "#nohit-submissions", [DiscordChannelType.Text]),
                "Submission Channel",
                "The channel where nohits should be submitted.")
            .AddSelectMenu(
                new DiscordChannelSelectComponent("setup_log", "#nohit-logs", [DiscordChannelType.Text]),
                "Log Channel",
                "The channel where nohit submissions should be logged.")
            .AddSelectMenu(
                new DiscordChannelSelectComponent("setup_journey", "#journey-logs", [DiscordChannelType.Text]),
                "Journey Channel",
                "The channel where completed nohit journeys should be logged.");

        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, modal);
    }

    [InteractionResponse("SETUP_RESPONSE")]
    public static async Task ReceiveModalAsync(ModalSubmittedEventArgs args)
    {
        ulong ChannelId(string customId) => (args.Values[customId] as ChannelSelectMenuModalSubmission)!.Ids[0];
        
        ulong submissions = ChannelId("setup_submission");
        ulong logs = ChannelId("setup_log");
        ulong journey = ChannelId("setup_journey");
        
        DiscordConfig config = DiscordConfig.Make(args.Interaction.GuildId!.Value, submissions, logs, journey);
        var messageBuilder = await CreateSetupMessageAsync(args.Interaction.Guild!, config);

        string setupId = args.Interaction.Data.CustomId.Split('_')[^1].Trim();
        string[] setupComponents = setupId.Split('/');
        
        ulong channelId = ulong.Parse(setupComponents[0]);
        ulong messageId = ulong.Parse(setupComponents[1]);
        
        DiscordChannel channel = await args.Interaction.Guild!.GetChannelAsync(channelId);
        DiscordMessage message = await channel.GetMessageAsync(messageId);
        
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
        await message.ModifyAsync(messageBuilder);
    }

    private static async Task<DiscordMessageBuilder> CreateSetupMessageAsync(DiscordGuild guild, DiscordConfig config)
    {
        string submission = (await guild.GetChannelSafeAsync(config.SubmissionChannelId))?.Name ?? "nohit-submissions";
        string log = (await guild.GetChannelSafeAsync(config.LogChannelId))?.Name ?? "nohit-logs";
        string journey = (await guild.GetChannelSafeAsync(config.JourneyChannelId))?.Name ?? "nohit-journeys";
            
        // Submission -> Log -> Journey
        return new DiscordMessageBuilder()
            .EnableV2Components()
            .AddContainerComponent(new([
                new DiscordTextDisplayComponent("## Submissions Channel\nThe channel where nohits should be submitted."),
                new DiscordActionRowComponent([new DiscordChannelSelectComponent("setup_channel_submission", $"#{submission}", [DiscordChannelType.Text])]),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent("## Log Channel\nThe channel where nohit submissions should be logged."),
                new DiscordActionRowComponent([new DiscordChannelSelectComponent("setup_channel_log", $"#{log}", [DiscordChannelType.Text])]),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordTextDisplayComponent("## Journey Channel\nThe channel where completed nohit journeys should be logged."),
                new DiscordActionRowComponent([new DiscordChannelSelectComponent("setup_channel_journey", $"#{journey}", [DiscordChannelType.Text])]),
                new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                new DiscordActionRowComponent([
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "setup_pin_judgeinfo", "Setup Judge Info"),
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "setup_pin_journeytracking", "Setup Journey Tracking"),
                ])
            ]));
    }

    [InteractionResponse("setup_channel")]
    public static async Task ReceiveConfigUpdateAsync(ComponentInteractionCreatedEventArgs args)
    {
        DiscordConfig config = DataBase.DiscordConfigs[args.Guild.Id];
        string channel = args.Interaction.Data.CustomId.Replace("setup_channel_", "");
        ulong id = ulong.Parse(args.Interaction.Data.Values[0]);

        switch (channel)
        {
            case "submission": config.SetChannels(submissionId: id); break;
            case "log": config.SetChannels(logId: id); break;
            case "journey": config.SetChannels(journeyId: id); break;
        }
        
        var message = new DiscordInteractionResponseBuilder(await CreateSetupMessageAsync(args.Guild, config));
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, message);
    }

    [InteractionResponse("setup_pin")]
    public static async Task ReceivePinUpdateAsync(ComponentInteractionCreatedEventArgs args)
    {
        string pinType = args.Interaction.Data.CustomId.Split('_')[^1].Trim();
        var modal = new DiscordModalBuilder().WithCustomId($"generate_pin_{pinType}");

        switch (pinType)
        {
            case "judgeinfo": modal
                .WithTitle("Setup Judge Info Pin")
                .AddSelectMenu(
                    new DiscordChannelSelectComponent("channel", "#bot-commands", [DiscordChannelType.Text]),
                    "Channel",
                    "The channel to post the generated pin in.");
                break;
            
            case "journeytracking": modal
                .WithTitle("Setup Journey Tracking Pin")
                .AddSelectMenu(
                    new DiscordChannelSelectComponent("channel", "#nohit-judging", [DiscordChannelType.Text]),
                    "Channel",
                    "The channel to post the generated pin in.");
                break;
        }
        
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, modal);
    }

    [InteractionResponse("generate_pin")]
    public static async Task GeneratePinAsync(ModalSubmittedEventArgs args)
    {
        string pinType = args.Interaction.Data.CustomId.Split('_')[^1].Trim();
        ulong channelId = ulong.Parse((args.Values["channel"] as SelectMenuModalSubmission)!.Values[0]);
        
        DiscordConfig config = DataBase.DiscordConfigs[args.Interaction.GuildId!.Value];
        DiscordChannel channel = await args.Interaction.Guild!.GetChannelAsync(channelId);
        DiscordMessage message = await channel.SendMessageAsync("Generating...");

        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);

        switch (pinType)
        {
            case "judgeinfo":
                config.SetJudgeInfoPin(channelId, message.Id);
                await config.UpdateJudgeInfoPin();
                break;
            
            case "journeytracking":
                config.SetJourneyTrackingPin(channelId, message.Id);
                await config.UpdateJourneyTrackingInfoPin();
                break;
        }
    }
}