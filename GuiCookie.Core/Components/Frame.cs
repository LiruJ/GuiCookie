using GuiCookie.Core.Data;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.Attributes;
using GuiCookie.Core.Styles.DataStructures;
using System.Drawing;

namespace GuiCookie.Core.Components
{
    public class Frame(ResourceManager resourceManager) : StyledComponent
    {
        #region Constants
        private const string frameImageAttributeName = "FrameImage";
        #endregion

        #region Fields
        private readonly StyleAttributeCache<SliceFrameStyleAttribute> sliceCache = new();
        #endregion

        #region Backing Fields
        private Image? frameImage = null;

        private DropShadow? dropShadow = null;

        private Color? colour = null;

        private Color? tint = null;
        #endregion

        #region Properties
        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Image? FrameImage
        {
            get => frameImage ?? (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Image : null);
            set => frameImage = value;
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public DropShadow? DropShadow
        {
            get => dropShadow ?? (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.DropShadow : null);
            set => dropShadow = value;
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Colour
        {
            get => colour ?? (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Colour : null);
            set => colour = value;
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Tint
        {
            get => tint ?? (sliceCache.TryGetVariantAttribute(Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Tint : null);
            set => tint = value;
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

            Colour = resourceManager.GetColourOrDefault(attributes, "Colour");
            Tint = resourceManager.GetColourOrDefault(attributes, "Tint");
            DropShadow dropShadow = new(resourceManager, attributes);
            if (dropShadow.HasData)
                DropShadow = dropShadow;
        }
        #endregion

        #region Texture Functions
        public override void OnStyleChanged(Style? style) => sliceCache.Refresh(style);
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            // If there is no defined SliceFrame for this style, don't draw.
            if (!sliceCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrameStyleAttribute? sliceFrame))
                return;

            guiCamera.DrawNineSlice(FrameImage, Bounds.AbsoluteTotalArea, FrameImage.Source, sliceFrame.MixedColour, sliceFrame.NineSlice.Value.ToVector4());
            //// If a shadow is to be drawn, do that first.
            //if (sliceFrame.DropShadow.HasData)
            //    NineSliceDrawer.DrawFrameOnDemand(sliceFrame, new Rectangle(Bounds.AbsoluteTotalPosition + sliceFrame.DropShadow.Offset.Value.ToPoint(), Bounds.TotalSize), guiCamera, sliceFrame.DropShadow.Colour.Value);

            //// Draw the frame.
            //NineSliceDrawer.DrawFrameOnDemand(sliceFrame, Bounds.AbsoluteTotalArea, guiCamera, sliceFrame.MixedColour);
        
        }
        #endregion
    }
}
