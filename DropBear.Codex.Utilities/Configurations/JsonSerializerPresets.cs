#region

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace DropBear.Codex.Utilities.Configurations;

public static class JsonSerializerPresets
{
    public static JsonSerializerOptions GetDefault()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Indent the output for better readability
            IncludeFields = true, // Include public fields in serialization
            PropertyNameCaseInsensitive = true, // Ignore case when matching property names
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Ignore null values when serializing
            NumberHandling = JsonNumberHandling.AllowReadingFromString, // Allow reading numbers from strings
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Use a more relaxed JSON encoder
            ReferenceHandler = ReferenceHandler.Preserve, // Preserve reference relationships
            MaxDepth = 64, // Set a maximum depth to avoid StackOverflowExceptions
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode, // Handle unknown types as JsonNode
            Converters =
            {
                new JsonStringEnumConverter() // Use a converter for enums
                // Add more custom converters here if needed
            }
        };

        return options;
    }
}
