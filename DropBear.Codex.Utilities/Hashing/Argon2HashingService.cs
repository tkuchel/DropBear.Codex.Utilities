using System.Collections;
using System.Text;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.Helpers;
using Konscious.Security.Cryptography;

namespace DropBear.Codex.Utilities.Hashing;

/// <summary>
///     Provides services for hashing and verifying hashes using Argon2.
/// </summary>
public class Argon2HashingService : IHashingService
{
    private const int SaltSize = 32; // The size of the salt in bytes.
    private const int HashSize = 16; // The size of the hash in bytes.
    private const int DegreeOfParallelism = 8; // The number of threads to use for hashing.
    private const int Iterations = 4; // The number of iterations to use in the hashing process.
    private const int MemorySize = 1024 * 1024; // Memory size for Argon2, set to 1GB.

    /// <summary>
    ///     Hashes the given input using Argon2 and returns the result as a Base64 string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A Result containing the Base64-encoded hash if successful, or an error message.</returns>
    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        var salt = HashingHelper.GenerateRandomSalt(SaltSize);
        using var argon2 = CreateArgon2(input, salt);
        var hashBytes = argon2.GetBytes(HashSize);
        var combinedBytes = HashingHelper.CombineBytes(salt, hashBytes);
        return Result<string>.Success(Convert.ToBase64String(combinedBytes));
    }

    /// <summary>
    ///     Verifies a given input against an expected hash.
    /// </summary>
    /// <param name="input">The input string to verify.</param>
    /// <param name="expectedHash">The expected hash in Base64 encoding.</param>
    /// <returns>A Result indicating success if the input matches the expected hash, or failure.</returns>
    public Result Verify(string input, string expectedHash)
    {
        try
        {
            var expectedBytes = Convert.FromBase64String(expectedHash);
            var (salt, expectedHashBytes) = HashingHelper.ExtractBytes(expectedBytes, SaltSize);
            using var argon2 = CreateArgon2(input, salt);
            var hashBytes = argon2.GetBytes(HashSize);

            return StructuralComparisons.StructuralEqualityComparer.Equals(hashBytes, expectedHashBytes)
                ? Result.Success()
                : Result.Failure("Verification failed.");
        }
        catch (FormatException)
        {
            return Result.Failure("Expected hash format is invalid.");
        }
    }

    /// <summary>
    ///     Encodes the given data to a Base64-encoded hash.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <returns>A Result containing the Base64-encoded string.</returns>
    public Result<string> EncodeToBase64Hash(byte[] data) =>
        Result<string>.Success(Convert.ToBase64String(data));

    /// <summary>
    ///     Verifies a given byte array against an expected Base64-encoded hash.
    /// </summary>
    /// <param name="data">The data to verify.</param>
    /// <param name="expectedBase64Hash">The expected hash in Base64 encoding.</param>
    /// <returns>A Result indicating success if the data matches the expected hash, or failure.</returns>
    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var base64Hash = Convert.ToBase64String(data);
        return base64Hash == expectedBase64Hash ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }

    // Creates a new instance of Argon2id using the provided input and salt.
    private static Argon2id CreateArgon2(string input, byte[] salt) =>
        new(Encoding.UTF8.GetBytes(input))
        {
            Salt = salt, DegreeOfParallelism = DegreeOfParallelism, Iterations = Iterations, MemorySize = MemorySize
        };
}
