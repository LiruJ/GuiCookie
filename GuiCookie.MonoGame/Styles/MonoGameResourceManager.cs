using GuiCookie.Core.Data;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.MonoGame.Extensions;
using GuiCookie.MonoGame.Rendering;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace GuiCookie.MonoGame.Styles
{
    public class MonoGameResourceManager(ContentManager contentManager) : ResourceManager
    {
        #region Properties
        /// <summary> The <see cref="Microsoft.Xna.Framework.Content.ContentManager"/> used by this resource manager to load the resources. </summary>
        public ContentManager ContentManager { get; } = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        #endregion

        #region Load Functions
        protected override void loadFontFromNode(IReadOnlySheetDataNode? fontNode, string? rootFolder)
        {
            if (fontNode == null)
                return;

            // Get the URI of the font from the node.
            string? fontURI = fontNode.Attributes.GetAttributeOrDefault(uriAttributeName, (string?)null)
                ?? throw new Exception($"{fontsNodeName} node was missing {uriAttributeName} attribute.");

            // Load the font.
            SpriteFont spriteFont = ContentManager.Load<SpriteFont>(string.IsNullOrWhiteSpace(rootFolder) ? fontURI : Path.Combine(rootFolder, fontURI));
            Font font = new MonoGameFont(spriteFont);

            // Add the font to the dictionary.
            AddFont(fontNode.Name, font);
        }

        protected override void loadImageFromNode(IReadOnlySheetDataNode? imageNode, string? rootFolder)
        {
            if (imageNode == null)
                return;

            // Get the URI of the image from the node.
            string? imageURI = imageNode.Attributes.GetAttributeOrDefault(uriAttributeName, (string?)null)
                ?? throw new Exception($"{imagesNodeName} node was missing {uriAttributeName} attribute.");

            // Load the texture.
            Texture2D texture = ContentManager.Load<Texture2D>(string.IsNullOrWhiteSpace(rootFolder) ? imageURI : Path.Combine(rootFolder, imageURI));

            // Add the root image to the dictionary.
            AddImage(new MonoGameImage(imageNode.Name, texture, texture.Bounds.ToDrawingRectangle()));

            // If a tilemap tag was given and it parses into a point, use the image as a tilemap.
            Point? tilemapSize = imageNode.Attributes.GetAttributeOrDefault(tilemapTagName, (Point?)null, ToPoint.TryParse);
            bool usingGrid = tilemapSize != null;
            Point currentGridCell = Point.Zero;
            Point tileSize = usingGrid ? (texture.Bounds.Size.ToVector2() / tilemapSize!.Value.ToVector2()).ToPoint() : Point.Zero;

            // If there are child nodes, add them as images.
            foreach (IReadOnlySheetDataNode sourceNode in imageNode.ChildNodes)
            {
                // Create a new source rectangle to be used.
                Rectangle sourceRectangle = Rectangle.Empty;

                // If a bounds tag was given, try to use that.
                string? boundsString = sourceNode.Attributes.GetAttributeOrDefault(boundsTagName, (string?)null);
                if (!string.IsNullOrWhiteSpace(boundsString))
                {
                    if (!ToRectangle.TryParse(boundsString, out sourceRectangle))
                        throw new Exception($"Image source with name {sourceNode.Name} has an invalid {boundsTagName} attribute.");
                }
                // Otherwise; if no bounds tag was given but a grid is being used, use that.
                else if (usingGrid)
                {
                    // If a specific cell position was given, use that.
                    string? tilePositionString = sourceNode.Attributes.GetAttributeOrDefault(tilePositionTagName, (string?)null);
                    if (!string.IsNullOrWhiteSpace(tilePositionString))
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
                        if (currentGridCell.X + 1 >= tilemapSize!.Value.X)
                        {
                            currentGridCell.X = 0;
                            currentGridCell.Y++;
                        }
                        else
                            currentGridCell.X++;
                    }

                }
                // Finally, if nothing at all was given and a grid isn't being used, throw an exception.
                else
                    throw new Exception($"Image source with name {sourceNode.Name} does not have enough information to be created. Must have either a {tilePositionTagName} or {boundsTagName} attribute.");

                // Create a new image with the root texture and user-defined bounds.
                Image image = new MonoGameImage(sourceNode.Name, texture, sourceRectangle.ToDrawingRectangle());

                // Add the image to the dictionary.
                AddImage(image);
            }
        }
        #endregion
    }
}
