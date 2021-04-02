using LiruGameHelper.Reflection;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GuiCookie.Styles
{
    /// <summary> Loads, stores, and handles styles. </summary>
    public class StyleManager
    {
        #region Constants
        private const string resourcesNodeName = "Resources";

        private const string stylesNodeName = "Styles";

        private const string defaultStyleAttributeName = "DefaultStyle";
        #endregion

        #region Fields
        private readonly Dictionary<Type, ITextureCreator> textureCreatorsByStyleAttribute = new Dictionary<Type, ITextureCreator>();

        private readonly Dictionary<string, Style> stylesByName = new Dictionary<string, Style>();
        #endregion

        #region Properties
        public Style DefaultStyle { get; private set; }

        /// <summary> The object that holds all loaded resources for this style manager. </summary>
        public ResourceManager ResourceManager { get; }

        public ConstructorCache<IStyleAttribute> AttributeConstructorCache { get; } = new ConstructorCache<IStyleAttribute>();
        #endregion

        #region Constructors
        public StyleManager(ResourceManager resourceManager, GraphicsDevice graphicsDevice)
        {
            // Set dependencies.
            ResourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            // Initialise the texture creator.
            RegisterTextureCreator(new NineSliceDrawer(graphicsDevice));

            // Register the default attributes.
            AttributeConstructorCache.RegisterType(typeof(Font));
            AttributeConstructorCache.RegisterType(typeof(SliceFrame));
            AttributeConstructorCache.RegisterType(typeof(Content));
        }
        #endregion

        #region Load Functions
        public void LoadFromSheet(string sheetPath)
        {
            // If the given sheet path has no extension, add one.
            if (!Path.HasExtension(sheetPath)) sheetPath += ".xml";

            // If the file does not exist, throw an exception.
            if (!File.Exists(sheetPath)) throw new FileNotFoundException("The given style sheet file path does not exist.");

            // Load the xml file.
            XmlDocument styleSheet = new XmlDocument();
            styleSheet.Load(sheetPath);

            // Load the resources first.
            ResourceManager.Load(styleSheet.LastChild.SelectSingleNode(resourcesNodeName));

            // Load the styles.
            loadStyles(styleSheet.LastChild.SelectSingleNode(stylesNodeName));
        }

        public void LoadDefaultStyle(XmlNode mainNode)
        {
            // Try to get the default style attribute from the main node, if none exists, do nothing.
            XmlAttribute defaultStyleAttribute = mainNode.Attributes[defaultStyleAttributeName];
            if (defaultStyleAttribute is null) return;

            // Try to set the default style to the style with the name set in the attribute.
            DefaultStyle = GetStyleFromName(defaultStyleAttribute.InnerText);
        }

        private void loadStyles(XmlNode stylesNode)
        {
            // If the node is null, throw an exception.
            if (stylesNode == null) throw new Exception($"The main node was missing a {stylesNodeName} node.");

            // Go over each style in the styles node and add it.
            foreach (XmlNode styleNode in stylesNode.ChildNodes) 
                if (styleNode.NodeType != XmlNodeType.Comment) add(new Style(ResourceManager, AttributeConstructorCache, styleNode));

            // Go over the styles and combine any that require bases.
            foreach (Style style in stylesByName.Values) combineStyleWithBase(style);
        }
        #endregion

        #region Add Functions
        /// <summary> Adds the given <paramref name="style"/> to the manager, allowing it to be used by <see cref="Elements.Element"/>s. </summary>
        /// <param name="style"> The style to add. </param>
        public void Add(Style style)
        {
            // Add the style to the dictionary.
            add(style);

            // Combine the style with its base.
            combineStyleWithBase(style);
        }

        private void add(Style style) => stylesByName.Add(style.Name, style);

        private void combineStyleWithBase(Style style)
        {
            // If this style has no base style, do nothing.
            if (string.IsNullOrWhiteSpace(style.BaseStyleName)) return;

            // Get the base style.
            if (!stylesByName.TryGetValue(style.BaseStyleName, out Style baseStyle)) throw new Exception($"Style with name {style.Name} has base style of {style.BaseStyleName} which does not exist.");

            // If the base style also has a base style, throw an exception.
            if (!string.IsNullOrWhiteSpace(baseStyle.BaseStyleName)) throw new Exception("Base style cannot have another base style, ensure that no style has a base style that also has a base style.");

            // Combine the styles.
            style.CombineWithBase(baseStyle);
        }
        #endregion

        #region Get Functions
        public Style GetStyleFromName(string styleName) { if (stylesByName.TryGetValue(styleName, out Style style)) return style.CreateCopy(); else throw new Exception($"Style with name {styleName} does not exist."); }
        #endregion

        #region Texture Functions
        public void RegisterTextureCreator<T>(T textureCreator) where T : class, ITextureCreator
        {
            // Ensure the given creator exists.
            if (textureCreator == null) throw new ArgumentNullException(nameof(textureCreator));

            // Try to add the creator to the dictionary by its type.
            if (textureCreatorsByStyleAttribute.ContainsKey(typeof(T))) throw new Exception($"Texture creator with type {typeof(T)} has already been registered.");
            else textureCreatorsByStyleAttribute.Add(typeof(T), textureCreator);
        }

        public T GetTextureCreator<T>() where T : class, ITextureCreator => textureCreatorsByStyleAttribute.TryGetValue(typeof(T), out ITextureCreator textureCreator) ? (T)textureCreator : null;
        #endregion
    }
}
