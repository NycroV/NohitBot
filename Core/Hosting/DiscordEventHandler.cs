using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace NohitBot.Hosting
{
    public abstract class DiscordEventHandler
    {
        // Give each event handler type its own semaphore to allow them to run one at a time
        protected static readonly Dictionary<Type, SemaphoreSlim> eventAccess = [];
    }

    public abstract class DiscordEventHandler<TEventArgs> : DiscordEventHandler, IEventHandler<TEventArgs> where TEventArgs : DiscordEventArgs
    {
        // Retrieve the corresponding semaphore, enter it, process the event, then exit
        public async Task HandleEventAsync(DiscordClient sender, TEventArgs args)
        {
            SemaphoreSlim semaphore;

            lock (eventAccess)
            {
                eventAccess.TryAdd(this.GetType(), new(1, 1));
                semaphore = eventAccess[this.GetType()];
            }

            await semaphore.WaitAsync();
            await DiscordBotService.CommandAccess.WaitAsync();

            try
            {
                await HandleAsync(args);
            }
            finally
            {
                DiscordBotService.CommandAccess.Release();
                semaphore.Release();
            }
        }

        public abstract Task HandleAsync(TEventArgs args);
    }

    public abstract class TransientDiscordEventHandler<TEventArgs> : DiscordEventHandler<TEventArgs> where TEventArgs : DiscordEventArgs
    { }

    public abstract class ScopedDiscordEventHandler<TEventArgs> : DiscordEventHandler<TEventArgs> where TEventArgs : DiscordEventArgs
    { }

    public static class EventHandlerExtensions
    {
        private static readonly MethodInfo registerMethod = typeof(EventHandlingBuilder).GetMethod(
            "AddEventHandlers",
            BindingFlags.Public | BindingFlags.Instance,
            [typeof(ServiceLifetime)])!;

        public static void RegisterEventHandlers(this IServiceCollection services)
        {
            services.ConfigureEventHandlers(events =>
            {
                var handlerType = typeof(DiscordEventHandler);
                var transientHandlerType = typeof(TransientDiscordEventHandler<>);
                var scopedHandlerType = typeof(ScopedDiscordEventHandler<>);

                var types = handlerType.Assembly.GetTypes().Where(t => t.IsSubclassOf(handlerType) && !t.IsAbstract);

                foreach (var type in types)
                {
                    ServiceLifetime lifetime =
                        type.BaseType == transientHandlerType ? ServiceLifetime.Transient : 
                        type.BaseType == scopedHandlerType ? ServiceLifetime.Scoped :
                        ServiceLifetime.Singleton;

                    MethodInfo registerGeneric = registerMethod.MakeGenericMethod(type);
                    registerGeneric.Invoke(events, [lifetime]);
                }
            });
        }
    }
}
