using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace GuiCookie.MonoGame.Rendering.Batching
{
    public class UISpriteBatch : IDisposable
    {
        #region Fields
        private readonly UISpriteBatcher spriteBatcher;

        private bool beginCalled = false;

        private readonly GuiCookieEffect guiCookieEffect;

        private SpriteSortMode sortMode;
        private BlendState blendState;
        private SamplerState samplerState;
        private DepthStencilState depthStencilState;
        private RasterizerState rasterizerState;
        private GuiCookieEffect currentEffect;
        #endregion

        #region Properties
        public GraphicsDevice GraphicsDevice { get; }
        #endregion

        #region Constructors
        public UISpriteBatch(GraphicsDevice graphicsDevice) : this(graphicsDevice, new GuiCookieEffect(graphicsDevice), 0)
        {

        }

        public UISpriteBatch(GraphicsDevice graphicsDevice, GuiCookieEffect guiCookieEffect) : this(graphicsDevice, guiCookieEffect, 0)
        {
        }

        public UISpriteBatch(GraphicsDevice graphicsDevice, GuiCookieEffect guiCookieEffect, int capacity)
        {
            ArgumentNullException.ThrowIfNull(graphicsDevice, nameof(graphicsDevice));
            GraphicsDevice = graphicsDevice;

            this.guiCookieEffect = guiCookieEffect;
            currentEffect = guiCookieEffect;

            spriteBatcher = new(graphicsDevice, capacity);

            sortMode = SpriteSortMode.Deferred;
            blendState = BlendState.AlphaBlend;
            samplerState = SamplerState.LinearClamp;
            depthStencilState = DepthStencilState.None;
            rasterizerState = RasterizerState.CullCounterClockwise;
        }
        #endregion

        #region Validity Functions
        private void checkValid(Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            if (!beginCalled)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
        }

        private void checkValid(SpriteFont spriteFont, string text)
        {
            if (spriteFont == null)
                throw new ArgumentNullException("spriteFont");
            if (text == null)
                throw new ArgumentNullException("text");
            if (!beginCalled)
                throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        }

        private void checkValid(SpriteFont spriteFont, StringBuilder text)
        {
            if (spriteFont == null)
                throw new ArgumentNullException("spriteFont");
            if (text == null)
                throw new ArgumentNullException("text");
            if (!beginCalled)
                throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        }
        #endregion

        #region Setup Functions
        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred,
             BlendState? blendState = null,
             SamplerState? samplerState = null,
             DepthStencilState? depthStencilState = null,
             RasterizerState? rasterizerState = null,
             GuiCookieEffect? effect = null,
             Matrix? transformMatrix = null)
        {
            if (beginCalled)
                throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");

            this.sortMode = sortMode;
            this.blendState = blendState ?? BlendState.AlphaBlend;
            this.samplerState = samplerState ?? SamplerState.LinearClamp;
            this.depthStencilState = depthStencilState ?? DepthStencilState.None;
            this.rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
            currentEffect = effect ?? guiCookieEffect;
            currentEffect.TransformMatrix = transformMatrix;

            if (sortMode == SpriteSortMode.Immediate)
                setup();

            beginCalled = true;
        }

        public void End()
        {
            if (!beginCalled)
                throw new InvalidOperationException("Begin must be called before calling End.");

            beginCalled = false;

            if (sortMode != SpriteSortMode.Immediate)
                setup();

            spriteBatcher.DrawBatch(sortMode, currentEffect);
        }

        private void setup()
        {
            GraphicsDevice.BlendState = blendState;
            GraphicsDevice.DepthStencilState = depthStencilState;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.SamplerStates[0] = samplerState;
        }
        #endregion

        #region Draw Functions
        public void DrawNineSlice(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Vector4 nineSlice)
            => draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth, FrameDrawMode.NineSlice, nineSlice);

        public void DrawStretched(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth, FrameDrawMode.Stretch);

        public void DrawTiled(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth, FrameDrawMode.Tile);

        private void draw(Texture2D texture,
                            Vector2 position,
                            Rectangle? sourceRectangle,
                            Color color,
                            float rotation,
                            Vector2 origin,
                            Vector2 scale,
                            SpriteEffects effects,
                            float layerDepth,
                            FrameDrawMode drawMode,
                            Vector4? nineSlice = null)
        {
            checkValid(texture);

            UIBatchItem item = spriteBatcher.GetBatchItem();
            item.Texture = texture;
            item.DrawMode = drawMode;

            origin *= scale;

            // set SortKey based on SpriteSortMode.
            switch (sortMode)
            {
                // Comparison of Texture objects.
                case SpriteSortMode.Texture:
                    throw new NotSupportedException("Texture sprite sorting mode is not supported!");
                // Comparison of Depth
                case SpriteSortMode.FrontToBack:
                    item.SortKey = layerDepth;
                    break;
                // Comparison of Depth in reverse
                case SpriteSortMode.BackToFront:
                    item.SortKey = -layerDepth;
                    break;
            }

            Vector2 sourceSize, textureCoordTL, textureCoordBR;
            if (sourceRectangle.HasValue)
            {
                Rectangle srcRect = sourceRectangle.GetValueOrDefault();
                sourceSize = srcRect.Size.ToVector2() * scale;

                Vector2 texelSize = Vector2.One / texture.Bounds.Size.ToVector2();
                textureCoordTL = srcRect.Location.ToVector2() * texelSize;
                textureCoordBR = (srcRect.Location + srcRect.Size).ToVector2() * texelSize;
            }
            else
            {
                sourceSize = texture.Bounds.Size.ToVector2() * scale;
                textureCoordTL = Vector2.Zero;
                textureCoordBR = Vector2.One;
            }

            if ((effects & SpriteEffects.FlipVertically) != 0)
                (textureCoordTL.Y, textureCoordBR.Y) = (textureCoordBR.Y, textureCoordTL.Y);
            if ((effects & SpriteEffects.FlipHorizontally) != 0)
                (textureCoordTL.X, textureCoordBR.X) = (textureCoordBR.X, textureCoordTL.X);

            if (rotation == 0f)
            {
                item.Set(position - origin,
                        sourceSize,
                        sourceSize,
                        color,
                        textureCoordTL,
                        textureCoordBR,
                        layerDepth,
                        nineSlice);
            }
            else
            {
                item.Set(position,
                        -origin.X,
                        -origin.Y,
                        sourceSize,
                        sourceSize,
                        MathF.Sin(rotation),
                        MathF.Cos(rotation),
                        color,
                        textureCoordTL,
                        textureCoordBR,
                        layerDepth,
                        nineSlice);
            }

            FlushIfNeeded();
        }

        public void DrawNineSlice(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth, Vector4 nineSlice)
            => draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth, FrameDrawMode.NineSlice, nineSlice);

        public void DrawStretched(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth, FrameDrawMode.Stretch);

        public void DrawTiled(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth, FrameDrawMode.Tile);

        private void draw(Texture2D texture,
                            Rectangle destinationRectangle,
                            Rectangle? sourceRectangle,
                            Color color,
                            float rotation,
                            Vector2 origin,
                            SpriteEffects effects,
                            float layerDepth,
                            FrameDrawMode drawMode,
                            Vector4? nineSlice = null)
        {
            checkValid(texture);

            UIBatchItem item = spriteBatcher.GetBatchItem();
            item.Texture = texture;
            item.DrawMode = drawMode;

            // set SortKey based on SpriteSortMode.
            switch (sortMode)
            {
                // Comparison of Texture objects.
                case SpriteSortMode.Texture:
                    throw new NotSupportedException("Texture sprite sorting mode is not supported!");
                // Comparison of Depth
                case SpriteSortMode.FrontToBack:
                    item.SortKey = layerDepth;
                    break;
                // Comparison of Depth in reverse
                case SpriteSortMode.BackToFront:
                    item.SortKey = -layerDepth;
                    break;
            }

            Vector2 sourceSize, textureCoordTL, textureCoordBR;
            Vector2 texelSize = Vector2.One / texture.Bounds.Size.ToVector2();
            if (sourceRectangle.HasValue)
            {
                Rectangle sourceRect = sourceRectangle.GetValueOrDefault();
                sourceSize = sourceRect.Size.ToVector2();

                textureCoordTL = sourceRect.Location.ToVector2() * texelSize;
                textureCoordBR = (sourceRect.Location + sourceRect.Size).ToVector2() * texelSize;

                if (sourceRect.Width != 0)
                    origin.X = origin.X * (float)destinationRectangle.Width / (float)sourceRect.Width;
                else
                    origin.X = origin.X * (float)destinationRectangle.Width * texelSize.X;
                if (sourceRect.Height != 0)
                    origin.Y = origin.Y * (float)destinationRectangle.Height / (float)sourceRect.Height;
                else
                    origin.Y = origin.Y * (float)destinationRectangle.Height * texelSize.Y;
            }
            else
            {
                sourceSize = texture.Bounds.Size.ToVector2();
                textureCoordTL = Vector2.Zero;
                textureCoordBR = Vector2.One;

                origin *= destinationRectangle.Size.ToVector2() * texelSize;
            }

            if ((effects & SpriteEffects.FlipVertically) != 0)
                (textureCoordTL.Y, textureCoordBR.Y) = (textureCoordBR.Y, textureCoordTL.Y);
            if ((effects & SpriteEffects.FlipHorizontally) != 0)
                (textureCoordTL.X, textureCoordBR.X) = (textureCoordBR.X, textureCoordTL.X);


            if (rotation == 0f)
            {
                item.Set(destinationRectangle.Location.ToVector2() - origin,
                        destinationRectangle.Size.ToVector2(),
                        sourceSize,
                        color,
                        textureCoordTL,
                        textureCoordBR,
                        layerDepth,
                        nineSlice);
            }
            else
            {
                item.Set(destinationRectangle.Location.ToVector2(),
                        -origin.X,
                        -origin.Y,
                        destinationRectangle.Size.ToVector2(),
                        sourceSize,
                        MathF.Sin(rotation),
                        MathF.Cos(rotation),
                        color,
                        textureCoordTL,
                        textureCoordBR,
                        layerDepth,
                        nineSlice);
            }

            FlushIfNeeded();
        }

        public void DrawNineSlice(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Vector4 nineSlice)
            => draw(texture, position, sourceRectangle, color, FrameDrawMode.NineSlice, nineSlice);

        public void DrawStretched(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            => draw(texture, position, sourceRectangle, color, FrameDrawMode.Stretch);

        public void DrawTiled(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            => draw(texture, position, sourceRectangle, color, FrameDrawMode.Tile);

        private void draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, FrameDrawMode drawMode, Vector4? nineSlice = null)
        {
            checkValid(texture);

            UIBatchItem item = spriteBatcher.GetBatchItem();
            item.Texture = texture;
            item.DrawMode = drawMode;
            item.SortKey = 0;

            if (sortMode == SpriteSortMode.Texture)
                throw new NotSupportedException("Texture sprite sorting mode is not supported!");

            Vector2 sourceSize, textureCoordTL, textureCoordBR;
            if (sourceRectangle.HasValue)
            {
                Rectangle sourceRect = sourceRectangle.GetValueOrDefault();
                sourceSize = sourceRect.Size.ToVector2();

                Vector2 texelSize = Vector2.One / texture.Bounds.Size.ToVector2();
                textureCoordTL = sourceRect.Location.ToVector2() * texelSize;
                textureCoordBR = (sourceRect.Location + sourceRect.Size).ToVector2() * texelSize;
            }
            else
            {
                sourceSize = texture.Bounds.Size.ToVector2();
                textureCoordTL = Vector2.Zero;
                textureCoordBR = Vector2.One;
            }

            item.Set(position,
                     sourceSize,
                     sourceSize,
                     color,
                     textureCoordTL,
                     textureCoordBR,
                     0,
                     nineSlice);

            FlushIfNeeded();
        }

        public void DrawNineSlice(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector4 nineSlice)
            => draw(texture, destinationRectangle, sourceRectangle, color, FrameDrawMode.NineSlice, nineSlice);

        public void DrawStretched(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            => draw(texture, destinationRectangle, sourceRectangle, color, FrameDrawMode.Stretch);

        public void DrawTiled(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            => draw(texture, destinationRectangle, sourceRectangle, color, FrameDrawMode.Tile);

        private void draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, FrameDrawMode drawMode, Vector4? nineSlice = null)
        {
            checkValid(texture);

            UIBatchItem item = spriteBatcher.GetBatchItem();
            item.Texture = texture;
            item.DrawMode = drawMode;
            item.SortKey = 0;

            if (sortMode == SpriteSortMode.Texture)
                throw new NotSupportedException("Texture sprite sorting mode is not supported!");

            Vector2 textureCoordTL, textureCoordBR, sourceSize;
            if (sourceRectangle.HasValue)
            {
                Rectangle srcRect = sourceRectangle.GetValueOrDefault();
                Vector2 texelSize = Vector2.One / texture.Bounds.Size.ToVector2();
                textureCoordTL = srcRect.Location.ToVector2() * texelSize;
                textureCoordBR = (srcRect.Location + srcRect.Size).ToVector2() * texelSize;
                sourceSize = srcRect.Size.ToVector2();
            }
            else
            {
                sourceSize = texture.Bounds.Size.ToVector2();
                textureCoordTL = Vector2.Zero;
                textureCoordBR = Vector2.One;
            }

            item.Set(destinationRectangle.Location.ToVector2(),
                     destinationRectangle.Size.ToVector2(),
                     sourceSize,
                     color,
                     textureCoordTL,
                     textureCoordBR,
                     0,
                     nineSlice);

            FlushIfNeeded();
        }

        // Mark the end of a draw operation for Immediate SpriteSortMode.
        internal void FlushIfNeeded()
        {
            if (sortMode == SpriteSortMode.Immediate)
            {
                spriteBatcher.DrawBatch(sortMode, currentEffect);
            }
        }
        #endregion

        #region Disposal Functions
        public void Dispose()
        {
            if (!guiCookieEffect.IsDisposed)
                guiCookieEffect.Dispose();
        }
        #endregion
    }
}
