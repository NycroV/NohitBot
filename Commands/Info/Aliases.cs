using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NohitBot.Data;
using NohitBot.Database;
using NohitBot.Hosting;

namespace NohitBot.Commands.Info;

public class Aliases(DiscordClient client) : DiscordEventHandler<ClientStartedEventArgs>
{
    private static DiscordComponentEmoji? LeftArrow { get; set; } = null;

    private static DiscordComponentEmoji? RightArrow { get; set; } = null;

    public override Task HandleAsync(ClientStartedEventArgs args)
    {
        LeftArrow ??= new(DiscordEmoji.FromName(client, ""));
        RightArrow ??= new(DiscordEmoji.FromName(client, ""));
        return Task.CompletedTask;
    }
    
    [Command(nameof(Aliases))]
    [RequireGuild]
    public static async ValueTask AliasesAsync(CommandContext ctx)
    {
        var bosses = DataBase.Bosses.Values.Where(b => b.ManagementServer == 0uL || b.ManagementServer == ctx.Guild!.Id).ToArray();
        var page = bosses.Take(25);

        var message = BuildAliasPage(ctx.Guild!.Name, page, 1, (bosses.Length - 1) / 25 + 1);
        await ctx.RespondAsync(message);
    }

    [InteractionResponse("aliases_page")]
    public static async ValueTask NavigateAliases(ComponentInteractionCreatedEventArgs args)
    {
        bool pageUp = args.Interaction.Data.CustomId.Split('_')[^1] == "right";
        int currentPage = int.Parse(args.Message.Embeds[0].Footer!.Text!.Split('/')[0].Trim());

        var bosses = DataBase.Bosses.Values.Where(b => b.ManagementServer == 0uL || b.ManagementServer == args.Guild.Id).ToArray();
        int maxPages = (bosses.Length - 1) / 25 + 1;

        currentPage += pageUp ? 1 : -1;
        currentPage = Math.Clamp(currentPage, 1, maxPages);

        var page = bosses.Skip((currentPage - 1) * 25).Take(25);
        var message = BuildAliasPage(args.Guild.Name, page, currentPage, maxPages);
        
        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(message));
    }

    private static DiscordMessageBuilder BuildAliasPage(string guildName, IEnumerable<Boss> bosses, int pageNumber, int maxPages)
    {
        var embed = new DiscordEmbedBuilder()
            .WithTitle("Boss Aliases")
            .WithDescription($"Aliases for bosses in {guildName}")
            .WithFooter($"{pageNumber} / {maxPages}");

        foreach (var boss in bosses)
            embed.AddField(boss.Name, string.Join(", ", boss.Aliases), true);

        return new DiscordMessageBuilder()
            .AddEmbed(embed)
            .AddActionRowComponent(
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "aliases_page_left", "", pageNumber == 1, LeftArrow!),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "aliases_page_right", "", pageNumber == maxPages, RightArrow!));
    }
}