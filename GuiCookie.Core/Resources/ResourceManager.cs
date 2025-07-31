using GuiCookie.Core.Data;
using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Helpers;
using GuiCookie.Core.Rendering;
using LiruGameHelper.Parsers;
using System.Drawing;
using System.Reflection;

namespace GuiCookie.Core.Resources
{
    public abstract class ResourceManager
    {
        #region Constants
        public const string ColourAttributeName = "Colour";

        public const string ResourceSheetsNodeName = "ResourceSheets";
        public const string ResourceSheetNodeName = "ResourceSheet";

        private const string defaultSheetPath = "GuiCookie.Core.Resources.Defaults";

        internal static IDictionary<string, Func<SheetDataSource>> DefaultSheetDataLoadFunctions { get; }

        static ResourceManager()
        {
            DefaultSheetDataLoadFunctions = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(x => x.StartsWith(defaultSheetPath))
                .ToDictionary(x => x, x =>
                    new Func<SheetDataSource>(() =>
                    {
                        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(x)
                            ?? throw new InvalidDataException($"Missing default resource sheet \"{x}\"!");
                        return XmlSheetDataSource.Load(stream, x);
                    }));
        }
        #endregion

        #region XML Constants
        protected const string rootFolderAttributeName = "Folder";

        protected const string uriAttributeName = "Source";

        protected const string coloursNodeName = "Colours";

        protected const string fontsNodeName = "Fonts";

        protected const string imagesNodeName = "Images";

        protected const string colourAttributeName = "Colour";

        protected const string boundsTagName = "Bounds";

        protected const string tilemapTagName = "Tilemap";

        protected const string tilePositionTagName = "Tile";
        #endregion

        #region Fields
        private readonly Dictionary<string, Color> coloursByName = [];

        private readonly Dictionary<string, Font> fontsByName = [];

        private readonly Dictionary<string, Image> imagesByName = [];

        private readonly Dictionary<string, List<Color>> coloursBySheetFilePath = [];

        private readonly Dictionary<string, List<Font>> fontsBySheetFilePath = [];

        private readonly Dictionary<string, List<Image>> imagesBySheetFilePath = [];
        #endregion

        #region Properties

        /// <summary> The readonly fonts keyed by name. </summary>
        public IReadOnlyDictionary<string, Font> FontsByName => fontsByName;

        /// <summary> The readonly images keyed by name. </summary>
        public IReadOnlyDictionary<string, Image> ImagesByName => imagesByName;

        /// <summary> The readonly colours keyed by name. </summary>
        public IReadOnlyDictionary<string, Color> ColoursByName => coloursByName;
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
        public Color? GetColourOrDefault(IReadOnlyAttributeCollection attributes, string attributeName, Color? defaultTo = null)
            => GetColourOrDefault(attributes.GetAttributeOrDefault(attributeName, (string?)null), defaultTo);

        /// <summary>
        /// Tries to parse the given <paramref name="colourString"/> as a <see cref="Color"/>, returning <paramref name="defaultTo"/> if it fails.
        /// This will use <see cref="ColoursByName"/> if the value starts with the '$' symbol.
        /// </summary>
        /// <param name="colourString"> The raw colour as a string. </param>
        /// <param name="defaultTo"> The default <see cref="Color"/> to use if the parsing fails. </param>
        /// <returns> The parsed <see cref="Color"/> if the <paramref name="colourString"/> was parsed successfully into a colour; otherwise <paramref name="defaultTo"/>. </returns>
        /// <seealso cref="Colour.TryParse(string, out Color)"/>
        public Color? GetColourOrDefault(string? colourString, Color? defaultTo = null)
        {
            // If the given string is empty, return the default colour.
            if (string.IsNullOrWhiteSpace(colourString))
                return defaultTo;

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

        #region Add Functions
        /// <summary> Adds the given <paramref name="colour"/> to the <see cref="ColoursByName"/> keyed by the given <paramref name="name"/> </summary>
        /// <param name="name"> The key. </param>
        /// <param name="sheetFilePath"> The file path which owns this colour. </param>
        /// <param name="colour"> The value. </param>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> was empty or null. </exception>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> has already been used as a key. </exception>
        public void AddColour(string name, string sheetFilePath, Color colour)
        {
            if (!TryAddColour(name, sheetFilePath, colour))
                throw new ArgumentException($"Colour with name \"{name}\" has already been defined by sheet \"{sheetFilePath}\"");
        }

        public bool TryAddColour(string name, string sheetFilePath, Color colour)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(sheetFilePath);

            if (!coloursByName.TryAdd(name, colour)) 
                return false;
            if (!coloursBySheetFilePath.TryGetValue(sheetFilePath, out List<Color>? fileColours))
            {
                fileColours = [];
                coloursBySheetFilePath.Add(sheetFilePath, fileColours);
            }
            fileColours.Add(colour);
            return true;
        }

        /// <summary> Adds the given <paramref name="font"/> to the <see cref="FontsByName"/> keyed by the given <paramref name="name"/> </summary>
        /// <param name="name"> The key. </param>
        /// <param name="sheetFilePath"> The file path which owns this font. </param>
        /// <param name="font"> The value. </param>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> was empty or null. </exception>
        /// <exception cref="ArgumentException"> The given <paramref name="name"/> has already been used as a key. </exception>
        public void AddFont(string name, string sheetFilePath, Font font)
        {
            if (!TryAddFont(name, sheetFilePath, font))
                throw new ArgumentException($"Font with name \"{name}\" has already been defined by sheet \"{sheetFilePath}\"");
        }

        public bool TryAddFont(string name, string sheetFilePath, Font font)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(sheetFilePath);

            if (!fontsByName.TryAdd(name, font)) 
                return false;
            if (!fontsBySheetFilePath.TryGetValue(sheetFilePath, out List<Font>? fileFonts))
            {
                fileFonts = [];
                fontsBySheetFilePath.Add(sheetFilePath, fileFonts);
            }
            fileFonts.Add(font);
            return true;
        }

        /// <summary> Adds the given <paramref name="image"/> to the <see cref="ImagesByName"/> keyed by <see cref="Image.Name"/>. </summary>
        /// <param name="sheetFilePath"> The file path which owns this image. </param>
        /// <param name="image"> The value. </param>
        /// <exception cref="ArgumentException"> The given <see cref="Image.Name"/> was empty or null. </exception>
        /// <exception cref="ArgumentException"> The given <see cref="Image.Name"/> has already been used as a key. </exception>
        public void AddImage(string sheetFilePath, Image image)
        {
            if (!TryAddImage(sheetFilePath, image)) 
                throw new ArgumentException($"Image with name \"{image.Name}\" has already been defined by sheet \"{sheetFilePath}\"");
        }

        public bool TryAddImage(string sheetFilePath, Image image)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sheetFilePath);

            if (!imagesByName.TryAdd(image.Name, image))
                return false;
            if (!imagesBySheetFilePath.TryGetValue(sheetFilePath, out List<Image>? fileImages))
            {
                fileImages = [];
                imagesBySheetFilePath.Add(sheetFilePath, fileImages);
            }
            fileImages.Add(image);
            return true;
        }
        #endregion

        #region Load Functions
        public virtual void LoadFromSheets(IEnumerable<IReadOnlySheetDataSource> resourceSheets)
        {
            foreach (IReadOnlySheetDataSource resourceSheet in resourceSheets)
                LoadFromSheet(resourceSheet);
        }

        public virtual void LoadFromSheet(IReadOnlySheetDataSource resourceSheet)
        {
            // Get the root path for the resources.
            string? rootFolder = resourceSheet.RootNode.Attributes.GetAttributeOrDefault(rootFolderAttributeName, (string?)null);

            string sheetFilePath = PathHelpers.NormaliseFilePath(resourceSheet.FilePath ?? throw new ArgumentException("The given resource sheet has no file path!"));
            loadColours(resourceSheet, sheetFilePath);
            loadFonts(resourceSheet, sheetFilePath, rootFolder);
            loadImages(resourceSheet, sheetFilePath, rootFolder);
        }

        private void loadColours(IReadOnlySheetDataSource resourceSheet, string sheetFilePath)
        {
            IReadOnlySheetDataNode? coloursNode = resourceSheet.GetChildWithName(coloursNodeName);
            if (coloursNode == null)
                return;
            foreach (IReadOnlySheetDataNode colourNode in coloursNode.ChildNodes)
                loadColourFromNode(colourNode, sheetFilePath);
        }

        protected virtual void loadColourFromNode(IReadOnlySheetDataNode colourNode, string sheetFilePath)
        {
            if (!colourNode.Attributes.TryGetAttribute(colourAttributeName, out Color colour, Colour.TryParse))
                throw new ArgumentException($"Colour node \"{colourNode.Name}\" has an invalid colour value!", nameof(colourNode));

            AddColour(colourNode.Name, sheetFilePath, colour);
        }

        private void loadFonts(IReadOnlySheetDataSource resourceSheet, string sheetFilePath, string? rootFolder)
        {
            // Do nothing if the given node does not exist.
            IReadOnlySheetDataNode? fontsNode = resourceSheet.GetChildWithName(fontsNodeName);
            if (fontsNode == null)
                return;

            // Load the fonts.
            foreach (IReadOnlySheetDataNode fontNode in fontsNode.ChildNodes)
                loadFontFromNode(fontNode, sheetFilePath, rootFolder);
        }

        protected abstract void loadFontFromNode(IReadOnlySheetDataNode fontNode, string sheetFilePath, string? rootFolder);

        private void loadImages(IReadOnlySheetDataSource resourceSheet, string sheetFilePath, string? rootFolder)
        {
            // Do nothing if the given node does not exist.
            IReadOnlySheetDataNode? imagesNode = resourceSheet.GetChildWithName(imagesNodeName);
            if (imagesNode == null)
                return;

            // Load the images.
            foreach (IReadOnlySheetDataNode imageNode in imagesNode.ChildNodes)
                loadImageFromNode(imageNode, sheetFilePath, rootFolder);
        }

        protected abstract void loadImageFromNode(IReadOnlySheetDataNode imageNode, string sheetFilePath, string? rootFolder);
        #endregion
    }
}