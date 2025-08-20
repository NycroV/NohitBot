using System.Diagnostics;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NohitBot.Hosting;
using NohitBot.Logging;
using DSharpPlus.Commands;
using NohitBot.Database;
using NohitBot.Discord;

//----------------------//
//----------------------//

var workingDirectory = Directory.CreateDirectory("BotFiles");
Directory.SetCurrentDirectory(workingDirectory.FullName);

AppDomain.CurrentDomain.UnhandledException += (_, exception) =>
{
    DiscordLogger.Writer.Close();
    File.WriteAllText("Crash.txt", exception.ExceptionObject.ToString());
    DataBase.Save();
};

var builder = new HostApplicationBuilder();

builder.Services.AddHostedSingleton<DiscordBotService>();
builder.Services.AddHostedSingleton<Cache>();

builder.Services.AddDiscordClient(DiscordBotService.Token, DiscordBotService.Intents);
builder.Services.RegisterEventHandlers();
builder.Services.AddAsyncTimers();

builder.Services.AddCommandsExtension(
    (_, commands) => {
        commands.AddCommands(typeof(DiscordBotService).Assembly);
        commands.CommandErrored += DiscordBotService.CommandErrored;
        // commands.AddCheck<DiscordBotService.OneCommandAtATime>();
        // commands.CommandExecuted += async (_, _) => await DiscordBotService.OneCommandAtATime.CompleteCommandAsync();
        // commands.CommandErrored += async (_, _) => await DiscordBotService.OneCommandAtATime.CompleteCommandAsync();
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

DiscordBotService.Host =  builder.Build();
DiscordBotService.Host.Run();

string executablePath = Environment.ProcessPath!;
Process.Start(executablePath);
Environment.Exit(0);