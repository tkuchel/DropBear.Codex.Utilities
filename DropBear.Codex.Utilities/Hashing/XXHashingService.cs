using System.Text;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using HashDepot;

namespace DropBear.Codex.Utilities.Hashing;

/// <summary>
///     Service for hashing and verifying data using the XXHash algorithm.
/// </summary>
public class XxHashingService : IHashingService
{
    /// <summary>
    ///     Computes the XXHash hash of the input string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A Result containing the hashed value.</returns>
    public Result<string> Hash(string input)
    {
        // Validates input to ensure it's not null or empty.
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        var buffer = Encoding.UTF8.GetBytes(input);
        var hash = XXHash.Hash64(buffer); // Default seed is used here.
        return Result<string>.Success(hash.ToString("x8")); // Returns hash in hexadecimal format.
    }

    /// <summary>
    ///     Verifies whether the input string matches the expected hashed value.
    /// </summary>
    /// <param name="input">The input string to verify.</param>
    /// <param name="expectedHash">The expected hashed value to compare against.</param>
    /// <returns>A Result indicating success or failure of the verification.</returns>
    public Result Verify(string input, string expectedHash)
    {
        // Utilizes the Hash method to calculate the hash of the input and compares it to the expected hash.
        var hashResult = Hash(input);
        if (!hashResult.IsSuccess)
            return Result.Success();

        return hashResult.Value == expectedHash ? Result.Success() : Result.Failure("Verification failed.");
    }

    /// <summary>
    ///     Computes the XXHash hash of the input data and encodes it to Base64.
    /// </summary>
    /// <param name="data">The input data to hash.</param>
    /// <returns>A Result containing the Base64 encoded hashed value.</returns>
    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        // Checks for null data to ensure robust error handling.
        if (data.Length is 0)
            return Result<string>.Failure("Data cannot be null or empty.");

        var hash = XXHash.Hash64(data); // Hashes the data using XXHash.
        var base64EncodedHash = Convert.ToBase64String(BitConverter.GetBytes(hash)); // Converts the hash to Base64.
        return Result<string>.Success(base64EncodedHash);
    }

    /// <summary>
    ///     Verifies whether the Base64 encoded hash matches the hashed value of the input data.
    /// </summary>
    /// <param name="data">The input data to verify.</param>
    /// <param name="expectedBase64Hash">The expected Base64 encoded hash.</param>
    /// <returns>A Result indicating success or failure of the verification.</returns>
    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        // Similar to EncodeToBase64Hash but with verification against an expected Base64 encoded hash.
        var hashResult = EncodeToBase64Hash(data);
        if (!hashResult.IsSuccess)
            return Result.Success(); // Propagates failure if hashing or encoding failed.

        return hashResult.Value == expectedBase64Hash
            ? Result.Success()
            : Result.Failure("Base64 hash verification failed.");
    }
}
