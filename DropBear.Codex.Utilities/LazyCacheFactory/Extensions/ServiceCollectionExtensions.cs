using Microsoft.Extensions.DependencyInjection;

namespace DropBear.Codex.Utilities.LazyCacheFactory.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLazyFactorySupport(this IServiceCollection services)
    {
        // Ensure MemoryCache is added
        services.AddMemoryCache();

        // Register the MemoryCacheManager as a singleton because it typically contains shared state or configuration
        services.AddSingleton<MemoryCacheManager>();

        // Any other dependencies required by your lazy factory can be registered here

        return services;
    }
}
