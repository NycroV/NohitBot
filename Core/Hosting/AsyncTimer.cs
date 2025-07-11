using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Timer = System.Timers.Timer;

namespace NohitBot.Hosting;

public abstract class AsyncTimer(ILogger logger) : IHostedService
{
    public static SemaphoreSlim Access { get; } = new(1, 1);
    
    public required Timer Timer { get; set; }

    public virtual TimeSpan? InitialInterval { get; } = null;

    public abstract TimeSpan Interval { get; }

    public ILogger Logger { get; init; } = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (InitialInterval is null)
        {
            await Task.Run(() =>
            {
                Timer = new(Interval);
                Timer.Elapsed += async (_, _) => await InvokeAsync().ConfigureAwait(false);
                Timer.AutoReset = true;
                Timer.Start();
            }, cancellationToken);
        }

        else
        {
            await Task.Run(() =>
            {
                Timer = new(InitialInterval.Value);
                Timer.Elapsed += async (_, _) => await InvokeAndChangeIntervalAsync().ConfigureAwait(false);
                Timer.AutoReset = false;
                Timer.Start();
            }, cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            Timer.Stop();
            Timer.Dispose();
        }, cancellationToken);
    }

    public virtual async Task ResetAsync()
    {
        await StopAsync(CancellationToken.None);
        await StartAsync(CancellationToken.None);
    }

    public abstract Task RunAsync();

    private async Task InvokeAsync()
    {
        Logger.LogDebug("Queuing run for AsyncTimer - {name}", GetType().Name);

        await Access.WaitAsync();

        Logger.LogInformation("Running AsyncTimer - {name}", GetType().Name);

        try
        {
            await RunAsync().ConfigureAwait(false);
        }
        finally
        {
            Access.Release();
        }

        Logger.LogDebug("Finished run for AsyncTimer - {name}", GetType().Name);
    }

    private async Task InvokeAndChangeIntervalAsync()
    {
        await InvokeAsync().ConfigureAwait(false);
        Timer.Dispose();

        Timer = new(Interval);
        Timer.Elapsed += async (_, _) => await InvokeAsync().ConfigureAwait(false);
        Timer.AutoReset = true;
        Timer.Start();
    }
}

public static class AsyncTimerExtensions
{
    private static readonly MethodInfo addHostedSingletonMethod = typeof(HostingExtensions).GetMethod(
        "AddHostedSingleton",
        BindingFlags.Public | BindingFlags.Static,
        [typeof(IServiceCollection)])!;
    
    public static void AddAsyncTimers(this IServiceCollection services)
    {
        var timerType = typeof(AsyncTimer);
        var types = timerType.Assembly.GetTypes().Where(t => t.IsSubclassOf(timerType) && !t.IsAbstract);
        
        foreach (var type in types)
        {
            var addHostedGeneric = addHostedSingletonMethod.MakeGenericMethod(type);
            addHostedGeneric.Invoke(null, [services]);
        }
    }
}