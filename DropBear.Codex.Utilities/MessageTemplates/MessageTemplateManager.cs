using System.Buffers;
using System.Collections.Concurrent;
using System.Text;
using Utf8StringInterpolation;

namespace DropBear.Codex.Utilities.MessageTemplates;

/// <summary>
///     Manages message templates and predefined messages for logging and formatting operations.
/// </summary>
public class MessageTemplateManager : IMessageTemplateManager
{
    private readonly object _formatLock = new();
    private readonly ConcurrentDictionary<string, string> _predefinedMessages = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _templates = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Registers a message template with the specified ID.
    /// </summary>
    /// <param name="templateId">The unique identifier for the template.</param>
    /// <param name="template">The template string.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when templateId is null or whitespace, or when template is null or
    ///     whitespace.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when a template or predefined message with the same ID has already
    ///     been registered.
    /// </exception>
    public void RegisterTemplate(string templateId, string template)
    {
        if (string.IsNullOrWhiteSpace(templateId))
            throw new ArgumentException("Template ID cannot be null or whitespace.", nameof(templateId));
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("Template cannot be null or whitespace.", nameof(template));

        if (!_templates.TryAdd(templateId, template) || _predefinedMessages.ContainsKey(templateId))
            throw new InvalidOperationException(
                $"A template or predefined message with the ID '{templateId}' has already been registered.");
    }

    public byte[] FormatUtf8(string templateOrMessageId, params object?[] args)
    {
        lock (_formatLock)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(templateOrMessageId))
                    throw new ArgumentException("Template or message ID cannot be null or whitespace.",
                        nameof(templateOrMessageId));

                if (_predefinedMessages.TryGetValue(templateOrMessageId, out var predefinedMessage))
                {
                    if (args.Length > 0)
                        throw new ArgumentException(
                            $"Predefined message '{templateOrMessageId}' does not accept arguments.", nameof(args));

                    return Encoding.UTF8.GetBytes(predefinedMessage);
                }

                if (!_templates.TryGetValue(templateOrMessageId, out var template))
                    throw new KeyNotFoundException(
                        $"No template or predefined message found with the ID '{templateOrMessageId}'.");

                var bufferWriter = new ArrayBufferWriter<byte>();
                using (var writer = Utf8String.CreateWriter(bufferWriter))
                {
                    writer.AppendLiteral(template);
                    foreach (var arg in args) writer.AppendFormatted(arg ?? "null");
                }

                return bufferWriter.WrittenSpan.ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to format the template or predefined message.", ex);
            }
        }
    }

    public string FormatString(string templateOrMessageId, params object?[] args)
    {
        var utf8Bytes = FormatUtf8(templateOrMessageId, args);
        return Encoding.UTF8.GetString(utf8Bytes);
    }

    public void RegisterTemplates(Dictionary<string, string> templatesToRegister)
    {
        foreach (var (key, value) in templatesToRegister)
            RegisterTemplate(key, value);
    }

    /// <summary>
    ///     Registers a predefined message with the specified ID.
    /// </summary>
    /// <param name="messageId">The unique identifier for the predefined message.</param>
    /// <param name="message">The predefined message string.</param>
    /// <exception cref="ArgumentException">Thrown when messageId is null or whitespace, or when message is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when a template or predefined message with the same ID has already
    ///     been registered.
    /// </exception>
    public void RegisterPredefinedMessage(string messageId, string message)
    {
        if (string.IsNullOrWhiteSpace(messageId))
            throw new ArgumentException("Message ID cannot be null or whitespace.", nameof(messageId));
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));

        if (!_predefinedMessages.TryAdd(messageId, message) || _templates.ContainsKey(messageId))
            throw new InvalidOperationException(
                $"A template or predefined message with the ID '{messageId}' has already been registered.");
    }

    public void RegisterPredefinedMessages(Dictionary<string, string> messagesToRegister)
    {
        foreach (var (key, value) in messagesToRegister)
            RegisterPredefinedMessage(key, value);
    }
}
