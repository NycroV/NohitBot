using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Entities;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace NohitBot.Commands.Admin;

public class Eval
{
    public class TestVariables(TextCommandContext context)
    {
        [UsedImplicitly]
        public TextCommandContext ctx { get; set; } = context;
    }
    
    [Command(nameof(Eval))]
    [RequireApplicationOwner]
    public static async ValueTask EvalAsync(TextCommandContext ctx, [RemainingText]string code)
    {
        // ReSharper disable once StringIndexOfIsCultureSpecific.1
        var code_start = code.IndexOf("```") + 3;
        code_start = code.IndexOf('\n', code_start) + 1;
        
        // ReSharper disable once StringLastIndexOfIsCultureSpecific.1
        var code_end = code.LastIndexOf("```");

        if (code_start == -1 || code_end == -1)
        {
            await ctx.RespondAsync("Format your code idiot");
            return;
        }

        var cs = code.Substring(code_start, code_end - code_start);

        await ctx.RespondAsync("Working...");
        await ctx.Channel.TriggerTypingAsync();

        try
        {
            var globals = new TestVariables(ctx);
            var scriptOptions = ScriptOptions.Default;
            scriptOptions = scriptOptions.WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location)));
            scriptOptions = scriptOptions.WithImports(
                "DSharpPlus",
                "DSharpPlus.Commands",
                "DSharpPlus.Entities",
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Text",
                "System.Threading.Tasks",
                "NohitBot",
                "NohitBot.Data",
                "NohitBot.Database",
                "NohitBot.Discord",
                "Newtonsoft.Json");

            var script = CSharpScript.Create(cs, scriptOptions, typeof(TestVariables));
            script.Compile();
            
            var result = await script.RunAsync(globals).ConfigureAwait(false);
            string? resultString = result?.ReturnValue?.ToString() ?? null;

            if (result is { ReturnValue: not null } && !string.IsNullOrWhiteSpace(resultString))
                await ctx.RespondAsync(new DiscordEmbedBuilder()
                {
                    Title = "✅ Evaluation Result",
                    Color = DiscordColor.Teal,
                    Description = result.ReturnValue.ToString()
                }.Build());
            
            else
                await ctx.RespondAsync(new DiscordEmbedBuilder()
                {
                    Title = "✅ Evaluation Result",
                    Color = DiscordColor.Teal,
                    Description = "No result was returned."
                }.Build());
        }
        
        catch (Exception ex)
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder()
            {
                Title = "⚠️ Evaluation Failure",
                Color = DiscordColor.DarkRed,
                Description = $"**{ex.GetType()}**:\n" +
                              $"{ex.Message}"
            }.Build());
        }
    }
}