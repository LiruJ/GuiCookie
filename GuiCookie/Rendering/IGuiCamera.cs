using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GuiCookie.Rendering
{
    /// <summary> Interfaces away a class that draws textures and fonts, implement this on your camera class to allow the UI to draw with it. </summary>
    public interface IGuiCamera
    {
        SpriteBatch SpriteBatch { get; }

        void DrawString(SpriteFont font, string text, Vector2 position, Color colour);

        void DrawTextureAt(Texture2D texture, Rectangle target);

        void DrawTextureAt(Texture2D texture, Rectangle target, Rectangle source, Color colour);
        void DrawTextureAt(Texture2D texture, Rectangle target, Color colour);
        void DrawTextureAt(Texture2D texture, Vector2 position, Rectangle source, Color colour);
    }
}
