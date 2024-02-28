using System.Security.Cryptography;
using System.Text;
using DropBear.Codex.Utilities.Converters;

namespace DropBear.Codex.Utilities.Encryption;

public static class RudimentaryEncryptor
{
    private static int? s_key;

    public static string? Ulid
    {
        set
        {
            if (!string.IsNullOrEmpty(value)) s_key = ConvertUlidToIntSeed(value);
        }
    }

    /// <summary>
    ///     Converts a ULID string to an integer seed using SHA-256 for cryptographic hashing,
    ///     then condenses the hash into an integer seed.
    /// </summary>
    /// <param name="ulid">The ULID string to convert.</param>
    /// <returns>An integer seed derived from the cryptographic hash of the ULID.</returns>
    private static int ConvertUlidToIntSeed(string ulid)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(ulid));

        // Condense the SHA-256 hash into an int by taking the first 4 bytes
        // Note: This is a simple approach; other methods might involve using more bytes for the seed or different reduction strategies
        var seed = BitConverter.ToInt32(hashBytes, 0);

        return seed;
    }


    /// <summary>
    ///     Generates a SHA-256 hash for the given data.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>A hexadecimal string representing the SHA-256 hash of the data.</returns>
    private static string GenerateHash(string data)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty, StringComparison.Ordinal);
        return hash;
    }

    /// <summary>
    ///     Encodes the given value into an obfuscated hexadecimal string using a complex process.
    ///     Appends a SHA-256 hash for data integrity checks before obfuscation.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>An obfuscated hexadecimal string with a SHA-256 hash appended.</returns>
    public static string Encode(string value)
    {
        // Check if the key has been set; if not, throw an exception
        if (s_key is null) throw new InvalidOperationException("Encryption key has not been set.");


        // Step 1: Convert the input string to its binary representation.
        var binary = BinaryAndHexConverter.StringToBinary(value);

        // Step 2: Generate a SHA-256 hash of the original input and convert it to binary.
        var hash = GenerateHash(value);
        var hashBinary = BinaryAndHexConverter.StringToBinary(hash);

        // Step 3: Append the hash binary to the original binary data.
        var binaryWithHash = binary + hashBinary;

        // Step 4: Generate a pseudo-random sequence based on the key for obfuscation.
        var pseudoRandomSequence = GeneratePseudoRandomSequence(binaryWithHash.Length);

        // Step 5: Apply complex obfuscation logic to the binary data, including the hash.
        var obfuscatedBinary = new StringBuilder();
        for (var i = 0; i < binaryWithHash.Length; i++)
            if (pseudoRandomSequence[i] % 2 is 0)
                obfuscatedBinary.Append(binaryWithHash[i] == '0' ? '1' : '0');
            else
                obfuscatedBinary.Insert(0, binaryWithHash[i]);
        var furtherObfuscatedBinary = ApplyMixedReversalPatterns(obfuscatedBinary.ToString(), pseudoRandomSequence);

        // Step 6: Convert the obfuscated binary data, including the hash, back to a hexadecimal string.
        return BinaryAndHexConverter.BinaryToHex(furtherObfuscatedBinary);
    }

    /// <summary>
    ///     Decodes the given obfuscated hexadecimal string back to the original value and verifies the SHA-256 hash for data
    ///     integrity.
    /// </summary>
    /// <param name="obfuscatedValue">The obfuscated value to decode.</param>
    /// <returns>The original value if the data integrity check passes.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the data integrity check fails.</exception>
    public static string Decode(string obfuscatedValue)
    {
        // Check if the key has been set; if not, throw an exception
        if (s_key is null) throw new InvalidOperationException("Decryption key has not been set.");


        var binary = BinaryAndHexConverter.HexToBinary(obfuscatedValue);
        var deobfuscatedBinary = ReverseObfuscationLogic(binary);

        // Assuming the hash is the last 256 bits (SHA-256 hash size) of the binary string
        var originalBinary = deobfuscatedBinary[..^256];
        var hashBinary = deobfuscatedBinary[^256..];
        var originalValue = BinaryAndHexConverter.BinaryToString(originalBinary);
        var hash = BinaryAndHexConverter.BinaryToString(hashBinary);

        if (GenerateHash(originalValue) != hash)
            throw new InvalidOperationException("Data integrity check failed - hash mismatch.");

        return originalValue;
    }

    /// <summary>
    ///     Generates a sequence of pseudo-random integers based on a specified key.
    ///     This sequence is used to apply variable obfuscation patterns to the binary data.
    /// </summary>
    /// <param name="length">The desired length of the sequence.</param>
    /// <returns>An array of integers representing the pseudo-random sequence.</returns>
    private static int[] GeneratePseudoRandomSequence(int length)
    {
        // Check if the key has been set; if not, throw an exception

        var sequence = new int[length];
        // RNGCryptoServiceProvider does not use a seed like System.Random, so the key parameter will not be used here.
        using var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[4]; // 4 bytes for each int
        for (var i = 0; i < length; i++)
        {
            rng.GetBytes(randomNumber);
            sequence[i] = BitConverter.ToInt32(randomNumber, 0);
        }

        return sequence;
    }

    /// <summary>
    ///     Applies mixed reversal patterns to a binary string based on a pseudo-random sequence.
    /// </summary>
    /// <param name="binary">The binary string to apply patterns to.</param>
    /// <param name="pseudoRandomSequence">The sequence determining the pattern application.</param>
    /// <returns>The binary string after applying mixed reversal patterns.</returns>
    private static string ApplyMixedReversalPatterns(string binary, int[] pseudoRandomSequence)
    {
        var result = new StringBuilder(binary);
        for (var i = 0; i < binary.Length; i++)
            if (pseudoRandomSequence[i] % 2 == 0)
            {
                var segmentLength = Math.Min(4, binary.Length - i);
                var segment = new string(result.ToString().Substring(i, segmentLength).Reverse().ToArray());
                result.Remove(i, segmentLength).Insert(i, segment);
            }

        return result.ToString();
    }

    /// <summary>
    ///     Reverses the obfuscation logic applied during encoding to retrieve the original binary string.
    /// </summary>
    /// <param name="binary">The obfuscated binary string.</param>
    /// <returns>The original binary string before obfuscation.</returns>
    private static string ReverseObfuscationLogic(string binary)
    {
        var pseudoRandomSequence = GeneratePseudoRandomSequence(binary.Length);
        var preMixedReversalBinary = ReverseMixedReversalPatterns(binary, pseudoRandomSequence);

        var originalBinary = new StringBuilder();
        for (var i = preMixedReversalBinary.Length - 1; i >= 0; i--)
        {
            var bit = preMixedReversalBinary[i];
            originalBinary.Append(pseudoRandomSequence[preMixedReversalBinary.Length - 1 - i] % 2 is 0
                ? bit == '0' ? '1' : '0'
                : bit);
        }

        return originalBinary.ToString();
    }

    /// <summary>
    ///     Reverses mixed reversal patterns applied to a binary string based on a pseudo-random sequence.
    /// </summary>
    /// <param name="binary">The obfuscated binary string.</param>
    /// <param name="pseudoRandomSequence">The pseudo-random sequence used during obfuscation.</param>
    /// <returns>The binary string after reversing mixed reversal patterns.</returns>
    private static string ReverseMixedReversalPatterns(string binary, int[] pseudoRandomSequence)
    {
        var result = new StringBuilder(binary);
        for (var i = binary.Length - 1; i >= 0; i--)
            if (pseudoRandomSequence[i] % 2 is 0)
            {
                var segmentLength = Math.Min(4, binary.Length - i);
                var segmentStartIndex = Math.Max(0, i - segmentLength + 1);
                var segment = new string(result.ToString().Substring(segmentStartIndex, segmentLength).Reverse()
                    .ToArray());
                result.Remove(segmentStartIndex, segmentLength).Insert(segmentStartIndex, segment);
            }

        return result.ToString();
    }
}
