using DropBear.Codex.AppLogger.Builders;
using DropBear.Codex.Core;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace DropBear.Codex.Utilities.MessageTemplates.Builder;

public class MessageTemplateProviderBuilder
{
    private readonly ILogger<MessageTemplateProviderBuilder> _logger;
    private readonly Dictionary<string, string> _predefinedMessages = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _templates = new(StringComparer.OrdinalIgnoreCase);

    public MessageTemplateProviderBuilder()
    {
        var loggerFactory = new LoggerConfigurationBuilder()
            .SetLogLevel(LogLevel.Information)
            .EnableConsoleOutput()
            .Build();

        _logger = loggerFactory.CreateLogger<MessageTemplateProviderBuilder>();
    }

    private Result ValidateInput(string id, string value, string type)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Result.Failure($"{type} ID cannot be null or whitespace.");

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure($"{type} cannot be null or whitespace.");

        if (_templates.ContainsKey(id) || _predefinedMessages.ContainsKey(id))
            return Result.Failure($"A template or predefined message with the ID '{id}' has already been registered.");

        return Result.Success();
    }

    /// <summary>
    ///     Adds a single template to the builder's configuration.
    /// </summary>
    public MessageTemplateProviderBuilder AddTemplate(string templateId, string template)
    {
        var validationResult = ValidateInput(templateId, template, "Template");

        if (!validationResult.IsSuccess)
        {
            _logger.ZLogError($"Failed to add template: {validationResult.ErrorMessage}");
            return this; // Return immediately on failure
        }

        // Since we know the input is valid at this point, proceed with adding.
        _templates.Add(templateId, template);
        return this;
    }


    /// <summary>
    ///     Adds multiple templates to the builder's configuration.
    /// </summary>
    public MessageTemplateProviderBuilder AddTemplates(Dictionary<string, string> templates)
    {
        foreach (var template in templates)
            try
            {
                AddTemplate(template.Key, template.Value);
            }
            catch (InvalidOperationException ex)
            {
                // Handle or log the error if necessary
                // Optionally continue to the next template
                _logger.ZLogError(ex, $"Failed to add template '{template.Key}'");
            }

        return this;
    }

    /// <summary>
    ///     Adds a single predefined message to the builder's configuration.
    /// </summary>
    public MessageTemplateProviderBuilder AddPredefinedMessage(string messageId, string message)
    {
        var validationResult = ValidateInput(messageId, message, "Predefined message");
        if (!validationResult.IsSuccess)
        {
            _logger.ZLogError($"Failed to add template: {validationResult.ErrorMessage}");
            return this; // Return immediately on failure
        }

        // Since we know the input is valid at this point, proceed with adding.
        _predefinedMessages.Add(messageId, message);
        return this;
    }

    /// <summary>
    ///     Adds multiple predefined messages to the builder's configuration.
    /// </summary>
    public MessageTemplateProviderBuilder AddPredefinedMessages(Dictionary<string, string> messages)
    {
        foreach (var message in messages)
            try
            {
                AddPredefinedMessage(message.Key, message.Value);
            }
            catch (InvalidOperationException ex)
            {
                // Handle or log the error if necessary
                // Optionally continue to the next message
                _logger.ZLogError(ex, $"Failed to add predefined message '{message.Key}'");
            }

        return this;
    }

    /// <summary>
    ///     Builds and returns a configured MessageTemplateProvider instance.
    /// </summary>
    public MessageTemplateProvider Build()
    {
        try
        {
            var provider = new MessageTemplateProvider();

            foreach (var template in _templates)
                provider.RegisterTemplate(template.Key, template.Value);

            foreach (var message in _predefinedMessages)
                provider.RegisterPredefinedMessage(message.Key, message.Value);

            return provider;
        }
        catch (Exception ex)
        {
            _logger.ZLogError(ex, $"Failed to build MessageTemplateProvider instance.");
            throw;
        }
        finally
        {
            _predefinedMessages.Clear();
            _templates.Clear();
            _logger.ZLogDebug($"MessageTemplateProvider instance built successfully.");
        }
    }
}
