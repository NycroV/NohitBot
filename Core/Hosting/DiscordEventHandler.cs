using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Handler = System.Func<DSharpPlus.DiscordClient, DSharpPlus.EventArgs.DiscordEventArgs, System.IServiceProvider, System.Threading.Tasks.Task>;

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

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InteractionResponseAttribute(string interactionId) : Attribute
    {
        public string InteractionId { get; } = interactionId;
    }
    
    public static class EventHandlerExtensions
    {
        private static readonly MethodInfo registerMethod = typeof(EventHandlingBuilder).GetMethod(
            "AddEventHandlers",
            BindingFlags.Public | BindingFlags.Instance,
            [typeof(ServiceLifetime)])!;

        private static readonly FieldInfo handlersField = typeof(EventHandlerCollection).GetField(
            "handlers",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        
        private static Dictionary<Type, List<object>> Handlers(this EventHandlerCollection c) =>
            (Dictionary<Type, List<object>>)handlersField.GetValue(c)!;

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

            services.ConfigureEventHandlers(events =>
            {
                var interactionAttribute = typeof(InteractionResponseAttribute);
                var types = interactionAttribute.Assembly.GetTypes(); 
                var methods = types.SelectMany(t => t.GetMethods().Where(m => Attribute.GetCustomAttribute(m, interactionAttribute) is not null));

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    string interactionKey = ((InteractionResponseAttribute)Attribute.GetCustomAttribute(method, interactionAttribute)!).InteractionId;

                    if (method.ReturnType != typeof(Task))
                        throw new("InteractionResponse methods must have a Task return type.");

                    Type eventType = parameters[0].ParameterType;
                    
                    if (parameters.Length != 1 || !eventType.IsAssignableTo(typeof(InteractionCreatedEventArgs)))
                        throw new("InteractionResponses must have 1 parameters that derives from InteractionCreatedEventArgs.");

                    Type? serviceType = method.IsStatic ? null : method.DeclaringType;
                    
                    if (serviceType is not null)
                        events.Services.TryAddSingleton(serviceType);

                    events.Services.Configure<EventHandlerCollection>(collection =>
                    {
                        var handlers = collection.Handlers();
                        handlers.TryAdd(eventType, []);
                        handlers[eventType].Add((Handler)WrapInteraction);
                    });
                    
                    continue;

                    Task WrapInteraction(DiscordClient client, DiscordEventArgs args, IServiceProvider provider)
                    {
                        object? invocationObject = serviceType is null ? null : provider.GetRequiredService(serviceType);
                        
                        if ((args as InteractionCreatedEventArgs)!.Interaction.Data.CustomId.StartsWith(interactionKey))
                            return (Task)method.Invoke(invocationObject, [args])!;

                        return Task.CompletedTask;
                    }
                }
            });
        }
    }
}
