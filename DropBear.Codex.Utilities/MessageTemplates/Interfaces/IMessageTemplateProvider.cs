using DropBear.Codex.Core;

namespace DropBear.Codex.Utilities.MessageTemplates.Interfaces;

public interface IMessageTemplateProvider
{
    /// <summary>
    /// Registers a message template with the specified ID.
    /// </summary>
    /// <param name="templateId">The unique identifier for the template.</param>
    /// <param name="template">The template string.</param>
    /// <exception cref="ArgumentException">Thrown when templateId is null or whitespace, or when template is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a template or predefined message with the same ID has already been registered.</exception>
    Result RegisterTemplate(string templateId, string template);

    /// <summary>
    /// Registers a predefined message with the specified ID.
    /// </summary>
    /// <param name="messageId">The unique identifier for the predefined message.</param>
    /// <param name="message">The predefined message string.</param>
    /// <exception cref="ArgumentException">Thrown when messageId is null or whitespace, or when message is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a template or predefined message with the same ID has already been registered.</exception>
    Result RegisterPredefinedMessage(string messageId, string message);

    /// <summary>
    /// Formats a template or predefined message with the provided arguments and returns the result as a byte array in UTF-8 encoding.
    /// </summary>
    /// <param name="templateOrMessageId">The ID of the template or predefined message to format.</param>
    /// <param name="args">The arguments to be used for formatting the template.</param>
    /// <returns>The formatted message as a byte array in UTF-8 encoding.</returns>
    /// <exception cref="ArgumentException">Thrown when templateOrMessageId is null or whitespace, or when arguments are provided for a predefined message.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no template or predefined message is found with the provided ID.</exception>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs during the formatting process.</exception>
    byte[] FormatUtf8(string templateOrMessageId, params object?[] args);

    /// <summary>
    /// Formats a template or predefined message with the provided arguments and returns the result as a string.
    /// </summary>
    /// <param name="templateOrMessageId">The ID of the template or predefined message to format.</param>
    /// <param name="args">The arguments to be used for formatting the template.</param>
    /// <returns>The formatted message as a string.</returns>
    /// <exception cref="ArgumentException">Thrown when templateOrMessageId is null or whitespace, or when arguments are provided for a predefined message.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no template or predefined message is found with the provided ID.</exception>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs during the formatting process.</exception>
    string FormatString(string templateOrMessageId, params object?[] args);

    /// <summary>
    /// Registers multiple templates from a dictionary.
    /// </summary>
    /// <param name="templatesToRegister">A dictionary containing the template IDs and their corresponding template strings.</param>
    Result RegisterTemplates(Dictionary<string, string> templatesToRegister);

    /// <summary>
    /// Registers multiple predefined messages from a dictionary.
    /// </summary>
    /// <param name="messagesToRegister">A dictionary containing the message IDs and their corresponding message strings.</param>
    Result RegisterPredefinedMessages(Dictionary<string, string> messagesToRegister);
}