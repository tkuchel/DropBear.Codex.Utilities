using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Models;

namespace DropBear.Codex.Utilities.Hashing.Interfaces;

/// <summary>
///     Defines the contract for a service that provides hashing and verification.
/// </summary>
public interface IDataHasher
{
    /// <summary>
    ///     Hashes a plain text password and returns the hashed password along with any relevant metadata.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <returns>
    ///     A <see cref="Result" /> object containing a <see cref="HashedPassword" /> instance
    ///     if hashing was successful, or an error result if hashing failed.
    /// </returns>
    Result<HashedPassword> HashPassword(string password);

    /// <summary>
    ///     Verifies a plain text password against a hashed password.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="expectedCombinedHash">The expected hashed password to compare against.</param>
    /// <returns>
    ///     A <see cref="Result" /> indicating the outcome of the verification. If the verification
    ///     is successful, the result is successful; otherwise, it contains details of the failure.
    /// </returns>
    Result VerifyPassword(string password, string expectedCombinedHash);
    
    /// <summary>
    /// Encodes data to a Base64 encoded hash.
    /// </summary>
    /// <param name="data">The data to encode.</param>
    /// <returns>A Result containing the Base64 encoded hash.</returns>
    Result<string> Base64EncodedHash(byte[] data);

    /// <summary>
    /// Verifies if the calculated hash of the provided data matches the expected hash.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <param name="expectedBase64Hash">The expected hash, Base64 encoded.</param>
    /// <returns>A Result indicating success if hashes match, or failure otherwise.</returns>
    Result VerifyHash(byte[] data, string expectedBase64Hash);
}
