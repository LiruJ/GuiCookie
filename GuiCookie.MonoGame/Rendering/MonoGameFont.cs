using GuiCookie.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static Microsoft.Xna.Framework.Graphics.SpriteFont;

namespace GuiCookie.MonoGame.Rendering
{
    public class MonoGameFont : Font
    {
        #region Properties
        public SpriteFont SpriteFont { get; }

        public Dictionary<char, Glyph> GlyphsByChar { get; }

        public Glyph? DefaultGlyph { get; }

        public MonoGameFont(SpriteFont spriteFont)
        {
            SpriteFont = spriteFont;
            GlyphsByChar = spriteFont.GetGlyphs();
            DefaultGlyph = SpriteFont.DefaultCharacter != null && GlyphsByChar.TryGetValue(SpriteFont.DefaultCharacter.Value, out Glyph defaultGlyph) ? defaultGlyph : null;
        }
        #endregion

        #region String Functions
        public override Vector2 MeasureString(string text)
            => SpriteFont.MeasureString(text).ToNumerics();

        public override Vector2 MeasureString(StringBuilder stringBuilder)
            => SpriteFont.MeasureString(stringBuilder).ToNumerics();
        #endregion

        #region Glyph Functions
        public Glyph GetGlyphOrDefault(char character)
        {
            if (GlyphsByChar.TryGetValue(character, out Glyph glyph)) return glyph;
            else if (DefaultGlyph.HasValue) return DefaultGlyph.Value;
            else throw new Exception($"{SpriteFont} does not define a character for '{character}'.");
        }
        #endregion
    }
}
