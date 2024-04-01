using DropBear.Codex.Utilities.Hashing;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.Hashing.Providers;
using DropBear.Codex.Utilities.MessageTemplates;
using Microsoft.Extensions.DependencyInjection;

namespace DropBear.Codex.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the DropBear Codex utility services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    public static void AddHashingUtilities(this IServiceCollection services)
    {
        services.AddKeyedTransient<IHashingService,Blake2HashingService>("Blake2");
        services.AddKeyedTransient<IHashingService, Argon2HashingService>("Argon2");
        services.AddKeyedTransient<IHashingService, Blake3HashingService>("Blake3");
        services.AddKeyedTransient<IHashingService, Fnv1AHashingService>("Fnv1A");
        services.AddKeyedTransient<IHashingService, MurmurHash3Service>("Murmur3");
        services.AddKeyedTransient<IHashingService, SipHashingService>("SipHash");
        services.AddKeyedTransient<IHashingService, XxHashingService>("XXHash");
        services.AddSingleton<IHashingServiceProvider, HashingServiceProvider>();

    }
    
    /// <summary>
    ///   Adds the DropBear Codex MessageTemplate Manager service to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"></param>
    public static void AddMessageTemplateManager(this IServiceCollection services)
    {
        services.AddSingleton<IMessageTemplateManager, MessageTemplateManager>();
    }
}
