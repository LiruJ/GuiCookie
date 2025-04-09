using GuiCookie.Core.Data;
using GuiCookie.Core.Data.Xml;
using System.Reflection;

namespace GuiCookie.Core.Templates
{
    /// <summary> Manages templates from a template sheet. </summary>
    public class TemplateManager : IReadOnlyTemplateManager
    {
        #region Constants
        private const string defaultTemplateSheetPath = "GuiCookie.Core.Templates.Templates.xml";
        #endregion

        #region Fields
        private readonly Dictionary<string, Template> templatesByName = [];
        #endregion

        #region Get Functions
        /// <summary> Gets the <see cref="Template"/> which has the given <paramref name="templateName"/>. </summary>
        /// <param name="templateName"> The name of the template to get. </param>
        /// <returns> The template with the given <paramref name="templateName"/>. </returns>
        public Template GetTemplateFromName(string templateName)
            => string.IsNullOrWhiteSpace(templateName) 
            ? throw new ArgumentException("Template name cannot be null") 
            : templatesByName.TryGetValue(templateName, out Template? template)
                ? template
                : throw new Exception($"Template with name {templateName} was not defined or included.");
        #endregion

        #region Load Functions
        /// <summary> Loads the default template definitions from an embedded xml file. </summary>
        public void LoadDefault()
        {
            // Load the embedded xml file for the pre-defined templates, then load their contents.
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(defaultTemplateSheetPath)
                ?? throw new InvalidDataException("Missing default template sheet!");

            // Load the contents of the file.
            XmlSheetDataSource dataSource = XmlSheetDataSource.Load(stream, defaultTemplateSheetPath);
            LoadFromSheet(dataSource);
        }

        public void LoadFromSheet(IReadOnlySheetDataSource templateSheet)
        {
            // Go over each template within the main node.
            foreach (IReadOnlySheetDataNode templateNode in templateSheet.RootNode.ChildNodes)
                getRootTemplate(templateSheet.RootNode, templateNode.Name);
        }

        internal Template getRootTemplate(IReadOnlySheetDataNode mainNode, string name)
        {
            // If the root template is already loaded, return it.
            if (templatesByName.TryGetValue(name, out Template? template))
                return template;

            // Otherwise; find it within the main node.
            IReadOnlySheetDataNode templateNode = mainNode.GetChildWithName(name) ?? throw new Exception($"Could not find template node with name: {name}");

            // Load the template.
            template = Template.Load(this, mainNode, templateNode);

            // Add the template to the dictionary keyed by its name.
            templatesByName.Add(template.Name, template);

            // Return the created template.
            return template;
        }
        #endregion
    }
}
