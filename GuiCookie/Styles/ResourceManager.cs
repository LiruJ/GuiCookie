using GuiCookie.Attributes;
using LiruGameHelper.XML;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GuiCookie.Styles
{
    /// <summary> Handles loading and holding style resources. </summary>
    public class ResourceManager
    {
        #region Constants
        public const string ColourAttributeName = "Colour";
        #endregion

        #region XML Constants
        private const string rootFolderAttributeName = "Folder";

        private const string uriAttributeName = "URI";

        private const string coloursNodeName = "Colours";

        private const string fontsNodeName = "Fonts";

        private const string imagesNodeName = "Images";

        private const string colourAttributeName = "Colour";

        private const string boundsTagName = "Bounds";

        private const string tilemapTagName = "Tilemap";

        private const string tilePositionTagName = "Tile";
        #endregion

        #region Fields
        /// <summary> The font resources keyed by name. </summary>
        private readonly Dictionary<string, SpriteFont> fontsByName = new Dictionary<string, SpriteFont>();

        /// <summary> The image resources keyed by name. </summary>
        private readonly Dictionary<string, Image> imagesByName = new Dictionary<string, Image>();

        /// <summary> The colour resources keyed by name. </summary>
        private readonly Dictionary<string, Color> coloursByName = new Dictionary<string, Color>();
        #endregion

        #region Properties
        /// <summary> The <see cref="Microsoft.Xna.Framework.Content.ContentManager"/> used by this resource manager to load the resources. </summary>
        public ContentManager ContentManager { get; }

        /// <summary> The readonly fonts keyed by name. </summary>
        public IReadOnlyDictionary<string, SpriteFont> FontsByName => fontsByName;

        /// <summary> The readonly images keyed by name. </summary>
        public IReadOnlyDictionary<string, Image> ImagesByName => imagesByName;

        /// <summary> The readonly colours keyed by name. </summary>
        public IReadOnlyDictionary<string, Color> ColoursByName => coloursByName;
        #endregion

        #region Constructors
        /// <summary> Creates a new resource manager using the given <paramref name="contentManager"/> to load content. </summary>
        /// <param name="contentManager"> The content manager with which content is loaded. </param>
        public ResourceManager(ContentManager contentManager)
        {
            ContentManager = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        }
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
        public void AddFont(string name, SpriteFont font)
        {
            // Ensure validity.
            if (font == null) throw new ArgumentNullException("Given font was null.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Given font name was empty or null.");
            if (fontsByName.ContainsKey(name)) throw new ArgumentException($"Font with name {name} has already been defined.");

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
        public void Load(XmlNode resourceNode)
        {
            // Ensure the node exists.
            if (resourceNode == null) throw new ArgumentNullException(nameof(resourceNode));

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
            {
                // Get the URI of the font from the node.
                if (!fontNode.GetAttributeValue(uriAttributeName, out string fontURI)) throw new Exception($"{fontsNodeName} node was missing {uriAttributeName} attribute.");

                // Load the font.
                SpriteFont font = ContentManager.Load<SpriteFont>(Path.Combine(rootFolder, fontURI));

                // Add the font to the dictionary.
                AddFont(fontNode.Name, font);
            }
        }

        private void loadImages(XmlNode imagesNode, string rootFolder)
        {
            // Do nothing if the given node does not exist.
            if (imagesNode == null) return;

            // Load the images.
            foreach (XmlNode imageNode in imagesNode)
            {
                // Get the URI of the image from the node.
                if (!imageNode.GetAttributeValue(uriAttributeName, out string imageURI)) throw new Exception($"{imagesNodeName} node was missing {uriAttributeName} attribute.");

                // Load the texture.
                Texture2D texture = ContentManager.Load<Texture2D>(Path.Combine(rootFolder, imageURI));

                // Add the root image to the dictionary.
                AddImage(new Image(imageNode.Name, texture, texture.Bounds));

                // If a tilemap tag was given and it parses into a point, use the image as a tilemap.
                bool usingGrid = imageNode.TryParseAttributeValue(tilemapTagName, ToPoint.TryParse, out Point tilemapSize);
                Point currentGridCell = Point.Zero;
                Point tileSize = (usingGrid) ? (texture.Bounds.Size.ToVector2() / tilemapSize.ToVector2()).ToPoint() : Point.Zero;

                // If there are child nodes, add them as images.
                foreach (XmlNode sourceNode in imageNode)
                {
                    // Create a new source rectangle to be used.
                    Rectangle sourceRectangle = Rectangle.Empty;

                    // If a bounds tag was given, try to use that.
                    if (sourceNode.GetAttributeValue(boundsTagName, out string boundsString))
                        { if (!ToRectangle.TryParse(boundsString, out sourceRectangle)) throw new Exception($"Image source with name {sourceNode.Name} has an invalid {boundsTagName} attribute."); }
                    // Otherwise; if no bounds tag was given but a grid is being used, use that.
                    else if (usingGrid)
                    {
                        // If a specific cell position was given, use that.
                        if (sourceNode.GetAttributeValue(tilePositionTagName, out string tilePositionString))
                        {
                            // Ensure the tile position is valid.
                            if (!ToPoint.TryParse(tilePositionString, out currentGridCell))
                                throw new Exception($"Image source with name {sourceNode.Name} has an invalid {tilePositionTagName} attribute.");

                            // Get the source rectangle from this grid position.
                            sourceRectangle = new Rectangle(tileSize * currentGridCell, tileSize);
                        }
                        // Otherwise; increment the tile position.
                        else
                        {
                            // Get the source rectangle from the current position.
                            sourceRectangle = new Rectangle(tileSize * currentGridCell, tileSize);

                            // Increment the tile position.
                            if (currentGridCell.X + 1 >= tilemapSize.X) { currentGridCell.X = 0; currentGridCell.Y++; }
                            else currentGridCell.X++;
                        }

                    }
                    // Finally, if nothing at all was given and a grid isn't being used, throw an exception.
                    else throw new Exception($"Image source with name {sourceNode.Name} does not have enough information to be created. Must have either a {tilePositionTagName} or {boundsTagName} attribute.");

                    // Create a new image with the root texture and user-defined bounds.
                    Image image = new Image(sourceNode.Name, texture, sourceRectangle);

                    // Add the image to the dictionary.
                    AddImage(image);
                }
            }
        }
        #endregion

        #region Get Functions
        /// <summary> Gets a <see cref="Color"/> from the given <paramref name="attributes"/> with the given <paramref name="attributeName"/>. If the colour cannot be parsed or does not exist, returns <paramref name="defaultTo"/>. This will use <see cref="ColoursByName"/> if the value starts with the '$' symbol. </summary>
        /// <param name="attributes"> The attributes to check for the value with the given <paramref name="attributeName"/> key. </param>
        /// <param name="attributeName"> The name of the attribute to try to parse. </param>
        /// <param name="defaultTo"> The default <see cref="Color"/> to use if the parsing fails. </param>
        /// <returns> The parsed <see cref="Color"/> if the attribute exists and was parsed successfully into a colour; otherwise <paramref name="defaultTo"/>. </returns>
        public Color? GetColourOrDefault(IReadOnlyAttributes attributes, string attributeName, Color? defaultTo = null) => GetColourOrDefault(attributes.GetAttributeOrDefault(attributeName, string.Empty), defaultTo);

        /// <summary> Tries to parse the given <paramref name="colourString"/> as a <see cref="Color"/>, returning <paramref name="defaultTo"/> if it fails. This will use <see cref="ColoursByName"/> if the value starts with the '$' symbol. </summary>
        /// <param name="colourString"> The raw colour as a string. </param>
        /// <param name="defaultTo"> The default <see cref="Color"/> to use if the parsing fails. </param>
        /// <returns> The parsed <see cref="Color"/> if the <paramref name="colourString"/> was parsed successfully into a colour; otherwise <paramref name="defaultTo"/>. </returns>
        /// <seealso cref="Colour.TryParse(string, out Color)"/>
        public Color? GetColourOrDefault(string colourString, Color? defaultTo = null)
        {
            // If the given string is empty, return the default colour.
            if (string.IsNullOrWhiteSpace(colourString)) return defaultTo;

            // If the string begins with a '$', get the colour from the dictionary.
            if (colourString.StartsWith("$"))
            {
                // If the colour string is literally just '$', return the default colour.
                if (colourString.Length == 1) return defaultTo;
                // Otherwise; try to get the colour from the dictionary.
                else if (coloursByName.TryGetValue(colourString[1..], out Color resourceColour)) return resourceColour;
                // In this case, it's clear that the user wanted a colour from the dictionary. Instead of returning the default colour, throw an exception. This makes it a little less confusing.
                else throw new Exception($"Colour with name {colourString[1..]} was not defined as a resource.");
            }
            // Otherwise; parse and return the colour.
            else if (Colour.TryParse(colourString, out Color parsedColour)) return parsedColour;
            // Finally, if all else fails, return the default colour.
            else return defaultTo;
        }
        #endregion
    }
}
