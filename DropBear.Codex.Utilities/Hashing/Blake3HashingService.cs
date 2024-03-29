using System.Text;
using Blake3;
using DropBear.Codex.Core.ReturnTypes;
using DropBear.Codex.Utilities.Hashing.Interfaces;

namespace DropBear.Codex.Utilities.Hashing;

public class Blake3HashingService : IHashingService
{
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

    // Additional functionalities like MAC generation and key derivation are not directly covered by IHashingService and would require expanding the interface or creating specific methods for those purposes.
}
