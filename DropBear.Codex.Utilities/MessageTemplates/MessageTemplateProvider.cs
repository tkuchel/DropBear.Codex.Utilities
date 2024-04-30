using System.Buffers;
using System.Collections.Concurrent;
using System.Text;
using DropBear.Codex.AppLogger.Builders;
using DropBear.Codex.Core;
using DropBear.Codex.Utilities.MessageTemplates.Interfaces;
using Microsoft.Extensions.Logging;
using Utf8StringInterpolation;
using ZLogger;

namespace DropBear.Codex.Utilities.MessageTemplates;

/// <summary>
///     Manages message templates and predefined messages for logging and formatting operations.
/// </summary>
public class MessageTemplateProvider : IMessageTemplateProvider
{
    private readonly object _formatLock = new();
    private readonly ILogger<MessageTemplateProvider> _logger;
    private readonly ConcurrentDictionary<string, string> _predefinedMessages = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, string> _templates = new(StringComparer.OrdinalIgnoreCase);

    public MessageTemplateProvider()
    {
        var loggerFactory = new LoggerConfigurationBuilder()
            .SetLogLevel(LogLevel.Information)
            .EnableConsoleOutput()
            .Build();

        _logger = loggerFactory.CreateLogger<MessageTemplateProvider>();
    }


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
    public Result RegisterTemplate(string templateId, string template)
    {
        var validationResult = ValidateInput(templateId, template, "Template");
        if (!validationResult.IsSuccess) return validationResult;

        lock (_formatLock)
        {
            if (_predefinedMessages.ContainsKey(templateId))
                return Result.Failure($"A predefined message with the ID '{templateId}' has already been registered.");

            var added = _templates.TryAdd(templateId, template);
            if (!added)
                return Result.Failure($"A template with the ID '{templateId}' has already been registered.");
        }

        _logger.ZLogInformation($"Successfully registered template '{templateId}'.");
        return Result.Success();
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

    public Result RegisterTemplates(Dictionary<string, string> templatesToRegister)
    {
        List<string> errors = [];
        foreach (var (templateId, template) in templatesToRegister)
        {
            var result = RegisterTemplate(templateId, template);
            if (!result.IsSuccess) errors.Add($"Failed to register template '{templateId}': {result.ErrorMessage}");
        }

        return errors.Count is not 0 ? Result.Failure(string.Join('\n', errors)) : Result.Success();
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
    public Result RegisterPredefinedMessage(string messageId, string message)
    {
        var validationResult = ValidateInput(messageId, message, "Predefined Message");
        if (!validationResult.IsSuccess) return validationResult;

        lock (_formatLock)
        {
            if (_templates.ContainsKey(messageId))
                return Result.Failure($"A template with the ID '{messageId}' has already been registered.");

            var added = _predefinedMessages.TryAdd(messageId, message);
            if (!added)
                return Result.Failure($"A predefined message with the ID '{messageId}' has already been registered.");
        }

        _logger.ZLogInformation($"Successfully registered predefined message '{messageId}'.");
        return Result.Success();
    }


    public Result RegisterPredefinedMessages(Dictionary<string, string> messagesToRegister)
    {
        List<string> errors = [];
        foreach (var (messageId, message) in messagesToRegister)
        {
            var result = RegisterPredefinedMessage(messageId, message);
            if (!result.IsSuccess) errors.Add($"Failed to register predefined message '{messageId}': {result.ErrorMessage}");
        }

        return errors.Count is not 0 ? Result.Failure(string.Join('\n', errors)) : Result.Success();
    }

    private static Result ValidateInput(string id, string value, string type)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(value))
            return Result.Failure($"{type} ID and value cannot be null or whitespace.");
        return Result.Success();
    }
}
