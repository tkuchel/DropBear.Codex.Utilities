using DropBear.Codex.Utilities.Hashing;
using DropBear.Codex.Utilities.Hashing.Interfaces;
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
        services.AddTransient<IHashingService, Blake2HashingService>();
        services.AddTransient<IHashingService, Argon2HashingService>();
        services.AddTransient<IHashingService, Blake3HashingService>();
        services.AddTransient<IHashingService, Fnv1AHashingService>();
        services.AddTransient<IHashingService, MurmurHash3Service>();
        services.AddTransient<IHashingService, SipHashingService>();
        services.AddTransient<IHashingService, XxHashingService>();
    }
}
