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

            for (var i = 0; i < bytesToPrint; i++) Console.Write("{0:X2} ", bytes[bytesRead + i]);

            for (var i = bytesToPrint; i < BytesPerLine; i++) Console.Write("   ");

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
}
