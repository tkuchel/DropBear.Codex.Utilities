### Example Usage of `HashingServiceFactory`

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using DropBear.Codex.Utilities.Hashing;
using DropBear.Codex.Utilities.Hashing.Interfaces;

public class HashingExample
{
 public static void Main(string[] args)
 {
 // Initialize the factory
 var factory = new HashingServiceFactory();

// Create a standard Blake3 hashing service
 var blake3Service = factory. CreateService("blake3");
 var hashResult = blake3Service.Hash("Hello World");
 Console.WriteLine($"Blake3 Hash: {hashResult.Value}");

// Create an extended Blake3 hashing service
 var extendedBlake3Service = factory. CreateService("extended_blake3") as ExtendedBlake3HashingService;
 if (extendedBlake3Service != null)
 {
 // Using the IncrementalHash method
 var dataSegments = new List<byte[]>
 {
 System.Text.Encoding.UTF8.GetBytes("Hello"),
 System.Text.Encoding.UTF8.GetBytes(" World")
 };
 var incrementalHash = extendedBlake3Service.IncrementalHash(dataSegments);
 Console.WriteLine($"Incremental Hash: {incrementalHash}");

// Generate a MAC
 var key = new byte[32]; // 256-bit key for Blake3 keyed hashing
 new Random(). NextBytes(key); // Generate a random key
 var mac = extendedBlake3Service.GenerateMac(System.Text.Encoding.UTF8.GetBytes("Hello World"), key);
 Console.WriteLine($"MAC: {mac}");

// Derive a subkey
 var context = System.Text.Encoding.UTF8.GetBytes("context");
 var subkey = extendedBlake3Service.DeriveKey(context, key);
 Console.WriteLine($"Derived Key: {BitConverter.ToString(subkey)}");

// Hash a stream
 using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Hello World"));
 var streamHash = extendedBlake3Service.HashStream(stream);
 Console.WriteLine($"Stream Hash: {streamHash}");
 }
 }
}
```

### Key Aspects of the Example:

1. **Factory Initialization**: A new instance of `HashingServiceFactory` is created.
2. **Service Creation**: The factory is used to create both a standard and an extended Blake3 hashing service.
3. **Basic Hashing**: Demonstrates hashing a simple string using `Blake3HashingService`.
4. **Extended Functionalities**:
- **Incremental Hashing**: Hashes multiple segments incrementally.
- **MAC Generation**: Generates a message authentication code using a provided key.
- **Key Derivation**: Derives a subkey from a given master key and context.
- **Stream Hashing**: Hashes data from a stream.