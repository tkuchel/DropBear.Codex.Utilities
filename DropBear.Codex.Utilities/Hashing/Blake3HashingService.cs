using System.Text;
using Blake3;
using DropBear.Codex.Core;
using DropBear.Codex.Utilities.Hashing.Interfaces;

namespace DropBear.Codex.Utilities.Hashing;

public class Blake3HashingService : IHashingService
{
    public IHashingService WithSalt(byte[] salt) =>
        // Blake3 does not use salt, so this method is effectively a noop.
        this;

    public IHashingService WithIterations(int iterations) =>
        // Blake3 does not use iterations, so this method is effectively a noop.
        this;

    public Result<string> Hash(string input)
    {
        try
        {
            var hash = Hasher.Hash(Encoding.UTF8.GetBytes(input)).ToString();
            return Result<string>.Success(hash);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during hashing: {ex.Message}");
        }
    }

    public Result Verify(string input, string expectedHash)
    {
        try
        {
            var hash = Hasher.Hash(Encoding.UTF8.GetBytes(input)).ToString();
            return hash == expectedHash ? Result.Success() : Result.Failure("Verification failed.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error during verification: {ex.Message}");
        }
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        try
        {
            var hash = Hasher.Hash(data);
            return Result<string>.Success(Convert.ToBase64String(hash.AsSpan()));
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during base64 encoding hash: {ex.Message}");
        }
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        try
        {
            var hash = Hasher.Hash(data);
            var base64Hash = Convert.ToBase64String(hash.AsSpan());
            return base64Hash == expectedBase64Hash
                ? Result.Success()
                : Result.Failure("Base64 hash verification failed.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error during base64 hash verification: {ex.Message}");
        }
    }

    public IHashingService WithHashSize(int size) =>
        // Blake3 has a fixed output size but implementing to comply with interface.
        this;
}
