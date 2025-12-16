using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NohitBot.Hosting;
using NohitBot.Utilities;

namespace NohitBot.Commands.Admin;

public class Reload(DiscordClient client) : DiscordEventHandler<ClientStartedEventArgs>
{
    private DiscordClient Client { get; init; } = client;
    
    public const string ReloadInfoPath = "ReloadInfo.txt";
    
    [Command(nameof(Reload))]
    [RequireApplicationOwner]
    public async ValueTask ReloadAsync(TextCommandContext ctx)
    {
        await ctx.RespondAsync("Reloading...");
        var message = await ctx.GetResponseAsync();
        
        await File.WriteAllTextAsync(ReloadInfoPath, $"{ctx.Guild!.Id}/{ctx.Channel.Id}/{message!.Id}");
        await DiscordBotService.Host.StopAsync();
    }
    
    public override async Task HandleAsync(ClientStartedEventArgs args)
    {
        if (!File.Exists(ReloadInfoPath))
            return;

        string text = await File.ReadAllTextAsync(ReloadInfoPath);
        File.Delete(ReloadInfoPath);

        string[] messageIds = text.Split('/');
        
        ulong guildId = ulong.Parse(messageIds[0]);
        ulong channelId = ulong.Parse(messageIds[1]);
        ulong messageId = ulong.Parse(messageIds[2]);

        DiscordGuild? guild = await Client.GetGuildSafeAsync(guildId);
        DiscordChannel? channel = guild is not null ? await guild.GetChannelSafeAsync(channelId) : null;
        DiscordMessage? message = channel is not null ? await channel.GetMessageSafeAsync(messageId) : null;

        if (message is null)
            return;

        await message.ModifyAsync("Reload complete!");
    }
}