using GuiCookie.Attributes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GuiCookie.Styles
{
    public class Font : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";
        private const string fontAttributeName = "Font";
        #endregion

        #region Properties
        public string Name { get; }

        /// <summary> The <see cref="SpriteFont"/>. </summary>
        public SpriteFont SpriteFont { get; set; }

        /// <summary> The text colour. </summary>
        public Color? Colour { get; set; }
        #endregion

        #region Constructors
        public Font(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Try get the font name from the attributes, default to null.
            string fontName = attributes.GetAttributeOrDefault(fontAttributeName, string.Empty);
            SpriteFont = !string.IsNullOrWhiteSpace(fontName) ?
                resourceManager.FontsByName.TryGetValue(fontName, out SpriteFont spriteFont) ? spriteFont : throw new Exception($"Font resource named \"{fontName}\" does not exist.")
                : null;
            
            // Parse the colour.
            Colour = resourceManager.GetColourOrDefault(attributes, ResourceManager.ColourAttributeName, null);

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);
        }

        private Font(Font original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            SpriteFont = original.SpriteFont;
            Colour = original.Colour;
        }
        #endregion

        #region Copy Functions
        public IStyleAttribute CreateCopy() => new Font(this);
        #endregion

        #region Combination Functions
        public void OverrideBaseAttribute(IStyleAttribute baseAttribute)
        {
#if DEBUG
            // Validity checks.
            if (baseAttribute == null) throw new ArgumentNullException(nameof(baseAttribute));
            if (baseAttribute.Name != Name) throw new Exception($"Font name mismatch; {baseAttribute.Name}, {Name}");
#endif

            // Ensure the attribute is a font.
            if (!(baseAttribute is Font baseFont)) throw new ArgumentException($"Cannot combine with attribute as it is not a font. {baseAttribute}");

            // Override the base's font and colour.
            if (SpriteFont == null) SpriteFont = baseFont.SpriteFont;
            if (Colour == null) Colour = baseFont.Colour;
        }
        #endregion
    }
}
