using DropBear.Codex.Utilities.FeatureFlags;
using DropBear.Codex.Utilities.Hashing.Factories;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.MessageTemplates;
using Microsoft.Extensions.DependencyInjection;

namespace DropBear.Codex.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHashingServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashingServiceFactory, HashingServiceFactory>();
        return services;
    }
    
    public static IServiceCollection AddDynamicFlagManager(this IServiceCollection services)
    {
        services.AddSingleton<IDynamicFlagManager, DynamicFlagManager>();
        return services;
    }
    
    public static IServiceCollection AddMessageTemplateManager(this IServiceCollection services)
    {
        services.AddSingleton<IMessageTemplateManager, MessageTemplateManager>();
        return services;
    }
}
