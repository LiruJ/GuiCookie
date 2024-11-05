using GuiCookie.Core.Attributes;
using GuiCookie.Core.Rendering;
using LiruGameHelper.Parsers;
using LiruGameHelper.XML;
using System.Drawing;
using System.Xml;

namespace GuiCookie.Core.Styles
{
    /// <summary> Handles loading and holding style resources. </summary>
    /// <remarks> Creates a new resource manager using the given <paramref name="contentManager"/> to load content. </remarks>
    /// <param name="contentManager"> The content manager with which content is loaded. </param>
    public abstract class ResourceManager
    {
        #region Constants
        public const string ColourAttributeName = "Colour";
        #endregion

        #region XML Constants
        protected const string rootFolderAttributeName = "Folder";

        protected const string uriAttributeName = "URI";

        protected const string coloursNodeName = "Colours";

        protected const string fontsNodeName = "Fonts";

        protected const string imagesNodeName = "Images";

        protected const string colourAttributeName = "Colour";

        protected const string boundsTagName = "Bounds";

        protected const string tilemapTagName = "Tilemap";

        protected const string tilePositionTagName = "Tile";
        #endregion

        #region Fields
        /// <summary> The font resources keyed by name. </summary>
        private readonly Dictionary<string, Font> fontsByName = [];

        /// <summary> The image resources keyed by name. </summary>
        private readonly Dictionary<string, Image> imagesByName = [];

        /// <summary> The colour resources keyed by name. </summary>
        private readonly Dictionary<string, Color> coloursByName = [];
        #endregion

        #region Properties

        /// <summary> The readonly fonts keyed by name. </summary>
        public IReadOnlyDictionary<string, Font> FontsByName => fontsByName;

        /// <summary> The readonly images keyed by name. </summary>
        public IReadOnlyDictionary<string, Image> ImagesByName => imagesByName;

        /// <summary> The readonly colours keyed by name. </summary>
        public IReadOnlyDictionary<string, Color> ColoursByName => coloursByName;
        #endregion

        #region Add Functions
        /// <summary> Adds the given <paramref name="colour"/> to the <see cref="ColoursByName"/> keyed by the given <paramref name="name"/> </summary>
        /// <param name="name"> The key. </param>
        /// <param name="colour"> The value. </param>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> was empty or null. </exception>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> has already been used as a key. </exception>
        public void AddColour(string name, Color colour)
        {
            // Ensure validity.
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Given colour name was empty or null.");
            if (coloursByName.ContainsKey(name)) throw new ArgumentException($"Colour with name {name} has already been defined.");

            // Add the colour.
            coloursByName.Add(name, colour);
        }

        /// <summary> Adds the given <paramref name="font"/> to the <see cref="FontsByName"/> keyed by the given <paramref name="name"/> </summary>
        /// <param name="name"> The key. </param>
        /// <param name="font"> The value. </param>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> was empty or null. </exception>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> has already been used as a key. </exception>
        public void AddFont(string name, Font font)
        {
            // Ensure validity.
            ArgumentNullException.ThrowIfNull(font);
            if (string.IsNullOrWhiteSpace(name)) 
                throw new ArgumentException("Given font name was empty or null.");
            if (fontsByName.ContainsKey(name))
                throw new ArgumentException($"Font with name {name} has already been defined.");

            // Add the font.
            fontsByName.Add(name, font);
        }

        /// <summary> Adds the given <paramref name="image"/> to the <see cref="ImagesByName"/> keyed by <see cref="Image.Name"/>. </summary>
        /// <param name="image"> The value. </param>
        /// <exception cref="ArgumentException"> The given <see cref="Image.Name"/> was empty or null. </exception>
        /// <exception cref="ArgumentException"> The given <see cref="Image.Name"/> has already been used as a key. </exception>
        public void AddImage(Image image)
        {
            // Ensure validity.
            if (image.IsEmpty) throw new ArgumentException("Given image was empty.");
            if (imagesByName.ContainsKey(image.Name)) throw new ArgumentException($"Image with name {image.Name} has already been defined.");

            // Add the image.
            imagesByName.Add(image.Name, image);
        }
        #endregion

        #region Load Functions
        /// <summary> Loads the given <paramref name="resourceNode"/> into this resource manager. </summary>
        /// <param name="resourceNode"> The XML node containing the resources. </param>
        public virtual void Load(XmlNode resourceNode)
        {
            // Ensure the node exists.
            ArgumentNullException.ThrowIfNull(resourceNode);

            // Get the root path for the resources.
            resourceNode.GetAttributeValue(rootFolderAttributeName, out string rootFolder);

            // Load the colours, fonts, and images.
            loadColours(resourceNode.SelectSingleNode(coloursNodeName));
            loadFonts(resourceNode.SelectSingleNode(fontsNodeName), rootFolder);
            loadImages(resourceNode.SelectSingleNode(imagesNodeName), rootFolder);
        }

        private void loadColours(XmlNode coloursNode)
        {
            // Do nothing if the given node does not exist.
            if (coloursNode == null) return;

            // Load the colours.
            foreach (XmlNode colourNode in coloursNode)
                // Add the parsed colour to the dictionary with the node's name.
                AddColour(colourNode.Name, colourNode.ParseAttributeValue(colourAttributeName, Colour.Parse));
        }

        private void loadFonts(XmlNode fontsNode, string rootFolder)
        {
            // Do nothing if the given node does not exist.
            if (fontsNode == null) return;

            // Load the fonts.
            foreach (XmlNode fontNode in fontsNode)
                loadFontFromNode(fontNode, rootFolder);
        }

        protected abstract void loadFontFromNode(XmlNode fontNode, string rootFolder);

        private void loadImages(XmlNode imagesNode, string rootFolder)
        {
            // Do nothing if the given node does not exist.
            if (imagesNode == null) return;

            // Load the images.
            foreach (XmlNode imageNode in imagesNode)
                loadImageFromNode(imageNode, rootFolder);
        }

        protected abstract void loadImageFromNode(XmlNode imageNode, string rootFolder);
        #endregion

        #region Get Functions
        /// <summary>
        /// Gets a <see cref="Color"/> from the given <paramref name="attributes"/> with the given <paramref name="attributeName"/>.
        /// If the colour cannot be parsed or does not exist, returns <paramref name="defaultTo"/>.
        /// This will use <see cref="ColoursByName"/> if the value starts with the '$' symbol.
        /// </summary>
        /// <param name="attributes"> The attributes to check for the value with the given <paramref name="attributeName"/> key. </param>
        /// <param name="attributeName"> The name of the attribute to try to parse. </param>
        /// <param name="defaultTo"> The default <see cref="Color"/> to use if the parsing fails. </param>
        /// <returns> The parsed <see cref="Color"/> if the attribute exists and was parsed successfully into a colour; otherwise <paramref name="defaultTo"/>. </returns>
        public Color? GetColourOrDefault(IReadOnlyAttributes attributes, string attributeName, Color? defaultTo = null) 
            => GetColourOrDefault(attributes.GetAttributeOrDefault(attributeName, string.Empty), defaultTo);

        /// <summary>
        /// Tries to parse the given <paramref name="colourString"/> as a <see cref="Color"/>, returning <paramref name="defaultTo"/> if it fails.
        /// This will use <see cref="ColoursByName"/> if the value starts with the '$' symbol.
        /// </summary>
        /// <param name="colourString"> The raw colour as a string. </param>
        /// <param name="defaultTo"> The default <see cref="Color"/> to use if the parsing fails. </param>
        /// <returns> The parsed <see cref="Color"/> if the <paramref name="colourString"/> was parsed successfully into a colour; otherwise <paramref name="defaultTo"/>. </returns>
        /// <seealso cref="Colour.TryParse(string, out Color)"/>
        public Color? GetColourOrDefault(string colourString, Color? defaultTo = null)
        {
            // If the given string is empty, return the default colour.
            if (string.IsNullOrWhiteSpace(colourString)) return defaultTo;

            // If the string begins with a '$', get the colour from the dictionary.
            if (colourString.StartsWith('$'))
            {
                // If the colour string is literally just '$', return the default colour.
                if (colourString.Length == 1) 
                    return defaultTo;
                // Otherwise; try to get the colour from the dictionary.
                else if (coloursByName.TryGetValue(colourString[1..], out Color resourceColour))
                    return resourceColour;
                // In this case, it's clear that the user wanted a colour from the dictionary. Instead of returning the default colour, throw an exception. This makes it a little less confusing.
                else 
                    throw new Exception($"Colour with name {colourString[1..]} was not defined as a resource.");
            }
            // Otherwise; parse and return the colour.
            else if (Colour.TryParse(colourString, out Color parsedColour))
                return parsedColour;
            // Finally, if all else fails, return the default colour.
            else 
                return defaultTo;
        }
        #endregion
    }
}