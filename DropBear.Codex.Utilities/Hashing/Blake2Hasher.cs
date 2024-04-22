using System.Collections;
using System.Text;
using Blake2Fast;
using DropBear.Codex.Core;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.Helpers;

namespace DropBear.Codex.Utilities.Hashing;

public class Blake2Hasher : IHasher
{
    private int _hashSize = 32; // Default hash size for Blake2b
    private byte[]? _salt;

    public IHasher WithSalt(byte[] salt)
    {
        _salt = salt;
        return this;
    }

    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        if (_salt is null || _salt.Length is 0)
            _salt = HashingHelper.GenerateRandomSalt(32); // Default to 32 bytes if not set explicitly

        var hashBytes = HashWithBlake2(input, _salt);
        var combinedBytes = HashingHelper.CombineBytes(_salt, hashBytes);
        return Result<string>.Success(Convert.ToBase64String(combinedBytes));
    }

    public Result Verify(string input, string expectedHash)
    {
        try
        {
            if (_salt is null || _salt.Length is 0)
                return Result.Failure("Salt is required for verification.");
            var expectedBytes = Convert.FromBase64String(expectedHash);
            var (salt, expectedHashBytes) = HashingHelper.ExtractBytes(expectedBytes, _salt.Length);
            var hashBytes = HashWithBlake2(input, salt);

            return StructuralComparisons.StructuralEqualityComparer.Equals(hashBytes, expectedHashBytes)
                ? Result.Success()
                : Result.Failure("Verification failed.");
        }
        catch (FormatException)
        {
            return Result.Failure("Expected hash format is invalid.");
        }
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        var hash = Blake2b.ComputeHash(_hashSize, data);
        return Result<string>.Success(Convert.ToBase64String(hash));
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var hash = Blake2b.ComputeHash(_hashSize, data);
        var base64Hash = Convert.ToBase64String(hash);

        return base64Hash == expectedBase64Hash
            ? Result.Success()
            : Result.Failure("Base64 hash verification failed.");
    }

    public IHasher WithIterations(int iterations) =>
        // Blake2b does not use the iterations parameter, so this is effectively a noop.
        this;

    public IHasher WithHashSize(int size)
    {
        _hashSize = size;
        return this;
    }

    private byte[] HashWithBlake2(string input, byte[] salt)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var saltedInput = HashingHelper.CombineBytes(salt, inputBytes);
        return Blake2b.ComputeHash(_hashSize, saltedInput);
    }
}
