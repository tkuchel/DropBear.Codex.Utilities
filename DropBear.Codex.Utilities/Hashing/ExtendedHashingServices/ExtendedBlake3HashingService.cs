using Blake3;

namespace DropBear.Codex.Utilities.Hashing.ExtendedHashingServices;

public class ExtendedBlake3HashingService : Blake3HashingService
{
    public static string IncrementalHash(IEnumerable<byte[]> dataSegments)
    {
        using var hasher = Hasher.New();
        foreach (var segment in dataSegments)
            hasher.Update(segment);
        return hasher.Finalize().ToString();
    }

    public static string GenerateMac(byte[] data, byte[] key)
    {
        if (key.Length is not 32)
            throw new ArgumentException("Key must be 256 bits (32 bytes).", nameof(key));

        using var hasher = Hasher.NewKeyed(key);
        hasher.Update(data);
        return hasher.Finalize().ToString();
    }

    public static byte[] DeriveKey(byte[] context, byte[] inputKeyingMaterial)
    {
        using var hasher = Hasher.NewDeriveKey(context);
        hasher.Update(inputKeyingMaterial);
        var result = hasher.Finalize();
        return result.AsSpan().ToArray();
    }

    public static string HashStream(Stream inputStream)
    {
        using var hasher = Hasher.New();
        var buffer = new byte[4096]; // Buffer size can be adjusted based on needs.
        int bytesRead;
        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            hasher.Update(buffer.AsSpan(0, bytesRead));
        return hasher.Finalize().ToString();
    }
}
