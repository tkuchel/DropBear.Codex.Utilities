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
        services.AddTransient<IDataHasher, BlakePasswordHasher>();
        services.AddTransient<IDataHasher, ArgonPasswordHasher>();
    }
    
}
