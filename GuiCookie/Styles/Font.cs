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

        public const string OffsetAttributeName = "Offset";
        #endregion

        #region Properties
        public string Name { get; }

        /// <summary> The <see cref="Microsoft.Xna.Framework.Graphics.SpriteFont"/>. </summary>
        public SpriteFont SpriteFont { get; set; }

        /// <summary> The colour and tint applied to the frame. </summary>
        public TintedColour TintedColour { get; set; }

        /// <summary> Accessor for <see cref="TintedColour.Colour"/>. </summary>
        public Color? Colour
        {
            get => TintedColour.Colour;
            set => TintedColour = new TintedColour(value, TintedColour.Tint);
        }

        /// <summary> Accessor for <see cref="TintedColour.Tint"/>. </summary>
        public Color? Tint
        {
            get => TintedColour.Tint;
            set => TintedColour = new TintedColour(TintedColour.Colour, value);
        }

        /// <summary> Accessor for <see cref="TintedColour.Mixed"/>. </summary>
        public Color MixedColour => TintedColour.Mixed;

        public Space? TextAnchor { get; set; }

        public Space? TextPivot { get; set; }

        /// <summary> The drop shadow data used to drop a shadow behind the text. </summary>
        public DropShadow DropShadow { get; set; }

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
            TintedColour = new TintedColour(resourceManager, attributes);

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);

            // Set the text anchor and pivot.
            TextAnchor = attributes.GetAttributeOrDefault(AnchorAttributeName, (Space?)null, Space.TryParse);
            TextPivot = attributes.GetAttributeOrDefault(PivotAttributeName, (Space?)null, Space.TryParse);

            // Set the drop shadow.
            DropShadow = new DropShadow(resourceManager, attributes);

            // Set the offset.
            Offset = attributes.GetAttributeOrDefault(OffsetAttributeName, (Vector2?)null, ToVector2.TryParse);
        }

        private Font(Font original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            SpriteFont = original.SpriteFont;
            TintedColour = original.TintedColour;

            TextAnchor = original.TextAnchor;
            TextPivot = original.TextPivot;

            DropShadow = original.DropShadow;

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
            TintedColour = TintedColour.CreateCombination(baseFont.TintedColour, TintedColour);
            DropShadow = DropShadow.CreateCombination(baseFont.DropShadow, DropShadow);
            if (Offset == null) Offset = baseFont.Offset;
        }
        #endregion
    }
}