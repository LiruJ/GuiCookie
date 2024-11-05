using GuiCookie.Core.Rendering;
using GuiCookie.MonoGame.Rendering.Batching;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace GuiCookie.MonoGame.Rendering
{
    public class MonoGameRenderManager(GraphicsDevice graphicsDevice, UISpriteBatch spriteBatch) : IGuiCamera
    {
        #region Properties
        public GraphicsDevice GraphicsDevice { get; } = graphicsDevice;

        public UISpriteBatch SpriteBatch { get; } = spriteBatch;
        #endregion

        #region Constructors
        public MonoGameRenderManager(GraphicsDevice graphicsDevice) : this(graphicsDevice, new UISpriteBatch(graphicsDevice))
        {
            
        }
        #endregion

        #region Batch Functions
        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deferred,
                             BlendState? blendState = null,
                             SamplerState? samplerState = null,
                             DepthStencilState? depthStencilState = null,
                             RasterizerState? rasterizerState = null,
                             GuiCookieEffect? effect = null,
                             Matrix? transformMatrix = null)
            => SpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);

        public void End() => SpriteBatch.End();
        #endregion

        #region Draw Texture Functions
        public void DrawNineSlice(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth, nineSlice);

        public void DrawNineSlice(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth, nineSlice);

        public void DrawNineSlice(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, position, sourceRectangle, color, nineSlice);

        public void DrawNineSlice(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, destinationRectangle, sourceRectangle, color, nineSlice);

        public void DrawStretched(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawStretched(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);

        public void DrawStretched(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawStretched(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);

        public void DrawStretched(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawStretched(texture, position, sourceRectangle, color);

        public void DrawStretched(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawStretched(texture, destinationRectangle, sourceRectangle, color);

        public void DrawTiled(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawTiled(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);

        public void DrawTiled(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawTiled(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);

        public void DrawTiled(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawTiled(texture, position, sourceRectangle, color);

        public void DrawTiled(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawTiled(texture, destinationRectangle, sourceRectangle, color);
        #endregion

        public void DrawString(Font font, string text, Vector2 position, Color? colour = null)
        {
            if (font is not MonoGameFont monoGameFont)
                return;

            Microsoft.Xna.Framework.Color monoGameColour = Microsoft.Xna.Framework.Color.Black;
            if (colour != null)
                monoGameColour = new Microsoft.Xna.Framework.Color(colour.Value.R, colour.Value.G, colour.Value.B, colour.Value.A);

            SpriteBatch.DrawString(monoGameFont.SpriteFont, text, new Microsoft.Xna.Framework.Vector2(position.X, position.Y), monoGameColour);
        }

        public void DrawString(Font font, StringBuilder stringBuilder, Vector2 position, Color? colour = null)
        {
            if (font is not MonoGameFont monoGameFont)
                return;

            Microsoft.Xna.Framework.Color monoGameColour = Microsoft.Xna.Framework.Color.Black;
            if (colour != null)
                monoGameColour = new Microsoft.Xna.Framework.Color(colour.Value.R, colour.Value.G, colour.Value.B, colour.Value.A);

            SpriteBatch.DrawString(monoGameFont.SpriteFont, stringBuilder, new Microsoft.Xna.Framework.Vector2(position.X, position.Y), monoGameColour);
        }
    }
}
