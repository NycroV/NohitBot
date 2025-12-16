using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NohitBot.Utilities;

namespace NohitBot.Commands.Admin;

public class Sudo
{
    [Command(nameof(Sudo))]
    [RequireApplicationOwner]
    [RequireGuild]
    public static async ValueTask SudoAsync(TextCommandContext ctx, DiscordMember member, [RemainingText] string command)
    {
        var sudoCtx = ctx with { User = member };
        string content = $"{ctx.Client.CurrentUser.Mention} {command}";

        MessageCreatedEventArgs fakeArgs = await Utils.CreateFakeMessageEventArgsAsync(sudoCtx, sudoCtx.Message, content);
        await ctx.Extension.GetProcessor<TextCommandProcessor>().ExecuteTextCommandAsync(ctx.Client, fakeArgs);
    }
}