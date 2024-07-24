#region

using System.Security.Cryptography;
using System.Text;
using DropBear.Codex.Utilities.Converters;

#endregion

namespace DropBear.Codex.Utilities.Encryption;

public static class RudimentaryEncryptor
{
    private static string GenerateHash(string data)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashBytes).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    public static string Encode(string value)
    {
        var binary = BinaryAndHexConverter.StringToBinary(value);
        var hash = GenerateHash(value);
        var hashBinary = BinaryAndHexConverter.StringToBinary(hash);

        var binaryWithHash = binary + hashBinary;
        var pseudoRandomSequence = GeneratePseudoRandomSequence(binaryWithHash.Length);

        var obfuscatedBinary = new StringBuilder(binaryWithHash.Length);
        foreach (var (c, random) in binaryWithHash.Zip(pseudoRandomSequence))
        {
            obfuscatedBinary.Append((c == '0' ? '1' : '0') ^ (random % 2));
        }

        return BinaryAndHexConverter.BinaryToHex(obfuscatedBinary.ToString());
    }

    public static string Decode(string obfuscatedValue)
    {
        var binary = BinaryAndHexConverter.HexToBinary(obfuscatedValue);
        var pseudoRandomSequence = GeneratePseudoRandomSequence(binary.Length);

        var deobfuscatedBinary = new StringBuilder(binary.Length);
        foreach (var (c, random) in binary.Zip(pseudoRandomSequence))
        {
            deobfuscatedBinary.Append((c ^ (random % 2)) == '0' ? '1' : '0');
        }

        var originalBinary = deobfuscatedBinary.ToString()[..^256];
        var hashBinary = deobfuscatedBinary.ToString()[^256..];
        var originalValue = BinaryAndHexConverter.BinaryToString(originalBinary);
        var hash = BinaryAndHexConverter.BinaryToString(hashBinary);

        if (GenerateHash(originalValue) != hash)
        {
            throw new InvalidOperationException("Data integrity check failed - hash mismatch.");
        }

        return originalValue;
    }

    private static List<int> GeneratePseudoRandomSequence(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[4]; // Buffer for 4 bytes, which is enough for an Int32
        var randomNumbers = new List<int>(length); // Preallocate list to store random numbers

        for (var i = 0; i < length; i++)
        {
            rng.GetBytes(randomNumber); // Fill buffer with secure random bytes
            randomNumbers.Add(BitConverter.ToInt32(randomNumber, 0)); // Convert bytes to an integer and add to list
        }

        return randomNumbers; // Return the fully created list of random numbers
    }
}
