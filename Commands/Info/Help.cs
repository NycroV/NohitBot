using System.Collections.Frozen;
using System.ComponentModel;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NohitBot.Hosting;
using NohitBot.Utilities;

namespace NohitBot.Commands.Info;

public class Help(CommandsExtension commands) : DiscordEventHandler<ClientStartedEventArgs>
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AllHelpAttribute : Attribute;
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class JudgeHelpAttribute : Attribute;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AdminHelpAttribute : Attribute;
    
    public static FrozenSet<Command>? AllHelpCommands { get; set; }
    
    public static FrozenSet<Command>? JudgeHelpCommands { get; set; }
    
    public static FrozenSet<Command>? AdminHelpCommands { get; set; }
    
    public override Task HandleAsync(ClientStartedEventArgs args)
    {
        AllHelpCommands = commands.Commands.Values.Where(c => c.Attributes.Any(a => a.GetType()! == typeof(AllHelpAttribute))).ToFrozenSet();
        JudgeHelpCommands = commands.Commands.Values.Where(c => c.Attributes.Any(a => a.GetType()! == typeof(JudgeHelpAttribute))).ToFrozenSet();
        AdminHelpCommands = commands.Commands.Values.Where(c => c.Attributes.Any(a => a.GetType()! == typeof(AdminHelpAttribute))).ToFrozenSet();
        return Task.CompletedTask;
    }
    
    [Command(nameof(Help))]
    [Description("Lists the available commands.")]
    public static async ValueTask HelpAsync(CommandContext ctx)
    {
        var message = new DiscordMessageBuilder()
            .EnableV2Components()
            .AddContainerComponent(new([
                new DiscordTextDisplayComponent(""),
                new DiscordActionRowComponent([
                    new DiscordSelectComponent("commandName", "Help", [])
                ]),
                new DiscordActionRowComponent([
                    new DiscordButtonComponent(DiscordButtonStyle.Primary, "help_all", "All"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "help_judge", "Judge"),
                    new DiscordButtonComponent(DiscordButtonStyle.Secondary, "help_admin", "Admin")
                ])], false, null
            ));

        await ctx.RespondAsync(message, true);
    }

    [InteractionResponse("help")]
    public static async ValueTask HelpCategoryAsync(ComponentInteractionCreatedEventArgs args)
    {
        string category = args.Interaction.Data.CustomId.Split('_')[1];

        var commands = category switch
        {
            "all" => AllHelpCommands,
            "judge" => JudgeHelpCommands,
            "admin" => AdminHelpCommands
        };

        var embed = new DiscordEmbedBuilder()
            .WithTitle("Help")
            .WithColor(DiscordColor.Red);

        foreach (Command command in commands!)
            embed.AddField(command.Name, command.Description!, true);

        var message = new DiscordMessageBuilder().AddEmbed(embed);
        var response = new DiscordInteractionResponseBuilder(message).AsEphemeral();
        
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, response);
    }
}