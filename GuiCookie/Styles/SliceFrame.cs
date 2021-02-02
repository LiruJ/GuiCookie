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
        public string Name { get; }

        /// <summary> Gets a value representing the name of the image atlas that this <see cref="SliceFrame"/> sources from. </summary>
        public Image? Image { get; set; }

        public NineSlice? NineSlice { get; set; }

        /// <summary> Gets a value representing the <see cref="Color"/> of the tint, or <see cref="SliceFrame"/> <see cref="Color"/> if no texture is supplied. </summary>
        public Color? Colour { get; set; }

        /// <summary> True if any texture created from this SliceFrame should be cached to a separate texture, false if it should be drawn on-demand. </summary>
        public bool? CacheTexture { get; set; }
        #endregion

        #region Constructors
        public SliceFrame(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Set the colour, slice, and cache texture.
            Colour = resourceManager.GetColourOrDefault(attributes, ResourceManager.ColourAttributeName, null);
            CacheTexture = attributes.GetAttributeOrDefault("Cached", null, (string input, out bool? output) => { output = bool.TryParse(input, out bool value) ? (bool?)value : null; return true; });
            NineSlice = attributes.GetAttributeOrDefault("NineSlice", null, (string input, out NineSlice? output) => { output = DataStructures.NineSlice.TryParse(input, out NineSlice value) ? (NineSlice?)value : null; return true; });

            // Get the image.
            string imageName = attributes.GetAttributeOrDefault(imageAttributeName, string.Empty);
            Image = !string.IsNullOrWhiteSpace(imageName) ?
                resourceManager.ImagesByName.TryGetValue(imageName, out Image image) ? image : throw new Exception($"Image resource named \"{imageName}\" does not exist.")
                : (Image?)null;

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);
        }

        private SliceFrame(SliceFrame original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            Image = original.Image;
            NineSlice = original.NineSlice;
            CacheTexture = original.CacheTexture;
            Colour = original.Colour;
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

            if (Image == null) Image = baseSliceFrame.Image;
            if (NineSlice == null) NineSlice = baseSliceFrame.NineSlice;
            if (Colour == null) Colour = baseSliceFrame.Colour;
            if (CacheTexture == null) CacheTexture = baseSliceFrame.CacheTexture;
        }
        #endregion
    }
}