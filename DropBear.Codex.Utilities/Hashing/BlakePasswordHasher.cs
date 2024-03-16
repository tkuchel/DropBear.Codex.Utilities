using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.Hashing.Models;
using Blake2Fast; // Make sure to import the Blake2Fast namespace

namespace DropBear.Codex.Utilities.Hashing
{
    /// <summary>
    /// Implements password hashing and verification using Blake2b with enhanced security and configurability.
    /// </summary>
    public class BlakePasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 32; // Size in bytes for the salt for enhanced security.
        private const int HashSize = 32; // Adjusting hash size for Blake2b

        /// <summary>
        /// Hashes a password using Blake2b with a salt for improved security.
        /// </summary>
        /// <param name="password">The password to hash. Must not be null or empty.</param>
        /// <returns>A result containing the hashed password and salt.</returns>
        public Result<HashedPassword> HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8) // Basic input validation
                return Result<HashedPassword>.Failure("Password is null or does not meet complexity requirements.");

            var salt = GenerateRandomSalt();
            var hash = HashWithBlake2(password, salt);
            var combinedBytes = CombineBytes(salt, hash);
            var base64Hash = Convert.ToBase64String(combinedBytes);

            return Result<HashedPassword>.Success(new HashedPassword(base64Hash, salt));
        }

        /// <summary>
        /// Verifies a password against a stored hash with enhanced error handling.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="expectedCombinedHash">The stored combined hash and salt.</param>
        /// <returns>A result indicating if the password is correct or not.</returns>
        public Result VerifyPassword(string password, string expectedCombinedHash)
        {
            byte[] combinedBytes;
            try
            {
                combinedBytes = Convert.FromBase64String(expectedCombinedHash);
            }
            catch (FormatException)
            {
                return Result.Failure("Invalid hash format.");
            }

            var (salt, expectedHash) = ExtractBytes(combinedBytes, SaltSize);
            var hash = HashWithBlake2(password, salt);

            var isValid = StructuralComparisons.StructuralEqualityComparer.Equals(hash, expectedHash);
            return isValid ? Result.Success() : Result.Failure("Password is incorrect");
        }

        /// <summary>
        /// Hashes a password using Blake2b with a prepended salt for improved security.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt to prepend to the password before hashing.</param>
        /// <returns>The hashed password as a byte array.</returns>
        private static byte[] HashWithBlake2(string password, byte[] salt)
        {
            // Prepend the salt to the password before hashing
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[salt.Length + passwordBytes.Length];
            Buffer.BlockCopy(salt, 0, saltedPassword, 0, salt.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, salt.Length, passwordBytes.Length);

            // Create and compute the hash
            var hash = Blake2b.ComputeHash(HashSize, saltedPassword);
            return hash;
        }

        private static byte[] GenerateRandomSalt()
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
}
