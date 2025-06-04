using GuiCookie.Core.Data;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.Attributes;
using GuiCookie.Core.Styles.DataStructures;
using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Components
{
    public class Frame(ResourceManager resourceManager, StyleManager styleManager) : StyledComponent
    {
        #region Constants
        private const string frameImageAttributeName = "FrameImage";
        #endregion

        #region Dependencies
        private readonly StyleManager styleManager = styleManager ?? throw new ArgumentNullException(nameof(styleManager));
        #endregion

        #region Fields
        private readonly Dictionary<StyleVariant, Image> texturesByStyleVariant = [];

        private readonly StyleAttributeCache<SliceFrameStyleAttribute> sliceCache = new();
        #endregion

        #region Properties
        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Image? FrameImage
        {
            get => sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Image : null;
            set { if (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame)) sliceFrame!.Image = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public DropShadow DropShadow
        {
            get => sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.DropShadow : new DropShadow((Vector2?)null, null);
            set { if (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame)) sliceFrame!.DropShadow = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Colour
        {
            get => sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Colour : null;
            set { if (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame)) sliceFrame!.Colour = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Tint
        {
            get => sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Tint : null;
            set { if (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame)) sliceFrame!.Tint = value; }
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Initialise the styled component first, so the style state machine can be set up.
            base.OnCreated(attributes);

            // Set the frame image.
            if (attributes.TryGetAttribute(frameImageAttributeName, out string? frameImageName))
                FrameImage = resourceManager.ImagesByName.TryGetValue(frameImageName!, out Image? image) ?
                    image : throw new Exception($"Image with name {frameImageName} has not been loaded.");

            // Set the drop shadow.
            DropShadow = DropShadow.CreateCombination(DropShadow, new DropShadow(resourceManager, attributes));

            // Set the colour and tint.
            if (sliceCache.TryGetVariantAttribute(StyleStateMachine?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame))
                sliceFrame!.TintedColour = TintedColour.CreateCombination(sliceFrame.TintedColour, new TintedColour(resourceManager, attributes));
        }
        #endregion

        #region Texture Functions
        public override void OnStyleChanged(Style? style) => sliceCache.Refresh(style);

        private Image? getCurrentTexture()
        {
            // If there is no current variant, return null.
            if (CurrentStyleVariant == null) 
                return null;

            // If the bounds are not enough to draw a texture, return null.
            if (Bounds.TotalSize.X <= 0 || Bounds.TotalSize.Y <= 0) 
                return null;

            // Try to get the texture from the dictionary, if it does not exist then create it.
            if (!texturesByStyleVariant.TryGetValue(CurrentStyleVariant, out Image? texture))
            {
                // Try to get the current SliceFrame. If none was found then return null.
                if (!sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame)) 
                    return null;

                // Before going through the effort of creating an entirely new texture, first check to see if there's any identical textures that could be reused.
                // For example, most of the time a hover variant is the same image but with a different tint, hence the same texture could be reused and it will be recoloured later.
                foreach (StyleVariant variant in Style.StyleVariantsByName.Values)
                {
                    if (texturesByStyleVariant.TryGetValue(variant, out Image? variantTexture) && sliceCache.TryGetVariantAttribute(variant, out SliceFrameStyleAttribute? variantFrame)
                        && variantFrame.Image == sliceFrame!.Image && variantFrame!.NineSlice == sliceFrame!.NineSlice)
                    {
                        // Add this texture to the dictionary again but with the current variant as a key.
                        texturesByStyleVariant.Add(CurrentStyleVariant, variantTexture);

                        // Return the texture.
                        return variantTexture;
                    }
                }

                // Create and add the texture.
                texturesByStyleVariant.Add(CurrentStyleVariant, texture);
            }

            // Return the texture.
            return texture;
        }
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            // If there is no defined SliceFrame for this style, don't draw.
            if (!sliceCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrameStyleAttribute? sliceFrame)) return;


            //// If a shadow is to be drawn, do that first.
            //if (sliceFrame.DropShadow.HasData)
            //    NineSliceDrawer.DrawFrameOnDemand(sliceFrame, new Rectangle(Bounds.AbsoluteTotalPosition + sliceFrame.DropShadow.Offset.Value.ToPoint(), Bounds.TotalSize), guiCamera, sliceFrame.DropShadow.Colour.Value);

            //// Draw the frame.
            //NineSliceDrawer.DrawFrameOnDemand(sliceFrame, Bounds.AbsoluteTotalArea, guiCamera, sliceFrame.MixedColour);
        
        }
        #endregion
    }
}
