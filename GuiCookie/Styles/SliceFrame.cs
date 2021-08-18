using GuiCookie.Attributes;
using GuiCookie.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Styles
{
    public class SliceFrame : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";
        private const string imageAttributeName = "Image";
        #endregion

        #region Properties
        /// <summary> The name of this attribute. </summary>
        public string Name { get; }

        /// <summary> The image atlas that this <see cref="SliceFrame"/> sources from. </summary>
        public Image? Image { get; set; }

        /// <summary> The <see cref="DataStructures.NineSlice"/> used to slice the image into sections. </summary>
        public NineSlice? NineSlice { get; set; }

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

        /// <summary> True if any texture created from this SliceFrame should be cached to a separate texture, false if it should be drawn on-demand. </summary>
        public bool? CacheTexture { get; set; }

        /// <summary> The drop shadow data used to drop a shadow behind the element. </summary>
        public DropShadow DropShadow { get; set; }
        #endregion

        #region Constructors
        public SliceFrame(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Set the colour, slice, and cache texture.
            TintedColour = new TintedColour(resourceManager, attributes);
            CacheTexture = attributes.GetAttributeOrDefault("Cached", (bool?)null, bool.TryParse);
            NineSlice = attributes.GetAttributeOrDefault("NineSlice", (NineSlice?)null, DataStructures.NineSlice.TryParse);

            // Get the image.
            string imageName = attributes.GetAttributeOrDefault(imageAttributeName, string.Empty);
            Image = !string.IsNullOrWhiteSpace(imageName) ?
                resourceManager.ImagesByName.TryGetValue(imageName, out Image image) ? image : throw new Exception($"Image resource named \"{imageName}\" does not exist.")
                : (Image?)null;

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);

            // Set the drop shadow.
            DropShadow = new DropShadow(resourceManager, attributes);
        }

        private SliceFrame(SliceFrame original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            Image = original.Image;
            NineSlice = original.NineSlice;
            CacheTexture = original.CacheTexture;
            TintedColour = original.TintedColour;

            DropShadow = original.DropShadow;
        }
        #endregion

        #region Copy Functions
        public IStyleAttribute CreateCopy() => new SliceFrame(this);
        #endregion

        #region Combination Functions
        public void OverrideBaseAttribute(IStyleAttribute baseAttribute)
        {
#if DEBUG
            // Validity checks.
            if (baseAttribute == null) throw new ArgumentNullException(nameof(baseAttribute));
            if (baseAttribute.Name != Name) throw new Exception($"SliceFrame name mismatch; {baseAttribute.Name}, {Name}");
#endif

            // Ensure the attribute is a SliceFrame.
            if (!(baseAttribute is SliceFrame baseSliceFrame)) throw new ArgumentException($"Cannot combine with attribute as it is not a SliceFrame. {baseAttribute}");

            // Override the properties.
            if (Image == null) Image = baseSliceFrame.Image;
            if (NineSlice == null) NineSlice = baseSliceFrame.NineSlice;
            TintedColour = TintedColour.CreateCombination(baseSliceFrame.TintedColour, TintedColour);
            DropShadow = DropShadow.CreateCombination(baseSliceFrame.DropShadow, DropShadow);
            if (CacheTexture == null) CacheTexture = baseSliceFrame.CacheTexture;
        }
        #endregion
    }
}