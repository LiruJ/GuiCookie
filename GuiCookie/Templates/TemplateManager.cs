using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GuiCookie.Templates
{
    /// <summary> Manages templates from a template sheet. </summary>
    public class TemplateManager : IReadOnlyTemplateManager
    {
        #region Constants
        private const string defaultTemplateSheetPath = "GuiCookie.Templates.Templates.xml";
        #endregion

        #region Fields
        private readonly Dictionary<string, Template> templatesByName;
        #endregion

        #region Constructors
        internal TemplateManager()
        {
            // Initialise the templates collection.
            templatesByName = new Dictionary<string, Template>();
        }
        #endregion

        #region Load Functions
        /// <summary> Loads the default template definitions from an embedded xml file. </summary>
        public void LoadDefault()
        {
            // Load the embedded xml file for the pre-defined templates, then load their contents.
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(defaultTemplateSheetPath))
            {
                // Load the file from the stream.
                XmlDocument templateSheet = new XmlDocument();
                templateSheet.Load(stream);

                // Load the contents of the file.
                loadFromSheet(templateSheet);
            }
        }

        public void LoadFromSheet(string sheetPath)
        {
            // If the given sheet path has no extension, add one.
            if (!Path.HasExtension(sheetPath)) sheetPath += ".xml";

            // If the file does not exist, throw an exception.
            if (!File.Exists(sheetPath)) throw new FileNotFoundException("The given template sheet file path does not exist.");

            // Load the xml file.
            XmlDocument templateSheet = new XmlDocument();
            templateSheet.Load(sheetPath);

            // Load the contents of the sheet.
            loadFromSheet(templateSheet);
        }

        private void loadFromSheet(XmlDocument templateSheet)
        {
            // Get the main node from the sheet.
            XmlNode mainNode = templateSheet.LastChild;

            // Go over each template within the main node.
            foreach (XmlNode templateNode in mainNode)
            {
                // If the node is a comment, skip it.
                if (templateNode.NodeType == XmlNodeType.Comment) continue;

                // Get the template from the node, this automatically adds it.
                getRootTemplate(mainNode, templateNode.Name);
            }
        }

        internal Template getRootTemplate(XmlNode mainNode, string name)
        {
            // If the root template is already loaded, return it.
            if (templatesByName.TryGetValue(name, out Template template)) return template;

            // Otherwise; find it within the main node.
            XmlNode templateNode = mainNode.SelectSingleNode(name) ?? throw new Exception($"Could not find template node with name: {name}");

            // Load the template.
            template = Template.LoadFromXML(this, mainNode, templateNode);

            // Add the template to the dictionary keyed by its name.
            templatesByName.Add(template.Name, template);

            // Return the created template.
            return template;
        }
        #endregion

        #region Get Functions
        public Template GetTemplateFromName(string templateName)
            => string.IsNullOrWhiteSpace(templateName) ? throw new ArgumentException("Template name cannot be null") : (templatesByName.TryGetValue(templateName, out Template template)
            ? template
            : throw new Exception($"Template with name {templateName} was not defined or included."));
        #endregion
    }
}
