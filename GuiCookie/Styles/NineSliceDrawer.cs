using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GuiCookie.Styles
{
    public class NineSliceDrawer : IDisposable, ITextureCreator
    {
        #region Dependencies
        private readonly GraphicsDevice graphicsDevice;
        #endregion

        #region Fields
        private static Texture2D whitePixel;

        private readonly SpriteBatch creatorSpriteBatch;
        #endregion

        #region Constructors
        public NineSliceDrawer(GraphicsDevice graphicsDevice)
        {
            // Set dependencies.
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));

            // Initialise spritebatch.
            creatorSpriteBatch = new SpriteBatch(graphicsDevice);

            // Create the texture.
            if (whitePixel == null)
            {
                whitePixel = new Texture2D(graphicsDevice, 1, 1);
                whitePixel.SetData(new Color[] { Color.White });
            }
        }
        #endregion

        #region Draw Functions
        public void DrawFrameCached(SliceFrame sliceFrame, Point size, ref Texture2D texture)
        {
            // If the given texture is null, create a new one.
            if (texture == null) texture = new RenderTarget2D(graphicsDevice, size.X, size.Y);

            // Set the render target to the given texture.
            if (!(texture is RenderTarget2D renderTarget)) throw new ArgumentException("Given texture was not a render target.");
            graphicsDevice.SetRenderTarget(renderTarget);

            // Clear the target.
            graphicsDevice.Clear(Color.Transparent);

            // Begin the spritebatch.
            creatorSpriteBatch.Begin();

            // Create the destination rectangle, which is simply the size of the element.
            Rectangle destination = new Rectangle(Point.Zero, size);

            // Draw the frame.
            drawNineSlice(sliceFrame.Image, sliceFrame.NineSlice, destination, creatorSpriteBatch, null, null);

            // End the spritebatch, drawing to the texture.
            creatorSpriteBatch.End();

            // Set the target to null, resetting it.
            graphicsDevice.SetRenderTarget(null);
        }

        public static void DrawFrameOnDemand(SliceFrame sliceFrame, Rectangle destinationRectangle, IGuiCamera camera, Color? colour) 
            => drawNineSlice(sliceFrame.Image, sliceFrame.NineSlice, destinationRectangle, null, camera, colour);

        public static void DrawFrameOnDemand(Image? image, NineSlice? nineSlice, Rectangle destinationRectangle, IGuiCamera camera, Color? colour) 
            => drawNineSlice(image, nineSlice, destinationRectangle, null, camera, colour);

        private static void drawNineSlice(Image? image, NineSlice? nineSlice, Rectangle mainDestination, SpriteBatch spriteBatch, IGuiCamera camera, Color? colour)
        {
            // If the sliceFrame's slice is null, just draw the texture.
            if (image != null && nineSlice == null)
            {
                drawPiece(image.Value.Texture, mainDestination, image.Value.Source, spriteBatch, camera, colour);
                return;
            }
            // Otherwise; If the background's nineslice or the given style image is null, just use a white texture.
            else if (nineSlice == null || image == null)
            {
                drawPiece(whitePixel, mainDestination, whitePixel.Bounds, spriteBatch, camera, colour);
                return;
            }
            // Otherwise; create the texture like normal.

            // Calculate the sources for each piece.
            Texture2D sourceTexture = image.Value.Texture;
            Rectangle mainSource = image.Value.Source;
            Rectangle topLeftSource = nineSlice.Value.CalculateSource(mainSource, Piece.TopLeft);
            Rectangle topRightSource = nineSlice.Value.CalculateSource(mainSource, Piece.TopRight);
            Rectangle bottomLeftSource = nineSlice.Value.CalculateSource(mainSource, Piece.BottomLeft);
            Rectangle bottomRightSource = nineSlice.Value.CalculateSource(mainSource, Piece.BottomRight);
            Rectangle leftSource = nineSlice.Value.CalculateSource(mainSource, Piece.Left);
            Rectangle rightSource = nineSlice.Value.CalculateSource(mainSource, Piece.Right);
            Rectangle topSource = nineSlice.Value.CalculateSource(mainSource, Piece.Top);
            Rectangle bottomSource = nineSlice.Value.CalculateSource(mainSource, Piece.Bottom);
            Rectangle centreSource = nineSlice.Value.CalculateSource(mainSource, Piece.Centre);

            // Centre.
            if (centreSource.Width > 0 && centreSource.Height > 0)
            {
                // Calculate the full area that the centre needs to cover.
                Rectangle innerDestination = new Rectangle(new Point(topLeftSource.Width, topLeftSource.Height) + mainDestination.Location, mainDestination.Size - (topLeftSource.Size + bottomRightSource.Size));

                // If the piece is stretched, just do one draw call.
                if (nineSlice.Value.IsPieceStretched(Piece.Centre))
                    drawPiece(sourceTexture, innerDestination, centreSource, spriteBatch, camera, colour);
                // Otherwise; tile it.
                else
                    for (int x = innerDestination.Left; x < innerDestination.Right; x += centreSource.Width)
                        for (int y = innerDestination.Top; y < innerDestination.Bottom; y += centreSource.Height)
                        {
                            Point usedSize = new Point(Math.Min(centreSource.Width, innerDestination.Right - x), Math.Min(centreSource.Height, innerDestination.Bottom - y));
                            drawPiece(sourceTexture, new Rectangle(new Point(x, y), usedSize), new Rectangle(centreSource.Location, usedSize), spriteBatch, camera, colour);
                        }
            }

            // Corners.
            if (topLeftSource.Width > 0 && topLeftSource.Height > 0)
                drawPiece(sourceTexture, new Rectangle(mainDestination.Location, topLeftSource.Size), topLeftSource, spriteBatch, camera, colour);
            if (topRightSource.Width > 0 && topRightSource.Height > 0)
                drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.Right - topRightSource.Width, mainDestination.Y), topRightSource.Size), topRightSource, spriteBatch, camera, colour);
            if (bottomLeftSource.Width > 0 && bottomLeftSource.Height > 0)
                drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.X, mainDestination.Bottom - bottomLeftSource.Height), bottomLeftSource.Size), bottomLeftSource, spriteBatch, camera, colour);
            if (bottomRightSource.Width > 0 && bottomRightSource.Height > 0)
                drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.Right - bottomRightSource.Width, mainDestination.Bottom - bottomRightSource.Height), bottomRightSource.Size),
                bottomRightSource, spriteBatch, camera, colour);

            // Left/Right.
            bool stretchLeft = nineSlice.Value.IsPieceStretched(Piece.Left), stretchRight = nineSlice.Value.IsPieceStretched(Piece.Right);
            bool drawLeft = leftSource.Width > 0 && leftSource.Height > 0, drawRight = rightSource.Height > 0 && rightSource.Width > 0;

            if (drawLeft && stretchLeft)
                drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.X, mainDestination.Y + topLeftSource.Height), new Point(leftSource.Width, mainDestination.Height - (topLeftSource.Height + bottomLeftSource.Height))),
                    leftSource, spriteBatch, camera, colour);

            if (drawRight && stretchRight)
                drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.Right - rightSource.Width, mainDestination.Y + topRightSource.Height), new Point(rightSource.Width, mainDestination.Height - (topRightSource.Height + bottomRightSource.Height))),
                    rightSource, spriteBatch, camera, colour);

            if ((drawLeft && !stretchLeft) || (drawRight && !stretchRight))
                for (int y = topLeftSource.Height; y < mainDestination.Height - bottomLeftSource.Height; y += leftSource.Height)
                {
                    int usedHeight = Math.Min(leftSource.Height, (mainDestination.Height - bottomLeftSource.Height) - y);
                    if (drawLeft && !stretchLeft)
                        drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.X, mainDestination.Y + y), new Point(leftSource.Width, usedHeight)),
                        new Rectangle(leftSource.Location, new Point(leftSource.Width, usedHeight)), spriteBatch, camera, colour);
                    if (drawRight && !stretchRight)
                        drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.Right - rightSource.Width, mainDestination.Y + y), new Point(rightSource.Width, usedHeight)),
                            new Rectangle(rightSource.Location, new Point(rightSource.Width, usedHeight)), spriteBatch, camera, colour);
                }

            // Top/Bottom.
            bool stretchTop = nineSlice.Value.IsPieceStretched(Piece.Top), stretchBottom = nineSlice.Value.IsPieceStretched(Piece.Bottom);
            bool drawTop = topSource.Width > 0 && topSource.Height > 0, drawBottom = bottomSource.Width > 0 && bottomSource.Height > 0;

            if (drawTop && stretchTop)
                drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.X + topLeftSource.Width, mainDestination.Y), new Point(mainDestination.Width - (topLeftSource.Width + topRightSource.Width), topSource.Height)),
                    topSource, spriteBatch, camera, colour);

            if (drawBottom && stretchBottom)
                drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.X + bottomLeftSource.Width, mainDestination.Bottom - bottomSource.Height), new Point(mainDestination.Width - (bottomLeftSource.Width + bottomRightSource.Width), bottomSource.Height)),
                    bottomSource, spriteBatch, camera, colour);

            if ((drawTop && !stretchTop) || (drawBottom && !stretchBottom))
                for (int x = topLeftSource.Width; x < mainDestination.Width - topRightSource.Width; x += topSource.Width)
                {
                    int usedWidth = Math.Min(topSource.Width, (mainDestination.Width - topRightSource.Width) - x);
                    if (drawTop && !stretchTop)
                        drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.X + x, mainDestination.Y), new Point(usedWidth, topSource.Height)),
                            new Rectangle(topSource.Location, new Point(usedWidth, topSource.Height)), spriteBatch, camera, colour);
                    if (drawBottom && !stretchBottom)
                        drawPiece(sourceTexture, new Rectangle(new Point(mainDestination.X + x, mainDestination.Bottom - bottomSource.Height), new Point(usedWidth, bottomSource.Height)),
                            new Rectangle(bottomSource.Location, new Point(usedWidth, bottomSource.Height)), spriteBatch, camera, colour);
                }
        }

        private static void drawPiece(Texture2D texture, Rectangle destination, Rectangle source, SpriteBatch spriteBatch, IGuiCamera camera, Color? colour)
        {
            if (camera != null) camera.DrawTextureAt(texture, destination, source, colour ?? Color.White);
            else if (spriteBatch != null) spriteBatch.Draw(texture, destination, source, colour ?? Color.White);
            else throw new Exception("Both camera and spritebatch were null.");
        }
        #endregion

        #region Disposal Functions
        public void Dispose()
        {
            creatorSpriteBatch.Dispose();
        }
        #endregion
    }
}
