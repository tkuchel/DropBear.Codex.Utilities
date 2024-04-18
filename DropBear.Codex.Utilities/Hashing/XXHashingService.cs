using System.Text;
using DropBear.Codex.Core;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using HashDepot;

namespace DropBear.Codex.Utilities.Hashing;

public class XxHashingService : IHashingService
{
    private ulong _seed; // Allows setting a custom seed if needed, default is 0.

    public IHashingService WithSalt(byte[] salt) =>
        // XXHash does not use salt, so this method is effectively a noop.
        this;

    public IHashingService WithIterations(int iterations) =>
        // XXHash does not use iterations, so this method is effectively a noop.
        this;

    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        var buffer = Encoding.UTF8.GetBytes(input);
        var hash = XXHash.Hash64(buffer, _seed); // Uses the custom seed if set.
        return Result<string>.Success(hash.ToString("x8"));
    }

    public Result Verify(string input, string expectedHash)
    {
        var hashResult = Hash(input);
        if (!hashResult.IsSuccess)
            return Result.Failure("Failed to compute hash.");

        return hashResult.Value == expectedHash ? Result.Success() : Result.Failure("Verification failed.");
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data.Length is 0)
            return Result<string>.Failure("Data cannot be null or empty.");

        var hash = XXHash.Hash64(data, _seed); // Uses the custom seed.
        var hashBytes = BitConverter.GetBytes(hash);
        var base64Hash = Convert.ToBase64String(hashBytes);
        return Result<string>.Success(base64Hash);
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var encodeResult = EncodeToBase64Hash(data);
        if (!encodeResult.IsSuccess)
            return Result.Failure("Failed to compute hash.");

        return encodeResult.Value == expectedBase64Hash
            ? Result.Success()
            : Result.Failure("Base64 hash verification failed.");
    }

    public IHashingService WithHashSize(int size) =>
        // XXHash output size is determined by the algorithm (32-bit or 64-bit), so this method is effectively a noop.
        this;

    public IHashingService WithSeed(ulong seed)
    {
        _seed = seed;
        return this;
    }
}
