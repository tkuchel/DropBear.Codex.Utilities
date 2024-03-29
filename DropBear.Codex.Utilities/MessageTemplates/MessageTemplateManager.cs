using System.Buffers;
using System.Collections.Concurrent;
using System.Text;
using Utf8StringInterpolation;

namespace DropBear.Codex.Utilities.MessageTemplates;

/// <summary>
/// Manages message templates for logging and formatting operations.
/// </summary>
public class MessageTemplateManager : IMessageTemplateManager
{
    private readonly ConcurrentDictionary<string, string> _templates = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers a message template with the specified ID.
    /// </summary>
    /// <param name="templateId">The unique identifier for the template.</param>
    /// <param name="template">The template string.</param>
    /// <exception cref="ArgumentException">Thrown when templateId is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when template is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a template with the same ID has already been registered.</exception>
    public void RegisterTemplate(string templateId, string template)
    {
        if (string.IsNullOrWhiteSpace(templateId))
            throw new ArgumentException("Template ID cannot be null or whitespace.", nameof(templateId));
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("Template cannot be null or whitespace.", nameof(template));

        if (!_templates.TryAdd(templateId, template))
            throw new InvalidOperationException($"A template with the ID '{templateId}' has already been registered.");
    }

    public byte[] FormatUtf8(string templateId, params object[] args)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(templateId))
                throw new ArgumentException("Template ID cannot be null or whitespace.", nameof(templateId));
            if (!_templates.TryGetValue(templateId, out var template))
                throw new KeyNotFoundException($"No template found with the ID '{templateId}'.");

            var bufferWriter = new ArrayBufferWriter<byte>();
            // Ensuring Utf8StringWriter is disposed correctly using 'using' statement
            using (var writer = Utf8String.CreateWriter(bufferWriter))
            {
                writer.AppendLiteral(template); // Assuming template is a literal part of the formatted message
                foreach (var arg in args) writer.AppendFormatted(arg);
            }

            // No need to manually call Flush() since it's implicitly called by Dispose()
            return bufferWriter.WrittenSpan.ToArray();
        }
        catch (Exception ex)
        {
            // Log the exception, consider rethrowing or handling it based on the use case
            throw new InvalidOperationException("Failed to format the template.", ex);
        }
    }

    public string FormatString(string templateId, params object[] args)
    {
        var utf8Bytes = FormatUtf8(templateId, args);
        return Encoding.UTF8.GetString(utf8Bytes);
    }

    public void RegisterTemplates(Dictionary<string, string> templatesToRegister)
    {
        foreach (var (key, value) in templatesToRegister) RegisterTemplate(key, value);
    }
}