using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Helpers;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.DataStructures;
using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Components
{
    public class ImageBlock(ResourceManager resourceManager) : StyledComponent
    {
        #region Constants
        private const string imageAttributeName = "Image";
        private const string clippingModeAttributeName = "ClippingMode";
        private const string centredAttributeName = "Centred";
        #endregion

        #region Properties
        public ClippingMode ClippingMode { get; set; }

        public bool Centred { get; set; }

        public Point Offset { get; private set; }

        public Vector2? Size { get; set; }

        public float Scale { get; private set; } = 1;

        public DropShadow? DropShadow { get; set; } = null;

        public TintedColour? TintedColour { get; set; } = null;

        /// <summary> The current image that is being displayed. </summary>
        public Image? Image { get; set; } = null;
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Initialise the styled component first, so the style state machine can be set up.
            base.OnCreated(attributes);

            // Set the attributes.
            string imageName = attributes.GetAttributeOrDefault(imageAttributeName, string.Empty)!;
            if (!string.IsNullOrEmpty(imageName))
                SetImageFromName(imageName);

            TintedColour tintedColour = new(resourceManager, attributes);
            if (tintedColour.HasData)
                TintedColour = tintedColour;
            DropShadow dropShadow = new(resourceManager, attributes);
            if (dropShadow.HasData)
                DropShadow = dropShadow;

            ClippingMode = attributes.GetEnumAttributeOrDefault(clippingModeAttributeName, ClippingMode.Squeeze);
            Centred = attributes.GetAttributeOrDefault(centredAttributeName, false);
        }
        #endregion

        #region Image Functions
        public void SetImageFromName(string name)
            => Image = resourceManager.ImagesByName.TryGetValue(name, out Image? image)
            ? image
            : throw new Exception($"Image with name {name} has not been loaded as an image resource.");
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            // If the current image is empty, don't draw.
            if (Image == null)
                return;

            // Calculate the spatial values based on the clipping mode.
            Size size = Size.HasValue ? Size.Value.ToSize() : (Size)Element.Bounds.ContentSize;
            ClippingModeHelper.CalculateClippingValues(ClippingMode, Bounds.AbsoluteContentPosition, size, Image.Source, Centred, out Rectangle source, out Rectangle destination, out Point offset, out float scale);

            Offset = offset;
            Scale = scale;

            StyleStateMachine?.CurrentContentAttribute?.Draw(guiCamera, destination, source, Image, TintedColour, DropShadow);
        }
        #endregion
    }
}
