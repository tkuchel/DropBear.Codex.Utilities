namespace DropBear.Codex.Utilities.Hashing.Models;

/// <summary>
///     Represents the result of hashing a password, including the hash and the salt used.
///     This class is designed to be immutable to ensure the integrity of the hashed password data.
/// </summary>
public class HashedPassword
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="HashedPassword" /> class with specified hash and salt.
    /// </summary>
    /// <param name="hash">The hashed password.</param>
    /// <param name="salt">The salt used in the hashing process.</param>
    /// <exception cref="ArgumentException">Thrown when hash or salt is null or empty.</exception>
    public HashedPassword(string hash, byte[] salt)
    {
        if (string.IsNullOrWhiteSpace(hash)) throw new ArgumentException("Hash cannot be null or empty.", nameof(hash));
        if (salt == null || salt.Length == 0)
            throw new ArgumentException("Salt cannot be null or empty.", nameof(salt));

        Hash = hash;
        Salt = salt;
    }

    /// <summary>
    ///     Parameterless constructor for serialization/deserialization purposes.
    /// </summary>
    /// <remarks>
    ///     Make sure that the serialization framework you use supports private setters
    ///     if you intend to keep the properties immutable.
    /// </remarks>
    private HashedPassword()
    {
        // Used for deserialization
    }

    /// <summary>
    ///     Gets the hashed password.
    /// </summary>
    /// <value>The hash of the password.</value>
    public string Hash { get; private set; }

    /// <summary>
    ///     Gets the salt used to hash the password.
    /// </summary>
    /// <value>The salt as a byte array.</value>
    public byte[] Salt { get; private set; }
}