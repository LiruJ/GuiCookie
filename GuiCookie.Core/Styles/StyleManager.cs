using GuiCookie.Core.Styles.Attributes;
using LiruGameHelper.Reflection;
using System.Xml;

namespace GuiCookie.Core.Styles
{
    public class StyleManager(ResourceManager resourceManager)
    {
        #region Constants
        private const string resourcesNodeName = "Resources";

        private const string stylesNodeName = "Styles";

        private const string defaultStyleAttributeName = "DefaultStyle";
        #endregion

        #region Backing Fields
        private readonly Dictionary<string, Style> stylesByName = [];
        #endregion

        #region Properties
        public IReadOnlyDictionary<string, Style> StylesByName => stylesByName;

        public Style DefaultStyle { get; set; }

        public ResourceManager ResourceManager => resourceManager;

        public ConstructorCache<IStyleAttribute> AttributeConstructorCache { get; } = new ConstructorCache<IStyleAttribute>();
        #endregion

        #region Constructors

        #endregion

        #region Load Functions
        public void LoadFromSheet(string sheetPath)
        {
            if (string.IsNullOrEmpty(sheetPath))
                throw new ArgumentNullException(nameof(sheetPath), "Missing stylesheet path!");

            if (!File.Exists(sheetPath))
                throw new FileNotFoundException($"No stylesheet exists at the given path! {sheetPath}");

            using FileStream stream = File.OpenRead(sheetPath);
            LoadFromSheet(stream);
        }

        public void LoadFromSheet(Stream stream)
        {
            XmlDocument styleSheet = new();
            styleSheet.Load(stream);
            LoadFromSheet(styleSheet);
        }

        public void LoadFromSheet(XmlDocument styleSheet)
        {
            // Get the main node.
            XmlNode? mainNode = styleSheet.SelectSingleNode($"/Main/{stylesNodeName}") ?? throw new ArgumentException($"The main node was missing a {stylesNodeName} node.");
            if (mainNode.NodeType != XmlNodeType.Element)
                throw new ArgumentException("Styles node was not an element!");

            loadStyles(mainNode);
        }

        private void loadStyles(XmlNode stylesNode)
        {
            // Go over each style in the styles node and add it.
            List<Style> addedStyles = new(stylesNode.ChildNodes.Count);
            foreach (XmlNode styleNode in stylesNode.ChildNodes)
            {
                if (styleNode.NodeType != XmlNodeType.Element)
                    continue;

                Style style = new(ResourceManager, AttributeConstructorCache, styleNode);
                Add(style);
                addedStyles.Add(style);
            }

            // Combine the added styles with their base styles.
            foreach (Style style in addedStyles)
                combineStyleWithBase(style);
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
            => stylesByName.TryGetValue(styleName, out Style? style) ? style : throw new ArgumentException("Style with name \"{styleName}\" does not exist!");
        #endregion
    }
}
