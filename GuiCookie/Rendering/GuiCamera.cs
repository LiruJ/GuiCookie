using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GuiCookie.Rendering
{
    public class GuiCamera : IGuiCamera
    {
        #region Properties
        public SpriteBatch SpriteBatch { get; }

        public SamplerState SamplerState { get; set; } = SamplerState.LinearClamp;

        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        #endregion

        #region Constructors
        public GuiCamera(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch ?? throw new ArgumentNullException(nameof(spriteBatch));
        }
        #endregion

        #region Functions
        public void Begin() => SpriteBatch.Begin(blendState: BlendState, samplerState: SamplerState);

        public void End() => SpriteBatch.End();

        public void DrawTextureAt(Texture2D texture, Rectangle target) => SpriteBatch.Draw(texture, target, Color.White);

        public void DrawTextureAt(Texture2D texture, Vector2 position, Rectangle source, Color colour) => SpriteBatch.Draw(texture, position, source, colour, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

        public void DrawTextureAt(Texture2D texture, Rectangle target, Color colour) => SpriteBatch.Draw(texture, target, colour);

        public void DrawTextureAt(Texture2D texture, Rectangle target, Rectangle source, Color colour) => SpriteBatch.Draw(texture, target, source, colour, 0.0f, Vector2.Zero, SpriteEffects.None, 0.0f);

        public void DrawString(SpriteFont font, string text, Vector2 position, Color colour) => SpriteBatch.DrawString(font, text, position, colour);
        #endregion
    }
}
