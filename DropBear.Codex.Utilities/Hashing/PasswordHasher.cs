using System.Collections;
using System.Security.Cryptography;
using System.Text;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.Hashing.Models;
using Konscious.Security.Cryptography;

namespace DropBear.Codex.Utilities.Hashing;

/// <summary>
///     Implements password hashing and verification using Argon2id with enhanced security and configurability.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 32; // Increased size in bytes for the salt for enhanced security.
    private const int HashSize = 16; // Size in bytes for the hash.

    // Configurable Argon2 parameters to allow flexibility based on the deployment environment.
    public int DegreeOfParallelism { get; set; } = 8;
    public int Iterations { get; set; } = 4;
    public int MemorySize { get; set; } = 1024 * 1024; // 1 GB

    /// <summary>
    ///     Hashes a password using Argon2id with improved security measures.
    /// </summary>
    /// <param name="password">The password to hash. Must not be null or empty.</param>
    /// <returns>A result containing the hashed password and salt.</returns>
    public Result<HashedPassword> HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8) // Basic input validation
            return Result<HashedPassword>.Failure("Password is null or does not meet complexity requirements.");

        var salt = GenerateRandomSalt();
        using var argon2 = CreateArgon2(password, salt);
        var hash = argon2.GetBytes(HashSize);
        var combinedBytes = CombineBytes(salt, hash);
        var base64Hash = Convert.ToBase64String(combinedBytes);

        return Result<HashedPassword>.Success(new HashedPassword(base64Hash, salt));
    }

    /// <summary>
    ///     Verifies a password against a stored hash with enhanced error handling.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="expectedCombinedHash">The stored combined hash and salt.</param>
    /// <returns>A result indicating if the password is correct or not.</returns>
    public Result VerifyPassword(string password, string expectedCombinedHash)
    {
        var combinedBytes = Array.Empty<byte>();
        try
        {
             combinedBytes = Convert.FromBase64String(expectedCombinedHash);
        }
        catch (FormatException)
        {
            return Result.Failure("Invalid hash format.");
        }

        var (salt, expectedHash) = ExtractBytes(combinedBytes, SaltSize);

        using var argon2 = CreateArgon2(password, salt);
        var hash = argon2.GetBytes(HashSize);

        var isValid = StructuralComparisons.StructuralEqualityComparer.Equals(hash, expectedHash);
        return isValid ? Result.Success() : Result.Failure("Password is incorrect");
    }

    private Argon2id CreateArgon2(string password, byte[] salt)
    {
        return new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = DegreeOfParallelism,
            Iterations = Iterations,
            MemorySize = MemorySize
        };
    }

    private byte[] GenerateRandomSalt()
    {
        var buffer = new byte[SaltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);

        return buffer;
    }

    private static byte[] CombineBytes(byte[] salt, byte[] hash)
    {
        var combinedBytes = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, combinedBytes, salt.Length, hash.Length);
        return combinedBytes;
    }

    private static (byte[] salt, byte[] hash) ExtractBytes(byte[] combinedBytes, int saltSize)
    {
        var salt = new byte[saltSize];
        var hash = new byte[combinedBytes.Length - saltSize];
        Buffer.BlockCopy(combinedBytes, 0, salt, 0, saltSize);
        Buffer.BlockCopy(combinedBytes, saltSize, hash, 0, hash.Length);
        return (salt, hash);
    }
}