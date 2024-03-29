using System.Text;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using HashDepot;

namespace DropBear.Codex.Utilities.Hashing;

/// <summary>
///     Service for hashing and verifying data using the SipHash algorithm.
/// </summary>
public class SipHashingService : IHashingService
{
    private readonly byte[] _key;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SipHashingService" /> class with the specified key.
    /// </summary>
    /// <param name="key">The key used for hashing. Must be 16 bytes in length.</param>
    /// <exception cref="ArgumentException">Thrown when the key is not 16 bytes in length.</exception>
    public SipHashingService(byte[] key)
    {
        if (key?.Length is not 16)
            throw new ArgumentException("Key must be 16 bytes in length.", nameof(key));
        _key = key;
    }

    /// <summary>
    ///     Computes the SipHash hash of the input string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>A Result containing the hashed value.</returns>
    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        try
        {
            var buffer = Encoding.UTF8.GetBytes(input);
            var hash = SipHash24.Hash64(buffer, _key);
            return Result<string>.Success(hash.ToString("x8")); // Hex string format
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during hashing: {ex.Message}");
        }
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

        // Expected hash should be compared in a case-insensitive manner for hex values
        var isValid = string.Equals(hashResult.Value, expectedHash, StringComparison.OrdinalIgnoreCase);
        return isValid ? Result.Success() : Result.Failure("Verification failed.");
    }

    /// <summary>
    ///     Computes the SipHash hash of the input data and encodes it to Base64.
    /// </summary>
    /// <param name="data">The input data to hash.</param>
    /// <returns>A Result containing the Base64 encoded hashed value.</returns>
    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data.Length is 0)
            return Result<string>.Failure("Data cannot be null or empty.");

        try
        {
            var hash = SipHash24.Hash64(data, _key);
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

        // Comparison is direct; consider the possibility of padding differences in base64
        var isValid = string.Equals(encodeResult.Value, expectedBase64Hash, StringComparison.Ordinal);
        return isValid ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }
}
