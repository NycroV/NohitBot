using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NohitBot.Hosting;
using NohitBot.Logging;
using DSharpPlus.Commands;
using NohitBot.Discord;

//----------------------//
//----------------------//

var workingDirectory = Directory.CreateDirectory("BotFiles");
Directory.SetCurrentDirectory(workingDirectory.FullName);

AppDomain.CurrentDomain.UnhandledException += (_, exception) =>
{
    DiscordLogger.Writer.Close();
    File.WriteAllText("Crash.txt", exception.ExceptionObject.ToString());
};

var builder = new HostApplicationBuilder();

builder.Services.AddHostedSingleton<DiscordBotService>();
builder.Services.AddHostedSingleton<Cache>();

builder.Services.AddDiscordClient(DiscordBotService.Token, DiscordBotService.Intents);
builder.Services.RegisterEventHandlers();
builder.Services.AddAsyncTimers();

builder.Services.AddCommandsExtension(
    (_, commands) => {
        commands.AddCheck<DiscordBotService.OneCommandAtATime>();
        commands.AddCommands(typeof(DiscordBotService).Assembly);
        commands.CommandExecuted += async (_, _) => await DiscordBotService.OneCommandAtATime.CompleteCommandAsync();
        commands.CommandErrored += async (_, _) => await DiscordBotService.OneCommandAtATime.CompleteCommandAsync();
    },

    new CommandsConfiguration() {
        UseDefaultCommandErrorHandler = false
    }
);

builder.Services.AddLogging(logger =>
{
    logger.ClearProviders();
    logger.AddProvider(new DiscordLoggerProvider());
    logger.SetMinimumLevel(LogLevel.Trace);
});

var host =  builder.Build();
host.Run();