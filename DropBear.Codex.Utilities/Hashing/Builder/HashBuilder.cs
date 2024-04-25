using DropBear.Codex.Utilities.Hashing.ExtendedHashingServices;
using DropBear.Codex.Utilities.Hashing.Interfaces;

namespace DropBear.Codex.Utilities.Hashing.Builder;

public class HashBuilder : IHashBuilder
{
    private readonly Dictionary<string, Func<IHasher>> _serviceConstructors;

    public HashBuilder() =>
        _serviceConstructors = new Dictionary<string, Func<IHasher>>(StringComparer.OrdinalIgnoreCase)
        {
            { "argon2", () => new Argon2Hasher() },
            { "blake2", () => new Blake2Hasher() },
            { "blake3", () => new Blake3Hasher() },
            { "fnv1a", () => new Fnv1AHasher() },
            { "murmur3", () => new MurmurHash3Service() },
            { "siphash", () => new SipHasher(new byte[16]) },
            { "xxhash", () => new XxHasher() },
            { "extended_blake3", () => new ExtendedBlake3Hasher() }, // Extended Blake3 Service
            // Additional hashing services can be added here as needed
        };

    public IHasher GetHasher(string key)
    {
        if (_serviceConstructors.TryGetValue(key, out var constructor))
            return constructor();
        throw new ArgumentException("No hashing service registered for key: " + key, nameof(key));
    }
}
