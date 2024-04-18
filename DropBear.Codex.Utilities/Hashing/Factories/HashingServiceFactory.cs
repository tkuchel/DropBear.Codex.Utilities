using DropBear.Codex.Utilities.Hashing.ExtendedHashingServices;
using DropBear.Codex.Utilities.Hashing.Interfaces;

namespace DropBear.Codex.Utilities.Hashing.Factories;

public class HashingServiceFactory : IHashingServiceFactory
{
    private readonly Dictionary<string, Func<IHashingService>> _serviceConstructors;

    public HashingServiceFactory() =>
        _serviceConstructors = new Dictionary<string, Func<IHashingService>>(StringComparer.OrdinalIgnoreCase)
        {
            { "argon2", () => new Argon2HashingService() },
            { "blake2", () => new Blake2HashingService() },
            { "blake3", () => new Blake3HashingService() },
            { "fnv1a", () => new Fnv1AHashingService() },
            { "murmur3", () => new MurmurHash3Service() },
            { "siphash", () => new SipHashingService(new byte[16]) }, // Example key, adjust as necessary
            { "xxhash", () => new XxHashingService() },
            { "extended_blake3", () => new ExtendedBlake3HashingService() }, // Extended Blake3 Service
            // Additional hashing services can be added here as needed
        };

    public IHashingService CreateService(string key)
    {
        if (_serviceConstructors.TryGetValue(key, out var constructor))
            return constructor();
        throw new ArgumentException("No hashing service registered for key: " + key, nameof(key));
    }
}
