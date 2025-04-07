using GuiCookie.Core.Rendering;
using GuiCookie.MonoGame.Extensions;
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
        void IGuiCamera.DrawNineSlice(Image image,
                                      System.Drawing.Rectangle destinationRectangle,
                                      System.Drawing.Rectangle? sourceRectangle,
                                      System.Drawing.Color color,
                                      float rotation,
                                      System.Numerics.Vector2 origin,
                                      float layerDepth,
                                      System.Numerics.Vector4 nineSlice)
        {
            if (image is MonoGameImage monoGameImage)
                DrawNineSlice(monoGameImage.Texture,
                              destinationRectangle.ToMonoGameRectangle(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              rotation,
                              origin.ToMonoGameVector(),
                              SpriteEffects.None,
                              layerDepth,
                              nineSlice.ToMonoGameVector());
        }

        public void DrawNineSlice(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth, nineSlice);

        void IGuiCamera.DrawNineSlice(Image image,
                                      System.Numerics.Vector2 position,
                                      System.Drawing.Rectangle? sourceRectangle,
                                      System.Drawing.Color color,
                                      float rotation,
                                      System.Numerics.Vector2 origin,
                                      System.Numerics.Vector2 scale,
                                      float layerDepth,
                                      System.Numerics.Vector4 nineSlice)
        {
            if (image is MonoGameImage monoGameImage)
                DrawNineSlice(monoGameImage.Texture,
                              position.ToMonoGameVector(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              rotation,
                              origin.ToMonoGameVector(),
                              scale.ToMonoGameVector(),
                              SpriteEffects.None,
                              layerDepth,
                              nineSlice.ToMonoGameVector());
        }

        public void DrawNineSlice(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth, nineSlice);

        void IGuiCamera.DrawNineSlice(Image image,
                                      System.Numerics.Vector2 position,
                                      System.Drawing.Rectangle? sourceRectangle,
                                      System.Drawing.Color color,
                                      System.Numerics.Vector4 nineSlice)
        {
            if (image is MonoGameImage monoGameImage)
                DrawNineSlice(monoGameImage.Texture,
                              position.ToMonoGameVector(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              nineSlice.ToMonoGameVector());
        }

        public void DrawNineSlice(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, position, sourceRectangle, color, nineSlice);

        void IGuiCamera.DrawNineSlice(Image image, System.Drawing.Rectangle destinationRectangle, System.Drawing.Rectangle? sourceRectangle, System.Drawing.Color color, System.Numerics.Vector4 nineSlice)
        {
            if (image is MonoGameImage monoGameImage)
                DrawNineSlice(monoGameImage.Texture,
                              destinationRectangle.ToMonoGameRectangle(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              nineSlice.ToMonoGameVector());
        }

        public void DrawNineSlice(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, Vector4 nineSlice)
            => SpriteBatch.DrawNineSlice(texture, destinationRectangle, sourceRectangle, color, nineSlice);

        void IGuiCamera.DrawStretched(Image image,
                                      System.Drawing.Rectangle destinationRectangle,
                                      System.Drawing.Rectangle? sourceRectangle,
                                      System.Drawing.Color color,
                                      float rotation,
                                      System.Numerics.Vector2 origin,
                                      float layerDepth)
        {
            if (image is MonoGameImage monoGameImage)
                DrawStretched(monoGameImage.Texture,
                              destinationRectangle.ToMonoGameRectangle(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              rotation,
                              origin.ToMonoGameVector(),
                              SpriteEffects.None,
                              layerDepth);
        }

        public void DrawStretched(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawStretched(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);

        void IGuiCamera.DrawStretched(Image image,
                                      System.Numerics.Vector2 position,
                                      System.Drawing.Rectangle? sourceRectangle,
                                      System.Drawing.Color color,
                                      float rotation,
                                      System.Numerics.Vector2 origin,
                                      System.Numerics.Vector2 scale,
                                      float layerDepth)
        {
            if (image is MonoGameImage monoGameImage)
                DrawStretched(monoGameImage.Texture,
                              position.ToMonoGameVector(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              rotation,
                              origin.ToMonoGameVector(),
                              scale.ToMonoGameVector(),
                              SpriteEffects.None,
                              layerDepth);
        }

        public void DrawStretched(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawStretched(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);

        void IGuiCamera.DrawStretched(Image image,
                                      System.Numerics.Vector2 position,
                                      System.Drawing.Rectangle? sourceRectangle,
                                      System.Drawing.Color color)
        {
            if (image is MonoGameImage monoGameImage)
                DrawStretched(monoGameImage.Texture,
                              position.ToMonoGameVector(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour());
        }

        public void DrawStretched(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawStretched(texture, position, sourceRectangle, color);

        void IGuiCamera.DrawStretched(Image image,
                                      System.Drawing.Rectangle destinationRectangle,
                                      System.Drawing.Rectangle? sourceRectangle,
                                      System.Drawing.Color color)
        {
            if (image is MonoGameImage monoGameImage)
                DrawStretched(monoGameImage.Texture,
                              destinationRectangle.ToMonoGameRectangle(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour());
        }

        public void DrawStretched(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawStretched(texture, destinationRectangle, sourceRectangle, color);

        void IGuiCamera.DrawTiled(Image image,
                                  System.Drawing.Rectangle destinationRectangle,
                                  System.Drawing.Rectangle? sourceRectangle,
                                  System.Drawing.Color color,
                                  float rotation,
                                  System.Numerics.Vector2 origin,
                                  float layerDepth)
        {
            if (image is MonoGameImage monoGameImage)
                DrawTiled(monoGameImage.Texture,
                              destinationRectangle.ToMonoGameRectangle(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              rotation,
                              origin.ToMonoGameVector(),
                              SpriteEffects.None,
                              layerDepth);
        }

        public void DrawTiled(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawTiled(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);

        void IGuiCamera.DrawTiled(Image image,
                                  System.Numerics.Vector2 position,
                                  System.Drawing.Rectangle? sourceRectangle,
                                  System.Drawing.Color color,
                                  float rotation,
                                  System.Numerics.Vector2 origin,
                                  System.Numerics.Vector2 scale,
                                  float layerDepth)
        {
            if (image is MonoGameImage monoGameImage)
                DrawTiled(monoGameImage.Texture,
                              position.ToMonoGameVector(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour(),
                              rotation,
                              origin.ToMonoGameVector(),
                              scale.ToMonoGameVector(),
                              SpriteEffects.None,
                              layerDepth);
        }

        public void DrawTiled(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
            => SpriteBatch.DrawTiled(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);

        void IGuiCamera.DrawTiled(Image image,
                                  System.Numerics.Vector2 position,
                                  System.Drawing.Rectangle? sourceRectangle,
                                  System.Drawing.Color color)
        {
            if (image is MonoGameImage monoGameImage)
                DrawTiled(monoGameImage.Texture,
                              position.ToMonoGameVector(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour());
        }

        public void DrawTiled(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawTiled(texture, position, sourceRectangle, color);

        void IGuiCamera.DrawTiled(Image image,
                                  System.Drawing.Rectangle destinationRectangle,
                                  System.Drawing.Rectangle? sourceRectangle,
                                  System.Drawing.Color color)
        {
            if (image is MonoGameImage monoGameImage)
                DrawTiled(monoGameImage.Texture,
                              destinationRectangle.ToMonoGameRectangle(),
                              sourceRectangle?.ToMonoGameRectangle(),
                              color.ToMonoGameColour());
        }

        public void DrawTiled(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
            => SpriteBatch.DrawTiled(texture, destinationRectangle, sourceRectangle, color);
        #endregion

        #region Draw Text Functions

        void IGuiCamera.DrawString(Font font, string text, System.Numerics.Vector2 position, System.Drawing.Color? colour)
            => DrawString(font, text, new Vector2(position.X, position.Y), colour != null ? new Color((uint)colour.Value.ToArgb()) : null);

        public void DrawString(Font font, string text, Vector2 position, Color? colour = null)
        {
            if (font is not MonoGameFont monoGameFont)
                return;

            Color monoGameColour = Color.Black;
            if (colour != null)
                monoGameColour = new Color(colour.Value.R, colour.Value.G, colour.Value.B, colour.Value.A);

            //SpriteBatch.DrawString(monoGameFont.SpriteFont, text, new Vector2(position.X, position.Y), monoGameColour);
        }

        public void DrawString(Font font, StringBuilder stringBuilder, Vector2 position, Color? colour = null)
        {
            if (font is not MonoGameFont monoGameFont)
                return;

            Color monoGameColour = Color.Black;
            if (colour != null)
                monoGameColour = new Color(colour.Value.R, colour.Value.G, colour.Value.B, colour.Value.A);

            //SpriteBatch.DrawString(monoGameFont.SpriteFont, stringBuilder, new Vector2(position.X, position.Y), monoGameColour);
        }

        void IGuiCamera.DrawString(Font font, StringBuilder stringBuilder, System.Numerics.Vector2 position, System.Drawing.Color? colour)
        {


            throw new System.NotImplementedException();
        }
        #endregion
    }
}
