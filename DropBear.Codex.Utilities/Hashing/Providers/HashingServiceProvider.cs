using DropBear.Codex.Utilities.Hashing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DropBear.Codex.Utilities.Hashing.Providers;

public class HashingServiceProvider(IServiceProvider serviceProvider) : IHashingServiceProvider
{
    public IHashingService GetHashingService(string key) =>
        // Resolve the hashing service using the key
        // The specific mechanism depends on how keyed services are implemented in your DI container
        serviceProvider.GetRequiredKeyedService<IHashingService>(key);
}
