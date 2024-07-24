namespace DropBear.Codex.Utilities.Helpers;

public static class ArrayHelper
{
    public static void PrintByteArray(byte[] bytes)
    {
        const int BytesPerLine = 16;
        var bytesRemaining = bytes.Length;
        var bytesRead = 0;

        while (bytesRemaining > 0)
        {
            Console.Write("{0:X8}: ", bytesRead);

            var bytesToPrint = Math.Min(BytesPerLine, bytesRemaining);

            for (var i = 0; i < bytesToPrint; i++)
            {
                Console.Write("{0:X2} ", bytes[bytesRead + i]);
            }

            for (var i = bytesToPrint; i < BytesPerLine; i++)
            {
                Console.Write("   ");
            }

            Console.Write(" ");

            for (var i = 0; i < bytesToPrint; i++)
            {
                var b = bytes[bytesRead + i];
                var c = b < 32 || b > 126 ? '.' : (char)b;
                Console.Write(c);
            }

            Console.WriteLine();

            bytesRead += bytesToPrint;
            bytesRemaining -= bytesToPrint;
        }
    }

    public static void CompareBytesArrays(byte[] array1, byte[] array2)
    {
        Console.WriteLine("Comparing byte arrays...");

        // Check if the lengths are different
        if (array1.Length != array2.Length)
        {
            Console.WriteLine($"The arrays have different lengths: {array1.Length} and {array2.Length}.");
        }

        var diffCount = 0;
        var bytesPerLine = 16;
        var maxLength = Math.Max(array1.Length, array2.Length);

        for (var i = 0; i < maxLength; i += bytesPerLine)
        {
            var bytesToPrint = Math.Min(bytesPerLine, maxLength - i);

            // Print the offset
            Console.Write($"{i:X8}: ");

            // Print the bytes for array1
            for (var j = 0; j < bytesToPrint; j++)
            {
                var index = i + j;
                if (index < array1.Length)
                {
                    Console.Write($"{array1[index]:X2} ");
                }
                else
                {
                    Console.Write("   ");
                }
            }

            Console.Write(" ");

            // Print the bytes for array2
            for (var j = 0; j < bytesToPrint; j++)
            {
                var index = i + j;
                if (index < array2.Length)
                {
                    Console.Write($"{array2[index]:X2} ");
                    if (index < array1.Length && array1[index] != array2[index])
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("*");
                        Console.ResetColor();
                        diffCount++;
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                else
                {
                    Console.Write("   ");
                }
            }

            Console.WriteLine();
        }

        Console.WriteLine($"Total differences: {diffCount}");
    }
}
