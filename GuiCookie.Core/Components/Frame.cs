using GuiCookie.Core.Data;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.DataStructures;

namespace GuiCookie.Core.Components
{
    public class Frame(ResourceManager resourceManager) : StyledComponent
    {
        #region Constants
        private const string frameImageAttributeName = "FrameImage";
        #endregion

        #region Properties
        public Image? FrameImage { get; set; } = null;

        public DropShadow? DropShadow { get; set; } = null;

        public TintedColour? TintedColour { get; set; } = null;
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

            TintedColour tintedColour = new(resourceManager, attributes);
            if (tintedColour.HasData)
                TintedColour = tintedColour;
            DropShadow dropShadow = new(resourceManager, attributes);
            if (dropShadow.HasData)
                DropShadow = dropShadow;
        }
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            StyleStateMachine?.CurrentSliceAttribute?.Draw(guiCamera, Bounds.AbsoluteTotalArea, FrameImage, TintedColour, DropShadow);
        }
        #endregion
    }
}
