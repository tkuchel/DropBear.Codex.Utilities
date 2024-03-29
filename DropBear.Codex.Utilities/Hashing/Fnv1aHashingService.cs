using System.Text;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using HashDepot;

namespace DropBear.Codex.Utilities.Hashing;

/// <summary>
///     Service for hashing and verifying data using FNV-1a algorithm.
/// </summary>
public class Fnv1AHashingService : IHashingService
{
    /// <summary>
    ///     Computes the FNV-1a hash of the input string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A Result containing the hashed value.</returns>
    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        var buffer = Encoding.UTF8.GetBytes(input);
        var hash = Fnv1a.Hash32(buffer); // For simplicity, using 32-bit hash
        return Result<string>.Success(hash.ToString("x8"));
    }

    /// <summary>
    ///     Verifies whether the input string matches the expected hashed value.
    /// </summary>
    /// <param name="input">The input string to verify.</param>
    /// <param name="expectedHash">The expected hashed value to compare against.</param>
    /// <returns>A Result indicating success or failure of the verification.</returns>
    public Result Verify(string input, string expectedHash)
    {
        var hashResult = Hash(input);
        if (!hashResult.IsSuccess)
            return Result.Failure("Failed to compute hash.");

        var isValid = string.Equals(hashResult.Value, expectedHash, StringComparison.OrdinalIgnoreCase);
        return isValid ? Result.Success() : Result.Failure("Verification failed.");
    }

    /// <summary>
    ///     Computes the FNV-1a hash of the input data and encodes it to Base64.
    /// </summary>
    /// <param name="data">The input data to hash.</param>
    /// <returns>A Result containing the Base64 encoded hashed value.</returns>
    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data.Length is 0)
            return Result<string>.Failure("Data cannot be null or empty.");

        try
        {
            var hash = Fnv1a.Hash64(data); // Using 64-bit for a better demonstration
            var hashBytes = BitConverter.GetBytes(hash);
            var base64Hash = Convert.ToBase64String(hashBytes);
            return Result<string>.Success(base64Hash);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during hashing: {ex.Message}");
        }
    }

    /// <summary>
    ///     Verifies whether the Base64 encoded hash matches the hashed value of the input data.
    /// </summary>
    /// <param name="data">The input data to verify.</param>
    /// <param name="expectedBase64Hash">The expected Base64 encoded hashed value.</param>
    /// <returns>A Result indicating success or failure of the verification.</returns>
    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var encodeResult = EncodeToBase64Hash(data);
        if (!encodeResult.IsSuccess)
            return Result.Failure("Failed to compute hash.");

        var isValid = string.Equals(encodeResult.Value, expectedBase64Hash, StringComparison.Ordinal);
        return isValid ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }
}
