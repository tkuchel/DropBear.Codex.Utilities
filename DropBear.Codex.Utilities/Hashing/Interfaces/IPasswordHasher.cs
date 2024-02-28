using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Models;

namespace DropBear.Codex.Utilities.Hashing.Interfaces;

/// <summary>
///     Defines the contract for a service that provides password hashing and verification.
/// </summary>
public interface IPasswordHasher
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
}
