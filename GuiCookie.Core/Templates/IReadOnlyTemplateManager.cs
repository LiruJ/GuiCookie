namespace GuiCookie.Core.Templates
{
    public interface IReadOnlyTemplateManager
    {
        Template GetTemplateFromName(string templateName);
    }
}