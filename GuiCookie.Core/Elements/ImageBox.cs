using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles.DataStructures;

namespace GuiCookie.Core.Elements
{
    /// <summary> Represents an element that displays an image. </summary>
    public class ImageBox : Element, IImageable
    {
        #region Properties
        /// <summary> The underlying <see cref="ImageBlock"/>. </summary>
        public ImageBlock? ImageBlock { get; private set; }

        /// <summary> The <see cref="ClippingMode"/> of the current image. </summary>
        public ClippingMode ClippingMode => ImageBlock?.ClippingMode ?? ClippingMode.None;

        /// <summary> The colour of the current image. </summary>
        public TintedColour? TintedColour => ImageBlock?.TintedColour;

        /// <summary> The current image. </summary>
        public Image? Image
        {
            get => ImageBlock?.Image;
            set
            {
                if (ImageBlock != null)
                    ImageBlock.Image = value;
            }
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Set components.
            ImageBlock = GetComponent<ImageBlock>();
        }
        #endregion

        #region Image Functions
        public void SetImageFromName(string name) => ImageBlock?.SetImageFromName(name);
        #endregion
    }
}
