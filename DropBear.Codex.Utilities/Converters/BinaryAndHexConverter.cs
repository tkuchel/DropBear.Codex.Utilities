using System.Globalization;
using System.Text;

namespace DropBear.Codex.Utilities.Converters;

/// <summary>
///     Provides methods to convert between string, binary, and hexadecimal representations.
/// </summary>
public static class BinaryAndHexConverter
{
    private static readonly Dictionary<string, string> BinaryToHexTable = new(StringComparer.Ordinal)
    {
        { "0000", "0" },
        { "0001", "1" },
        { "0010", "2" },
        { "0011", "3" },
        { "0100", "4" },
        { "0101", "5" },
        { "0110", "6" },
        { "0111", "7" },
        { "1000", "8" },
        { "1001", "9" },
        { "1010", "a" },
        { "1011", "b" },
        { "1100", "c" },
        { "1101", "d" },
        { "1110", "e" },
        { "1111", "f" }
    };

    private static readonly Dictionary<string, string> HexToBinaryTable = new(StringComparer.Ordinal)
    {
        { "0", "0000" },
        { "1", "0001" },
        { "2", "0010" },
        { "3", "0011" },
        { "4", "0100" },
        { "5", "0101" },
        { "6", "0110" },
        { "7", "0111" },
        { "8", "1000" },
        { "9", "1001" },
        { "a", "1010" },
        { "b", "1011" },
        { "c", "1100" },
        { "d", "1101" },
        { "e", "1110" },
        { "f", "1111" }
    };

    /// <summary>
    ///     Converts a string to its binary representation.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The binary representation of the input string.</returns>
    public static string StringToBinary(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        return string.Join(string.Empty,
            Encoding.UTF8.GetBytes(value).Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

    /// <summary>
    ///     Converts a binary string to its original string representation.
    /// </summary>
    /// <param name="value">The binary string to convert.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>The original string representation.</returns>
    public static string BinaryToString(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length % 8 is not 0)
            throw new ArgumentException("Binary string is not valid.", nameof(value));

        var bytes = Enumerable.Range(0, value.Length / 8)
            .Select(i => Convert.ToByte(value.Substring(i * 8, 8), 2))
            .ToArray();

        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    ///     Converts a binary string to its hexadecimal representation using a lookup table.
    /// </summary>
    public static string BinaryToHex(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        var hex = new StringBuilder();
        for (var i = 0; i < value.Length; i += 4)
        {
            var nibble = value.Substring(i, 4);
            hex.Append(BinaryToHexTable[nibble]);
        }

        return hex.ToString().ToUpper(CultureInfo.CurrentCulture);
    }

    /// <summary>
    ///     Converts a hexadecimal string to its binary representation using a lookup table.
    /// </summary>
    public static string HexToBinary(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;

        var binary = new StringBuilder();
        foreach (var c in value.ToLower(CultureInfo.CurrentCulture)) binary.Append(HexToBinaryTable[c.ToString()]);

        return binary.ToString();
    }

    /// <summary>
    ///     Converts a string to a binary byte array.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>A byte array representing the binary data of the string.</returns>
    public static byte[] StringToBinaryBytes(string value)
    {
        if (string.IsNullOrEmpty(value)) return Array.Empty<byte>();

        var bytes = Encoding.UTF8.GetBytes(value);
        return bytes; // Directly return the byte array without conversion
    }

    /// <summary>
    ///     Converts a binary byte array back to its string representation.
    /// </summary>
    /// <param name="value">The binary byte array to convert.</param>
    /// <returns>The string representation of the binary data.</returns>
    public static string BinaryBytesToString(byte[]? value)
    {
        if (value is null || value.Length is 0) return string.Empty;

        var result = Encoding.UTF8.GetString(value);
        return result;
    }

    /// <summary>
    ///     Converts a string to a hexadecimal byte array.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>A byte array representing the hexadecimal data of the string.</returns>
    public static byte[] StringToHexBytes(string value)
    {
        if (string.IsNullOrEmpty(value)) return Array.Empty<byte>();

        var hexStringBuilder = new StringBuilder();
        foreach (var b in Encoding.UTF8.GetBytes(value)) hexStringBuilder.Append(b.ToString("x2"));

        return Enumerable.Range(0, hexStringBuilder.Length / 2)
            .Select(i => Convert.ToByte(hexStringBuilder.ToString().Substring(i * 2, 2), 16))
            .ToArray();
    }

    /// <summary>
    ///     Converts a hexadecimal byte array back to its string representation.
    /// </summary>
    /// <param name="value">The hexadecimal byte array to convert.</param>
    /// <returns>The string representation of the hexadecimal data.</returns>
    public static string HexBytesToString(byte[]? value)
    {
        if (value is null || value.Length is 0) return string.Empty;

        var stringBuilder = new StringBuilder();
        foreach (var b in value) stringBuilder.Append(Convert.ToString(b, 16).PadLeft(2, '0'));

        return stringBuilder.ToString();
    }

    /// <summary>
    ///     Converts a hexadecimal string to a byte array.
    /// </summary>
    /// <param name="hex">The hexadecimal string to convert.</param>
    /// <returns>A byte array representing the bytes of the hexadecimal string.</returns>
    /// <exception cref="ArgumentException">Thrown if the hex string is null, empty, or has an odd length.</exception>
    public static byte[] HexToByteArray(string hex)
    {
        if (string.IsNullOrEmpty(hex)) throw new ArgumentException("Hex string cannot be null or empty.", nameof(hex));

        if (hex.Length % 2 is not 0) throw new ArgumentException("Hex string must have an even length.", nameof(hex));

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2) bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    /// <summary>
    ///     Converts a byte array to its hexadecimal string representation.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>A string representing the hexadecimal representation of the byte array.</returns>
    /// <exception cref="ArgumentException">Thrown if the byte array is null or empty.</exception>
    public static string ByteArrayToHex(byte[]? bytes)
    {
        if (bytes is null || bytes.Length is 0)
            throw new ArgumentException("Byte array cannot be null or empty.", nameof(bytes));

        var hex = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) hex.Append(CultureInfo.CurrentCulture, $"{b:x2}");

        return hex.ToString();
    }

    /// <summary>
    ///     Validates if the given string is a valid binary representation.
    /// </summary>
    /// <param name="value">The binary string to validate.</param>
    /// <returns>True if the string is a valid binary representation; otherwise, false.</returns>
    public static bool IsValidBinaryString(string value) =>
        !string.IsNullOrEmpty(value) && value.All(c => c == '0' || c == '1');

    /// <summary>
    ///     Converts a hexadecimal string to its original string representation.
    /// </summary>
    /// <param name="value">The hexadecimal string to convert.</param>
    /// <returns>The original string representation.</returns>
    public static string HexToString(string value) => BinaryToString(HexToBinary(value));

    /// <summary>
    ///     Validates if the given string is a valid hexadecimal representation.
    /// </summary>
    /// <param name="value">The hexadecimal string to validate.</param>
    /// <returns>True if the string is a valid hexadecimal representation; otherwise, false.</returns>
    public static bool IsValidHexString(string value) => !string.IsNullOrEmpty(value) &&
                                                         value.All(c =>
                                                             "0123456789ABCDEFabcdef".Contains(c,
                                                                 StringComparison.Ordinal));

    /// <summary>
    ///     Converts a string to its hexadecimal representation.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The hexadecimal representation of the input string.</returns>
    public static string StringToHex(string value) => BinaryToHex(StringToBinary(value));

    /// <summary>
    ///     Performs a bitwise AND operation on two binary strings.
    /// </summary>
    /// <param name="binary1">The first binary string.</param>
    /// <param name="binary2">The second binary string.</param>
    /// <returns>The result of the bitwise AND operation as a binary string.</returns>
    // ReSharper disable once InconsistentNaming
    public static string BitwiseAND(string binary1, string binary2) => PerformBitwiseOperation(binary1, binary2,
        (b1, b2) => (b1 & b2).ToString(CultureInfo.CurrentCulture));

    /// <summary>
    ///     Performs a bitwise OR operation on two binary strings.
    /// </summary>
    /// <param name="binary1">The first binary string.</param>
    /// <param name="binary2">The second binary string.</param>
    /// <returns>The result of the bitwise OR operation as a binary string.</returns>
    // ReSharper disable once InconsistentNaming
    public static string BitwiseOR(string binary1, string binary2) => PerformBitwiseOperation(binary1, binary2,
        (b1, b2) => (b1 | b2).ToString(CultureInfo.CurrentCulture));

    /// <summary>
    ///     Performs a bitwise XOR operation on two binary strings.
    /// </summary>
    /// <param name="binary1">The first binary string.</param>
    /// <param name="binary2">The second binary string.</param>
    /// <returns>The result of the bitwise XOR operation as a binary string.</returns>
    // ReSharper disable once InconsistentNaming
    public static string BitwiseXOR(string binary1, string binary2) =>
        PerformBitwiseOperation(binary1, binary2, (b1, b2) => (b1 ^ b2).ToString(CultureInfo.CurrentCulture));

    /// <summary>
    ///     Performs a bitwise NOT operation on a binary string.
    /// </summary>
    /// <param name="binary">The binary string to negate.</param>
    /// <returns>The result of the bitwise NOT operation as a binary string.</returns>
    // ReSharper disable once InconsistentNaming
    public static string BitwiseNOT(string binary) => string.Concat(binary.Select(b => b == '0' ? '1' : '0'));

    /// <summary>
    ///     Shifts a binary string to the left by a specified number of bits.
    /// </summary>
    /// <param name="binary">The binary string to shift.</param>
    /// <param name="shift">The number of bits to shift.</param>
    /// <returns>The shifted binary string.</returns>
    public static string ShiftLeft(string binary, int shift) =>
        binary.PadRight(binary.Length + shift, '0').Substring(0, binary.Length);

    /// <summary>
    ///     Shifts a binary string to the right by a specified number of bits.
    /// </summary>
    /// <param name="binary">The binary string to shift.</param>
    /// <param name="shift">The number of bits to shift.</param>
    /// <returns>The shifted binary string.</returns>
    public static string ShiftRight(string binary, int shift) =>
        binary.PadLeft(binary.Length + shift, '0').Substring(shift);

    /// <summary>
    ///     Performs a specified bitwise operation on two binary strings.
    /// </summary>
    /// <param name="binary1">The first binary string.</param>
    /// <param name="binary2">The second binary string.</param>
    /// <param name="operation">The bitwise operation to perform, represented as a Func delegate.</param>
    /// <returns>The result of performing the bitwise operation on the binary strings.</returns>
    private static string PerformBitwiseOperation(string binary1, string binary2, Func<int, int, string> operation)
    {
        // Ensure both binary strings are of equal length for bitwise operations
        var maxLength = Math.Max(binary1.Length, binary2.Length);
        binary1 = binary1.PadLeft(maxLength, '0');
        binary2 = binary2.PadLeft(maxLength, '0');

        var result = new StringBuilder();
        for (var i = 0; i < maxLength; i++)
        {
            // Convert each character to a number (0 or 1) and apply the operation
            var bit1 = binary1[i] - '0';
            var bit2 = binary2[i] - '0';
            result.Append(operation(bit1, bit2));
        }

        return result.ToString();
    }
}
