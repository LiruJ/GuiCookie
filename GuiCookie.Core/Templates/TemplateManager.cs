using GuiCookie.Core.Data;
using GuiCookie.Core.Data.Xml;
using System.Reflection;

namespace GuiCookie.Core.Templates
{
    /// <summary> Manages templates from a template sheet. </summary>
    public class TemplateManager : IReadOnlyTemplateManager
    {
        #region Constants
        public const string TemplateSheetsNodeName = "TemplateSheets";
        public const string TemplateSheetNodeName = "TemplateSheet";

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
                : TryGetTemplateFromName(templateName, out Template? template) 
                    ? template!
                    : throw new Exception($"Template with name {templateName} was not defined or included.");

        public bool TryGetTemplateFromName(string templateName, out Template? template)
            => templatesByName.TryGetValue(templateName, out template);
        #endregion

        #region Load Functions
        public static SheetDataSource LoadDefaultSheetData()
        {
            // Load the contents of the file.
            using Stream stream = CreateDefaultTemplatesStream();
            return XmlSheetDataSource.Load(stream, defaultTemplateSheetPath);
        }

        public static Stream CreateDefaultTemplatesStream() =>
            // Load the embedded xml file into a stream and make sure it exists.
            Assembly.GetExecutingAssembly().GetManifestResourceStream(defaultTemplateSheetPath)
                ?? throw new InvalidDataException("Missing default template sheet!");

        public void LoadFromSheets(IEnumerable<IReadOnlySheetDataSource> templateSheets)
        {
            var templateNodes = templateSheets.SelectMany(x => x.RootNode.ChildNodes);

            // Stage 1: Load the templates themselves. This does not reference anything else, it purely sets up each template from a node.
            foreach (var templateNode in templateNodes)
            {
                Template template = Template.Load(templateNode);
                if (!templatesByName.TryAdd(template.Name, template))
                    throw new InvalidDataException($"Template \"{template.Name}\" was defined more than once!");
            }

            // Stage 2: Resolve the template bases and children. This gets the base and child templates from this template manager, but does not do any combining.
            foreach (var templateNode in templateNodes)
            {
                Template template = templatesByName[templateNode.Name];
                template.ResolveBase(this);
            }

            // Stage 3: Combine templates with their bases.
            foreach (var templateNode in templateNodes)
            {
                Template template = templatesByName[templateNode.Name];
                template.CombineOverBase();
            }

            foreach (var templateNode in templateNodes)
            {
                Template template = templatesByName[templateNode.Name];
                template.ResolveChildren(this, templateNode.ChildNodes);
            }
        }

        //public void LoadFromSheet(IReadOnlySheetDataSource templateSheet)
        //{
        //    // Go over each template within the main node.
        //    foreach (IReadOnlySheetDataNode templateNode in templateSheet.RootNode.ChildNodes)
        //        getRootTemplate(templateSheet.RootNode, templateNode.Name);
        //}

        //private void loadTemplate(IReadOnlySheetDataNode templateNode)
        //{
        //    Template.Load(this, )
        //}

        //internal Template getRootTemplate(IReadOnlySheetDataNode mainNode, string name)
        //{
        //    // TODO: Split loading into two steps, so that all templates can be cross-referenced.
        //    // If the root template is already loaded, return it.
        //    if (templatesByName.TryGetValue(name, out Template? template))
        //        return template;

        //    // Otherwise; find it within the main node.
        //    IReadOnlySheetDataNode templateNode = mainNode.GetChildWithName(name) ?? throw new Exception($"Could not find template node with name: {name}");

        //    // Load the template.
        //    template = Template.Load(this, mainNode, templateNode);

        //    // Add the template to the dictionary keyed by its name.
        //    templatesByName.Add(template.Name, template);

        //    // Return the created template.
        //    return template;
        //}
        #endregion
    }
}
