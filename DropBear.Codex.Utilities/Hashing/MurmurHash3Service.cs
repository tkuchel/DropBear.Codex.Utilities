using System.Text;
using DropBear.Codex.Core;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using HashDepot;

namespace DropBear.Codex.Utilities.Hashing;

public class MurmurHash3Service : IHashingService
{
    private uint _seed; // Allows for seed to be changed via fluent API

    public MurmurHash3Service(uint seed = 0) => _seed = seed;

    public IHashingService WithSalt(byte[] salt) =>
        // MurmurHash3 does not use salt, so this method is effectively a noop.
        this;

    public IHashingService WithIterations(int iterations) =>
        // MurmurHash3 does not use iterations, so this method is effectively a noop.
        this;

    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        var buffer = Encoding.UTF8.GetBytes(input);
        var hash = MurmurHash3.Hash32(buffer, _seed);
        return Result<string>.Success(hash.ToString("x8"));
    }

    public Result Verify(string input, string expectedHash)
    {
        var hashResult = Hash(input);
        if (!hashResult.IsSuccess)
            return Result.Failure("Failed to compute hash.");

        var isValid = string.Equals(hashResult.Value, expectedHash, StringComparison.OrdinalIgnoreCase);
        return isValid ? Result.Success() : Result.Failure("Verification failed.");
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data.Length is 0)
            return Result<string>.Failure("Data cannot be null or empty.");

        try
        {
            var hash = MurmurHash3.Hash32(data, _seed);
            var hashBytes = BitConverter.GetBytes(hash);
            var base64Hash = Convert.ToBase64String(hashBytes);
            return Result<string>.Success(base64Hash);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during hashing: {ex.Message}");
        }
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var encodeResult = EncodeToBase64Hash(data);
        if (!encodeResult.IsSuccess)
            return Result.Failure("Failed to compute hash.");

        var isValid = string.Equals(encodeResult.Value, expectedBase64Hash, StringComparison.Ordinal);
        return isValid ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }

    public IHashingService WithHashSize(int size) =>
        // MurmurHash3 output size is determined by the algorithm (32-bit or 128-bit), so this method is effectively a noop.
        this;

    public IHashingService WithSeed(uint seed)
    {
        _seed = seed;
        return this;
    }
}
