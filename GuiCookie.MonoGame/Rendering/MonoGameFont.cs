using GuiCookie.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System.Numerics;
using System.Text;

namespace GuiCookie.MonoGame.Rendering
{
    public class MonoGameFont(SpriteFont spriteFont) : Font
    {
        #region Properties
        public SpriteFont SpriteFont { get; } = spriteFont;
        #endregion

        #region String Functions
        public override Vector2 MeasureString(string text)
            => SpriteFont.MeasureString(text).ToNumerics();

        public override Vector2 MeasureString(StringBuilder stringBuilder)
            => SpriteFont.MeasureString(stringBuilder).ToNumerics();
        #endregion
    }
}
