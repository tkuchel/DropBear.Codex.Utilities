using DropBear.Codex.Core;

namespace DropBear.Codex.Utilities.Hashing.Interfaces;

public interface IHasher
{
    Result<string> Hash(string input);
    Result Verify(string input, string expectedHash);
    Result<string> EncodeToBase64Hash(byte[] data);
    Result VerifyBase64Hash(byte[] data, string expectedBase64Hash);

    // Fluent API extensions
    IHasher WithSalt(byte[] salt);
    IHasher WithIterations(int iterations);
}
