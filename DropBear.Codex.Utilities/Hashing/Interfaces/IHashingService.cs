using DropBear.Codex.Core.ReturnTypes;

namespace DropBear.Codex.Utilities.Hashing.Interfaces;

/// <summary>
/// Defines the contract for hashing services, including password and general data hashing.
/// </summary>
public interface IHashingService
{
    /// <summary>
    /// Hashes a password or general data, returning a hash and relevant metadata.
    /// </summary>
    /// <param name="input">The input data to hash. For passwords, this is the plain text password.</param>
    /// <returns>A Result object containing the hash and any metadata.</returns>
    Result<string> Hash(string input);

    /// <summary>
    /// Verifies input data against a provided hash.
    /// </summary>
    /// <param name="input">The input data to verify.</param>
    /// <param name="expectedHash">The expected hash to compare against.</param>
    /// <returns>A Result indicating the verification outcome.</returns>
    Result Verify(string input, string expectedHash);

    /// <summary>
    /// Encodes input data to a Base64 encoded hash.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <returns>A Result containing the Base64 encoded hash.</returns>
    Result<string> EncodeToBase64Hash(byte[] data);

    /// <summary>
    /// Verifies the calculated hash of provided data matches the expected Base64 encoded hash.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <param name="expectedBase64Hash">The expected Base64 encoded hash.</param>
    /// <returns>A Result indicating if the hashes match.</returns>
    Result VerifyBase64Hash(byte[] data, string expectedBase64Hash);
}
