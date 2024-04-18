using System.Collections;
using System.Text;
using DropBear.Codex.Core;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using DropBear.Codex.Utilities.Helpers;
using Konscious.Security.Cryptography;

namespace DropBear.Codex.Utilities.Hashing;

public class Argon2HashingService : IHashingService
{
    private int _degreeOfParallelism = 8; // Default degree of parallelism
    private int _hashSize = 16; // Default hash size
    private int _iterations = 4; // Default iterations
    private int _memorySize = 1024 * 1024; // Default memory size (1GB)
    private byte[]? _salt;

    public IHashingService WithSalt(byte[] salt)
    {
        _salt = salt;
        return this;
    }

    public IHashingService WithIterations(int iterations)
    {
        _iterations = iterations;
        return this;
    }

    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        if (_salt is null || _salt.Length is 0)
            _salt = HashingHelper.GenerateRandomSalt(32); // Default to 32 bytes if not set

        using var argon2 = CreateArgon2(input, _salt);
        var hashBytes = argon2.GetBytes(_hashSize);
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
            using var argon2 = CreateArgon2(input, salt);
            var hashBytes = argon2.GetBytes(_hashSize);

            return StructuralComparisons.StructuralEqualityComparer.Equals(hashBytes, expectedHashBytes)
                ? Result.Success()
                : Result.Failure("Verification failed.");
        }
        catch (FormatException)
        {
            return Result.Failure("Expected hash format is invalid.");
        }
    }

    public Result<string> EncodeToBase64Hash(byte[] data) =>
        Result<string>.Success(Convert.ToBase64String(data));

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var base64Hash = Convert.ToBase64String(data);
        return base64Hash == expectedBase64Hash ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }

    public IHashingService WithHashSize(int size)
    {
        _hashSize = size;
        return this;
    }

    public IHashingService WithDegreeOfParallelism(int degree)
    {
        _degreeOfParallelism = degree;
        return this;
    }

    public IHashingService WithMemorySize(int size)
    {
        _memorySize = size;
        return this;
    }

    private Argon2id CreateArgon2(string input, byte[] salt) =>
        new(Encoding.UTF8.GetBytes(input))
        {
            Salt = salt,
            DegreeOfParallelism = _degreeOfParallelism,
            Iterations = _iterations,
            MemorySize = _memorySize
        };
}
