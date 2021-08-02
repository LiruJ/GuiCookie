using GuiCookie.Attributes;
using GuiCookie.DataStructures;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GuiCookie.Styles
{
    public class Font : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";

        public const string FontAttributeName = "Font";

        public const string AnchorAttributeName = "TextAnchor";
        public const string PivotAttributeName = "TextPivot";
        public const string ShadowColourAttributeName = "ShadowColour";
        public const string ShadowOffsetAttributeName = "ShadowOffset";

        public const string OffsetAttributeName = "Offset";

        public const string TintAttributeName = "Tint";
        #endregion

        #region Properties
        public string Name { get; }

        /// <summary> The <see cref="SpriteFont"/>. </summary>
        public SpriteFont SpriteFont { get; set; }

        /// <summary> The text colour. </summary>
        public Color? Colour { get; set; }

        public Color? Tint { get; set; }

        public Color FinalColour => Colour.HasValue && Tint.HasValue
            ? new Color(Colour.Value.ToVector4() * Tint.Value.ToVector4())
            : Colour ?? Tint ?? Color.Black;

        public Space? TextAnchor { get; set; }

        public Space? TextPivot { get; set; }

        public Vector2? DropShadowOffset { get; set; }

        public Color? DropShadowColour { get; set; }

        /// <summary> The amount of pixels to offset the final position by. This is useful for things like buttons that appear to be pushed inwards. </summary>
        public Vector2? Offset { get; set; }
        #endregion

        #region Constructors
        public Font(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Try get the font name from the attributes, default to null.
            string fontName = attributes.GetAttributeOrDefault(FontAttributeName, string.Empty);
            SpriteFont = !string.IsNullOrWhiteSpace(fontName) ?
                resourceManager.FontsByName.TryGetValue(fontName, out SpriteFont spriteFont) ? spriteFont : throw new Exception($"Font resource named \"{fontName}\" does not exist.")
                : null;
            
            // Parse the colour.
            Colour = resourceManager.GetColourOrDefault(attributes, ResourceManager.ColourAttributeName, null);
            Tint = resourceManager.GetColourOrDefault(attributes, TintAttributeName, null);

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);

            // Set the text anchor and pivot.
            TextAnchor = attributes.GetAttributeOrDefault(AnchorAttributeName, (Space?)null, Space.TryParse);
            TextPivot = attributes.GetAttributeOrDefault(PivotAttributeName, (Space?)null, Space.TryParse);
            
            // Set the drop shadow.
            DropShadowOffset = attributes.GetAttributeOrDefault(ShadowOffsetAttributeName, (Vector2?)null, ToVector2.TryParse);
            DropShadowColour = resourceManager.GetColourOrDefault(attributes, ShadowColourAttributeName);

            // Set the offset.
            Offset = attributes.GetAttributeOrDefault(OffsetAttributeName, (Vector2?)null, ToVector2.TryParse);
        }

        private Font(Font original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            SpriteFont = original.SpriteFont;
            Colour = original.Colour;
            Tint = original.Tint;

            TextAnchor = original.TextAnchor;
            TextPivot = original.TextPivot;

            DropShadowOffset = original.DropShadowOffset;
            DropShadowColour = original.DropShadowColour;

            Offset = original.Offset;
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

            // Override the base's properties.
            if (SpriteFont == null) SpriteFont = baseFont.SpriteFont;
            if (Colour == null) Colour = baseFont.Colour;
            if (Tint == null) Tint = baseFont.Tint;
            if (Offset == null) Offset = baseFont.Offset;
            if (DropShadowOffset == null) DropShadowOffset = baseFont.DropShadowOffset;
            if (DropShadowColour == null) DropShadowColour = baseFont.DropShadowColour;
        }
        #endregion
    }
}