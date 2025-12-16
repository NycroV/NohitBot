using System.ComponentModel;
using DSharpPlus.Commands;

namespace NohitBot.Commands.Info;

public class Info
{
    [Command(nameof(Info))]
    [Description("Info about the NohitBot.")]
    public async ValueTask InfoAsync(CommandContext ctx)
    {
        
    }
}