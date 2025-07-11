using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NohitBot.Hosting;

public static class HostingExtensions
{
    public static void AddHostedSingleton<T>(this IServiceCollection services) where T : class, IHostedService
    {
        services.AddSingleton<T>();
        services.AddHostedService<T>(provider => provider.GetService<T>()!);
    }
}