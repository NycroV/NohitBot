using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.TextCommands;

namespace NohitBot.Commands.Admin;

public class Ping
{
    [Command(nameof(Ping))]
    [RequireApplicationOwner]
    public static async ValueTask PingAsync(TextCommandContext ctx, [RemainingText] string input)
    {
        Console.WriteLine(input);
        await ctx.Channel.SendMessageAsync(input);
    }
}