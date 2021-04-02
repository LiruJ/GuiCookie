using GuiCookie.Attributes;
using GuiCookie.DataStructures;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Styles
{
    public class SliceFrame : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";
        private const string imageAttributeName = "Image";

        public const string ShadowColourAttributeName = "ShadowColour";
        public const string ShadowOffsetAttributeName = "ShadowOffset";

        public const string TintAttributeName = "Tint";
        #endregion

        #region Properties
        public string Name { get; }

        /// <summary> Gets a value representing the name of the image atlas that this <see cref="SliceFrame"/> sources from. </summary>
        public Image? Image { get; set; }

        public NineSlice? NineSlice { get; set; }

        /// <summary> Gets a value representing the <see cref="Color"/> of the main tint, or <see cref="SliceFrame"/> <see cref="Color"/> if no texture is supplied. </summary>
        public Color? Colour { get; set; }

        public Color? Tint { get; set; }

        public Color FinalColour => Colour.HasValue && Tint.HasValue 
            ? new Color(Colour.Value.ToVector4() * Tint.Value.ToVector4()) 
            : Colour ?? Tint ?? Color.White;

        /// <summary> True if any texture created from this SliceFrame should be cached to a separate texture, false if it should be drawn on-demand. </summary>
        public bool? CacheTexture { get; set; }

        public Vector2? DropShadowOffset { get; set; }

        public Color DropShadowColour { get; set; }
        #endregion

        #region Constructors
        public SliceFrame(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Set the colour, slice, and cache texture.
            Colour = resourceManager.GetColourOrDefault(attributes, ResourceManager.ColourAttributeName, null);
            Tint = resourceManager.GetColourOrDefault(attributes, TintAttributeName, null);
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
            DropShadowOffset = attributes.GetAttributeOrDefault(ShadowOffsetAttributeName, (Vector2?)null, ToVector2.TryParse);
            DropShadowColour = attributes.GetAttributeOrDefault(ShadowColourAttributeName, Color.Black);
        }

        private SliceFrame(SliceFrame original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            Image = original.Image;
            NineSlice = original.NineSlice;
            CacheTexture = original.CacheTexture;
            Colour = original.Colour;
            Tint = original.Tint;

            DropShadowOffset = original.DropShadowOffset;
            DropShadowColour = original.DropShadowColour;
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