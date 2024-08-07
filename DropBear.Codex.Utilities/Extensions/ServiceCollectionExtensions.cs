﻿#region

using DropBear.Codex.Utilities.DebounceManagement;
using DropBear.Codex.Utilities.FeatureFlags;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#endregion

namespace DropBear.Codex.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the DynamicFlagManager to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddDynamicFlagManager(this IServiceCollection services)
    {
        // Ensure that DynamicFlagManager is only added once
        services.TryAddSingleton<IDynamicFlagManager, DynamicFlagManager>();
        return services;
    }


    /// <summary>
    ///     Adds the DebounceService to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
    public static IServiceCollection AddDebounceService(this IServiceCollection services)
    {
        // Ensure MemoryCache is registered
        if (services.All(x => x.ServiceType != typeof(IMemoryCache)))
        {
            services.AddMemoryCache();
        }

        // Ensure that DebounceService is only added once
        services.TryAddSingleton<IDebounceService, DebounceService>();

        return services;
    }
}
