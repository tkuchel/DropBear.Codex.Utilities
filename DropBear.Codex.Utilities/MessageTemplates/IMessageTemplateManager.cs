namespace DropBear.Codex.Utilities.MessageTemplates;

public interface IMessageTemplateManager
{
    void RegisterTemplate(string templateId, string template);
    byte[] FormatUtf8(string templateId, params object[] args);
    string FormatString(string templateId, params object[] args);
    void RegisterTemplates(Dictionary<string, string> templatesToRegister);
}
