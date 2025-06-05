using GuiCookie.Core.Data;
using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Styles.Attributes;
using LiruGameHelper.Reflection;
using System.Buffers;
using System.Reflection;

namespace GuiCookie.Core.Styles
{
    public class StyleManager(ResourceManager resourceManager)
    {
        #region Constants
        public const string ResourcesNodeName = "Resources";

        public const string DefaultStyleAttributeName = "DefaultStyle";
        
        private const string stylesNodeName = "Styles";

        private const string defaultStyleSheetPath = "GuiCookie.Core.Styles.Styles.xml";
        #endregion

        #region Backing Fields
        private readonly Dictionary<string, Style> stylesByName = [];
        #endregion

        #region Properties
        public IReadOnlyDictionary<string, Style> StylesByName => stylesByName;

        public Style? DefaultStyle { get; set; }

        public ResourceManager ResourceManager => resourceManager;

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
        public static SheetDataSource LoadDefaultSheetData()
        {
            // Load the contents of the file.
            using Stream stream = CreateDefaultTemplatesStream();
            return XmlSheetDataSource.Load(stream, defaultStyleSheetPath);
        }

        public static Stream CreateDefaultTemplatesStream() =>
            // Load the embedded xml file into a stream and make sure it exists.
            Assembly.GetExecutingAssembly().GetManifestResourceStream(defaultStyleSheetPath)
                ?? throw new InvalidDataException("Missing default template sheet!");

        public void LoadFromSheet(IReadOnlySheetDataSource styleSheet, bool includeResources = false)
        {
            if (includeResources)
            {
                IReadOnlySheetDataNode resourceNode = styleSheet.GetChildWithName(ResourcesNodeName) ?? throw new ArgumentException("Given style sheet is missing resource node!");
                ResourceManager.LoadFromNode(resourceNode);
            }

            IReadOnlySheetDataNode mainStyleNode = styleSheet.GetChildWithName(stylesNodeName) ?? throw new ArgumentException("Given style sheet is missing style node!");

            // Go over each style in the styles node and add it.
            Style[] addedStyles = ArrayPool<Style>.Shared.Rent(mainStyleNode.ChildNodes.Count);
            int addedStyleIndex = 0;
            foreach (IReadOnlySheetDataNode styleNode in mainStyleNode.ChildNodes)
            {
                Style style = Style.Load(ResourceManager, AttributeConstructorCache, styleNode);
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
