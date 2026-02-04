using System.Reflection;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Handler = System.Func<DSharpPlus.DiscordClient, DSharpPlus.EventArgs.DiscordEventArgs, System.IServiceProvider, System.Threading.Tasks.Task>;

namespace NohitBot.Hosting;

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
        Type type = GetType();

        lock (eventAccess)
        {
            eventAccess.TryAdd(type, new(1, 1));
            semaphore = eventAccess[type];
        }

        await semaphore.WaitAsync();
        //await DiscordBotService.CommandAccess.WaitAsync();

        try
        {
            await HandleAsync(args);
        }
        finally
        {
            //DiscordBotService.CommandAccess.Release();
            semaphore.Release();
        }
    }

    public abstract Task HandleAsync(TEventArgs args);
}

public abstract class TransientDiscordEventHandler<TEventArgs> : DiscordEventHandler<TEventArgs> where TEventArgs : DiscordEventArgs
{
}

public abstract class ScopedDiscordEventHandler<TEventArgs> : DiscordEventHandler<TEventArgs> where TEventArgs : DiscordEventArgs
{
}

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

    private static Dictionary<Type, List<object>> Handlers(this EventHandlerCollection c)
    {
        return (Dictionary<Type, List<object>>)handlersField.GetValue(c)!;
    }

    public static void RegisterEventHandlers(this IServiceCollection services)
    {
        services.ConfigureEventHandlers(events =>
        {
            Type handlerType = typeof(DiscordEventHandler);
            Type singletonHandlerType = typeof(DiscordEventHandler<>);
            Type transientHandlerType = typeof(TransientDiscordEventHandler<>);
            Type scopedHandlerType = typeof(ScopedDiscordEventHandler<>);

            var types = handlerType.Assembly.GetTypes().Where(t => t.IsSubclassOf(handlerType) && !t.IsAbstract);

            foreach (Type type in types)
            {
                Type lifetimeType = type;
                ServiceLifetime? lifetime;
                
                while (true)
                {
                    if (lifetimeType.BaseType!.GenericTypeArguments.Length != 1)
                    {
                        if (lifetimeType.BaseType == handlerType)
                            throw new($"You cannot inherit from DiscordEventHandler directly - instead, inherit from one of its generic implementations.\n" +
                                      $"Type: {lifetimeType}");
                        
                        lifetimeType = lifetimeType.BaseType;
                        continue;
                    }
                    
                    var genericType = lifetimeType.BaseType.GenericTypeArguments[0];

                    if (!genericType.IsSubclassOf(typeof(DiscordEventArgs)))
                    {
                        lifetimeType = lifetimeType.BaseType;
                        continue;
                    }

                    lifetime = lifetimeType switch
                    {
                        _ when lifetimeType.BaseType == singletonHandlerType.MakeGenericType(genericType) => ServiceLifetime.Singleton,
                        _ when lifetimeType.BaseType == transientHandlerType.MakeGenericType(genericType) => ServiceLifetime.Transient,
                        _ when lifetimeType.BaseType == scopedHandlerType.MakeGenericType(genericType) => ServiceLifetime.Scoped,
                        _ => null
                    };

                    if (lifetime is not null)
                        break;
                    
                    lifetimeType = lifetimeType.BaseType;
                }

                MethodInfo registerGeneric = registerMethod.MakeGenericMethod(type);
                registerGeneric.Invoke(events, [lifetime!.Value]);
            }
        });

        services.ConfigureEventHandlers(events =>
        {
            Type interactionAttribute = typeof(InteractionResponseAttribute);
            var types = interactionAttribute.Assembly.GetTypes();
            var methods = types.SelectMany(t => t.GetMethods().Where(m => Attribute.GetCustomAttribute(m, interactionAttribute) is not null));

            foreach (MethodInfo method in methods)
            {
                var parameters = method.GetParameters();
                string interactionKey = ((InteractionResponseAttribute)Attribute.GetCustomAttribute(method, interactionAttribute)!).InteractionId;

                if (method.ReturnType != typeof(ValueTask) && method.ReturnType != typeof(Task))
                    throw new("InteractionResponse methods must have a ValueTask or Task return type.");

                if (parameters.Length != 1 || !parameters[0].ParameterType.IsAssignableTo(typeof(InteractionCreatedEventArgs)))
                    throw new("InteractionResponse methods must have only 1 parameter, which derives from InteractionCreatedEventArgs.");

                Type eventType = parameters[0].ParameterType;
                Type? serviceType = method.IsStatic ? null : method.DeclaringType;

                if (serviceType is not null)
                    events.Services.TryAddSingleton(serviceType);

                Task WrapInteraction(DiscordClient client, DiscordEventArgs args, IServiceProvider provider)
                {
                    object? invocationObject = serviceType is null ? null : provider.GetRequiredService(serviceType);

                    if (!(args as InteractionCreatedEventArgs)!.Interaction.Data.CustomId.StartsWith(interactionKey))
                        return Task.CompletedTask;
                    
                    if (method.ReturnType == typeof(ValueTask))
                        return ((ValueTask)method.Invoke(invocationObject, [args])!).AsTask();
                        
                    return (Task)method.Invoke(invocationObject, [args])!;
                }

                events.Services.Configure<EventHandlerCollection>(collection =>
                {
                    var handlers = collection.Handlers();
                    handlers.TryAdd(eventType, []);
                    handlers[eventType].Add((Handler)WrapInteraction);
                });
            }
        });
    }
}