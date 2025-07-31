using GuiCookie.Core.Data;
using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Resources;
using GuiCookie.Core.Styles.Attributes;
using LiruGameHelper.Reflection;
using System.Buffers;
using System.Reflection;

namespace GuiCookie.Core.Styles
{
    public class StyleManager(ResourceManager resourceManager)
    {
        #region Constants
        public const string StyleSheetsNodeName = "StyleSheets";
        public const string StyleSheetNodeName = "StyleSheet";

        private const string stylesNodeName = "Styles";

        private const string defaultStyleSheetPath = "GuiCookie.Core.Styles.Defaults";

        internal static IDictionary<string, Func<SheetDataSource>> DefaultSheetDataLoadFunctions { get; }

        static StyleManager()
        {
            DefaultSheetDataLoadFunctions = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(x => x.StartsWith(defaultStyleSheetPath))
                .ToDictionary(x => x, x =>
                    new Func<SheetDataSource>(() =>
                    {
                        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(x)
                            ?? throw new InvalidDataException($"Missing default style sheet \"{x}\"!");
                        return XmlSheetDataSource.Load(stream, x);
                    }));
        }
        #endregion

        #region Backing Fields
        private readonly Dictionary<string, Style> stylesByName = [];

        private readonly Dictionary<string, List<Style>> stylesPerFilePath = [];
        #endregion

        #region Properties
        public IReadOnlyDictionary<string, Style> StylesByName => stylesByName;

        public ConstructorCache<IStyleAttribute> AttributeConstructorCache { get; } = new ConstructorCache<IStyleAttribute>();
        #endregion

        #region Attribute Functions
        public void RegisterDefaultAttributes()
        {
            // Register the default attributes.
            AttributeConstructorCache.RegisterType(typeof(FontStyleAttribute));
            AttributeConstructorCache.RegisterType(typeof(SliceFrameStyleAttribute));
            AttributeConstructorCache.RegisterType(typeof(ContentStyleAttribute));
        }
        #endregion

        #region Load Functions
        public void LoadIncluded(IReadOnlySheetDataNode styleSheetsNode, SheetDataSourceLoader sourceLoader)
        {
            HashSet<string> loadedFilePaths = [.. stylesPerFilePath.Keys];
            List<SheetDataSource> includedSources = [];
            sourceLoader.ResolveAndLoadIncluded(styleSheetsNode, StyleSheetsNodeName, ref includedSources, loadedFilePaths);
            LoadFromSheets(includedSources);
        }

        public void LoadFromSheets(IEnumerable<IReadOnlySheetDataSource> styleSheets)
        {
            foreach (IReadOnlySheetDataSource styleSheet in styleSheets)
                LoadFromSheet(styleSheet);
        }

        public void LoadFromSheet(IReadOnlySheetDataSource styleSheet)
        {
            // TODO: Have this work like templates, so styles can reference other sheet styles.
            // Go over each style in the styles node and add it.
            Style[] addedStyles = ArrayPool<Style>.Shared.Rent(styleSheet.RootNode.ChildNodes.Count);
            int addedStyleIndex = 0;
            foreach (IReadOnlySheetDataNode styleNode in styleSheet.RootNode.GetChildWithName(stylesNodeName)?.ChildNodes
                ?? throw new InvalidDataException($"Style sheet \"{styleSheet.FilePath}\" is missing a \"{stylesNodeName}\" node!"))
            {
                Style style = Style.Load(resourceManager, AttributeConstructorCache, styleNode);
                Add(style);
                addedStyles[addedStyleIndex] = style;
            }

            // Combine the added styles with their base styles.
            for (int i = 0; i < addedStyleIndex; i++)
            {
                Style style = addedStyles[i];
                combineStyleWithBase(style);
            }

            ArrayPool<Style>.Shared.Return(addedStyles, true);
        }
        #endregion

        #region Add Functions
        /// <summary> Adds the given <paramref name="style"/> to the manager, allowing it to be used by elements. </summary>
        /// <param name="style"> The style to add. </param>
        public void Add(Style style) 
            => stylesByName.Add(style.Name, style);

        private void combineStyleWithBase(Style style)
        {
            // If this style has no base style, do nothing.
            if (string.IsNullOrWhiteSpace(style.BaseStyleName))
                return;

            // Get the base style.
            if (!stylesByName.TryGetValue(style.BaseStyleName, out Style? baseStyle))
                throw new Exception($"Style with name {style.Name} has base style of {style.BaseStyleName} which does not exist.");

            // If the base style also has a base style, throw an exception.
            if (!string.IsNullOrWhiteSpace(baseStyle.BaseStyleName)) 
                throw new Exception("Base style cannot have another base style, ensure that no style has a base style that also has a base style.");

            // Combine the styles.
            style.CombineWithBase(baseStyle);
        }
        #endregion

        #region Get Functions
        public Style GetStyleFromName(string styleName)
            => stylesByName.TryGetValue(styleName, out Style? style) ? style : throw new ArgumentException($"Style with name \"{styleName}\" does not exist!");
        #endregion
    }
}
