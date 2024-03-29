using System.Collections;
using System.Text;
using Blake2Fast;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.Helpers;

namespace DropBear.Codex.Utilities.Hashing;

/// <summary>
///     Service for hashing and verifying data using Blake2b algorithm.
/// </summary>
public class Blake2HashingService : IHashingService
{
    private const int SaltSize = 32; // Optimal salt size for Blake2b
    private const int HashSize = 32; // Adjusted hash size for Blake2b

    /// <summary>
    ///     Computes the Blake2b hash of the input string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A Result containing the hashed value.</returns>
    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        var salt = HashingHelper.GenerateRandomSalt(SaltSize);
        var hashBytes = HashWithBlake2(input, salt);
        var combinedBytes = HashingHelper.CombineBytes(salt, hashBytes);
        return Result<string>.Success(Convert.ToBase64String(combinedBytes));
    }

    /// <summary>
    ///     Verifies whether the input string matches the expected hashed value.
    /// </summary>
    /// <param name="input">The input string to verify.</param>
    /// <param name="expectedHash">The expected hashed value to compare against.</param>
    /// <returns>A Result indicating success or failure of the verification.</returns>
    public Result Verify(string input, string expectedHash)
    {
        try
        {
            var expectedBytes = Convert.FromBase64String(expectedHash);
            var (salt, expectedHashBytes) = HashingHelper.ExtractBytes(expectedBytes, SaltSize);
            var hashBytes = HashWithBlake2(input, salt);

            if (StructuralComparisons.StructuralEqualityComparer.Equals(hashBytes, expectedHashBytes))
                return Result.Success();
            return Result.Failure("Verification failed.");
        }
        catch (FormatException)
        {
            return Result.Failure("Expected hash format is invalid.");
        }
    }

    /// <summary>
    ///     Computes the Blake2b hash of the input data and encodes it to Base64.
    /// </summary>
    /// <param name="data">The input data to hash.</param>
    /// <returns>A Result containing the Base64 encoded hashed value.</returns>
    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        var hash = Blake2b.ComputeHash(HashSize, data);
        return Result<string>.Success(Convert.ToBase64String(hash));
    }

    /// <summary>
    ///     Verifies whether the Base64 encoded hash matches the hashed value of the input data.
    /// </summary>
    /// <param name="data">The input data to verify.</param>
    /// <param name="expectedBase64Hash">The expected Base64 encoded hashed value.</param>
    /// <returns>A Result indicating success or failure of the verification.</returns>
    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var hash = Blake2b.ComputeHash(HashSize, data);
        var base64Hash = Convert.ToBase64String(hash);

        return base64Hash == expectedBase64Hash
            ? Result.Success()
            : Result.Failure("Base64 hash verification failed.");
    }

    /// <summary>
    ///     Computes the Blake2b hash of the input string with the given salt.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <param name="salt">The salt to use in hashing.</param>
    /// <returns>The hashed bytes.</returns>
    private static byte[] HashWithBlake2(string input, byte[] salt)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var saltedInput = HashingHelper.CombineBytes(salt, inputBytes);
        return Blake2b.ComputeHash(HashSize, saltedInput);
    }
}
