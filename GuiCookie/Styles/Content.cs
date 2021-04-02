using GuiCookie.Attributes;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Styles
{
    public class Content : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";

        public const string ShadowColourAttributeName = "ShadowColour";
        public const string ShadowOffsetAttributeName = "ShadowOffset";

        public const string TintAttributeName = "Tint";
        #endregion        

        #region Properties
        public string Name { get; }

        /// <summary> The main colour applied to content (image blocks, etc.). </summary>
        public Color? Colour { get; set; }

        public Color? Tint { get; set; }

        public Color FinalColour => Colour.HasValue && Tint.HasValue
            ? new Color(Colour.Value.ToVector4() * Tint.Value.ToVector4())
            : Colour ?? Tint ?? Color.White;

        public Vector2? DropShadowOffset { get; set; }

        public Color DropShadowColour { get; set; }
        #endregion

        #region Constructors
        public Content(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Set the colour.
            Colour = resourceManager.GetColourOrDefault(attributes, ResourceManager.ColourAttributeName);
            Tint = resourceManager.GetColourOrDefault(attributes, TintAttributeName, null);

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);

            // Set the drop shadow.
            DropShadowOffset = attributes.GetAttributeOrDefault(ShadowOffsetAttributeName, (Vector2?)null, ToVector2.TryParse);
            DropShadowColour = resourceManager.GetColourOrDefault(attributes, ShadowColourAttributeName, Color.Black).Value;
        }

        private Content(Content original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;

            Colour = original.Colour;
            Tint = original.Tint;

            DropShadowOffset = original.DropShadowOffset;
            DropShadowColour = original.DropShadowColour;
        }
        #endregion

        #region Copy Functions
        public IStyleAttribute CreateCopy() => new Content(this);
        #endregion

        #region Combination Functions
        public void OverrideBaseAttribute(IStyleAttribute baseAttribute)
        {
#if DEBUG
            // Validity checks.
            if (baseAttribute == null) throw new ArgumentNullException(nameof(baseAttribute));
            if (baseAttribute.Name != Name) throw new Exception($"Content name mismatch; {baseAttribute.Name}, {Name}");
#endif

            // Ensure the attribute is a content.
            if (!(baseAttribute is Content baseContent)) throw new ArgumentException($"Cannot combine with attribute as it is not a content. {baseAttribute}");

            // Override the colour.
            if (Colour == null) Colour = baseContent.Colour;
        }
        #endregion
    }
}
