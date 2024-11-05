using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.MonoGame.Rendering;
using LiruGameHelper.Parsers;
using LiruGameHelper.XML;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml;

namespace GuiCookie.MonoGame.Styles
{
    public class MonoGameResourceManager(ContentManager contentManager) : ResourceManager
    {
        #region Properties
        /// <summary> The <see cref="Microsoft.Xna.Framework.Content.ContentManager"/> used by this resource manager to load the resources. </summary>
        public ContentManager ContentManager { get; } = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
        #endregion

        #region Load Functions
        protected override void loadFontFromNode(XmlNode fontNode, string rootFolder)
        {
            // Get the URI of the font from the node.
            if (!fontNode.GetAttributeValue(uriAttributeName, out string fontURI))
                throw new Exception($"{fontsNodeName} node was missing {uriAttributeName} attribute.");

            // Load the font.
            SpriteFont spriteFont = ContentManager.Load<SpriteFont>(Path.Combine(rootFolder, fontURI));
            Font font = new MonoGameFont(spriteFont);

            // Add the font to the dictionary.
            AddFont(fontNode.Name, font);
        }

        protected override void loadImageFromNode(XmlNode imageNode, string rootFolder)
        {
            // Get the URI of the image from the node.
            if (!imageNode.GetAttributeValue(uriAttributeName, out string imageURI)) throw new Exception($"{imagesNodeName} node was missing {uriAttributeName} attribute.");

            // Load the texture.
            Texture2D texture = ContentManager.Load<Texture2D>(Path.Combine(rootFolder, imageURI));

            // Add the root image to the dictionary.
            AddImage(new Image(imageNode.Name, texture, texture.Bounds));

            // If a tilemap tag was given and it parses into a point, use the image as a tilemap.
            bool usingGrid = imageNode.TryParseAttributeValue(tilemapTagName, ToPoint.TryParse, out Point tilemapSize);
            GUIPoint currentGridCell = GUIPoint.Zero;
            GUIPoint tileSize = usingGrid ? (texture.Bounds.Size.ToVector2() / tilemapSize.ToVector2()).ToPoint() : GUIPoint.Zero;

            // If there are child nodes, add them as images.
            foreach (XmlNode sourceNode in imageNode)
            {
                // Create a new source rectangle to be used.
                GUIRectangle sourceRectangle = GUIRectangle.Empty;

                // If a bounds tag was given, try to use that.
                if (sourceNode.GetAttributeValue(boundsTagName, out string boundsString))
                { 
                    if (!ToRectangle.TryParse(boundsString, out sourceRectangle)) 
                        throw new Exception($"Image source with name {sourceNode.Name} has an invalid {boundsTagName} attribute."); 
                }
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
                        if (currentGridCell.X + 1 >= tilemapSize.X) 
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
                Image image = new Image(sourceNode.Name, texture, sourceRectangle);

                // Add the image to the dictionary.
                AddImage(image);
            }
        }
        #endregion
    }
}
