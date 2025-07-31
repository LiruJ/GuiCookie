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

        private const string templatesNodeName = "Templates";

        private const string defaultSheetPath = "GuiCookie.Core.Templates.Defaults";

        internal static IDictionary<string, Func<SheetDataSource>> DefaultSheetDataLoadFunctions { get; }

        static TemplateManager()
        {
            DefaultSheetDataLoadFunctions = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(x => x.StartsWith(defaultSheetPath))
                .ToDictionary(x => x, x =>
                    new Func<SheetDataSource>(() =>
                    {
                        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(x)
                            ?? throw new InvalidDataException($"Missing default template sheet \"{x}\"!");
                        return XmlSheetDataSource.Load(stream, x);
                    }));
        }
        #endregion

        #region Fields
        private readonly Dictionary<string, Template> templatesByName = [];

        private readonly Dictionary<string, List<Template>> templatesByFilePath = [];
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
        /// <summary>
        /// Loads all the template sheets referenced in the given <paramref name="templateSheetsNode"/> (usually from the layout sheet), recursively loading all references.
        /// </summary>
        /// <param name="templateSheetsNode"></param>
        /// <param name="sourceLoader"></param>
        public void LoadIncluded(IReadOnlySheetDataNode templateSheetsNode, SheetDataSourceLoader sourceLoader)
        {
            HashSet<string> loadedFilePaths = [.. templatesByFilePath.Keys];
            List<SheetDataSource> includedSources = [];
            sourceLoader.ResolveAndLoadIncluded(templateSheetsNode, TemplateSheetsNodeName, ref includedSources, loadedFilePaths);
            LoadFromSheets(includedSources);
        }

        public void LoadFromSheets(IEnumerable<IReadOnlySheetDataSource> templateSheets)
        {
            var templateNodes = templateSheets.SelectMany(x => x.GetChildWithName(templatesNodeName)?.ChildNodes 
                ?? throw new InvalidDataException($"Template sheet \"{x.FilePath}\" is missing a \"{templatesNodeName}\" node!"));
            List<SheetNodePathPair> templateNodeSourcePairs = SheetDataSourceLoader.RentNodesBySheetPath(templateSheets, templatesNodeName);

            // Stage 1: Load the templates themselves. This does not reference anything else, it purely sets up each template from a node.
            foreach (var templateNodeSourcePair in templateNodeSourcePairs)
            {
                Template template = Template.Load(templateNodeSourcePair.Node);
                addTemplate(template, templateNodeSourcePair.FilePath);
            }

            // Stage 2: Resolve the template bases and children. This gets the base and child templates from this template manager, but does not do any combining.
            foreach (var templateNodeSourcePair in templateNodeSourcePairs)
            {
                Template template = templatesByName[templateNodeSourcePair.Node.Name];
                template.ResolveBase(this);
            }

            // Stage 3: Combine templates with their bases.
            foreach (var templateNodeSourcePair in templateNodeSourcePairs)
            {
                Template template = templatesByName[templateNodeSourcePair.Node.Name];
                template.CombineOverBase();
            }

            foreach (var templateNodeSourcePair in templateNodeSourcePairs)
            {
                Template template = templatesByName[templateNodeSourcePair.Node.Name];
                template.ResolveChildren(this, templateNodeSourcePair.Node.ChildNodes);
            }

            SheetDataSourceLoader.ReturnNodesBySheetPath(templateNodeSourcePairs);
        }

        private void addTemplate(Template template, string filePath)
        {
            if (!templatesByName.TryAdd(template.Name, template))
                throw new InvalidDataException($"Template \"{template.Name}\" was defined more than once!");
            if (!templatesByFilePath.TryGetValue(filePath, out List<Template>? sheetTemplates))
            {
                sheetTemplates = [];
                templatesByFilePath.Add(filePath, sheetTemplates);
            }
            sheetTemplates.Add(template);
        }
        #endregion
    }
}
