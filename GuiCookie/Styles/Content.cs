using GuiCookie.Attributes;
using GuiCookie.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Styles
{
    public class Content : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";
        #endregion        

        #region Properties
        public string Name { get; }

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

        /// <summary> The drop shadow data used to drop a shadow behind the content. </summary>
        public DropShadow DropShadow { get; set; }
        #endregion

        #region Constructors
        public Content(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Parse the colour.
            TintedColour = new TintedColour(resourceManager, attributes);

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);

            // Set the drop shadow.
            DropShadow = new DropShadow(resourceManager, attributes);
        }

        private Content(Content original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;

            TintedColour = original.TintedColour;

            DropShadow = original.DropShadow;
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

            // Override the properties.
            TintedColour = TintedColour.CreateCombination(baseContent.TintedColour, TintedColour);
            DropShadow = DropShadow.CreateCombination(baseContent.DropShadow, DropShadow);
        }
        #endregion
    }
}
