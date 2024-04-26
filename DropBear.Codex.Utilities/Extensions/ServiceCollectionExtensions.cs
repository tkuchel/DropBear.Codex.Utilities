﻿using DropBear.Codex.Utilities.FeatureFlags;
using DropBear.Codex.Utilities.MessageTemplates;
using DropBear.Codex.Utilities.MessageTemplates.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DropBear.Codex.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDynamicFlagManager(this IServiceCollection services)
    {
        services.AddSingleton<IDynamicFlagManager, DynamicFlagManager>();
        return services;
    }

    public static IServiceCollection AddMessageTemplateManager(this IServiceCollection services)
    {
        services.AddSingleton<IMessageTemplateProvider, MessageTemplateProvider>();
        return services;
    }
}
