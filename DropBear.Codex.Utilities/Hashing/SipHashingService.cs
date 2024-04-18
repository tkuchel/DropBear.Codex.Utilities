using System.Text;
using DropBear.Codex.Core;
using DropBear.Codex.Utilities.Hashing.Interfaces;
using HashDepot;

namespace DropBear.Codex.Utilities.Hashing;

public class SipHashingService : IHashingService
{
    private byte[] _key;

    public SipHashingService(byte[] key)
    {
        if ((key?.Length) is not 16)
            throw new ArgumentException("Key must be 16 bytes in length.", nameof(key));
        _key = key;
    }

    public IHashingService WithSalt(byte[] salt) =>
        // SipHash does not use salt, so this method is effectively a noop.
        this;

    public IHashingService WithIterations(int iterations) =>
        // SipHash does not use iterations, so this method is effectively a noop.
        this;

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

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var encodeResult = EncodeToBase64Hash(data);
        if (!encodeResult.IsSuccess)
            return Result.Failure("Failed to compute hash.");

        var isValid = string.Equals(encodeResult.Value, expectedBase64Hash, StringComparison.Ordinal);
        return isValid ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }

    public IHashingService WithKey(byte[] key)
    {
        if ((key?.Length) is not 16)
            throw new ArgumentException("Key must be 16 bytes in length.", nameof(key));
        _key = key;
        return this;
    }

    public IHashingService WithHashSize(int size) =>
        // SipHash output size is fixed by the algorithm, so this method is effectively a noop.
        this;
}
