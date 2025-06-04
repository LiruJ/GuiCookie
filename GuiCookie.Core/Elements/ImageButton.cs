using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Rendering;
using System.Drawing;

namespace GuiCookie.Core.Elements
{
    /// <summary> Represents a button with an <see cref="ImageBlock"/> component. </summary>
    public class ImageButton : Button, IImageable
    {
        #region Properties
        /// <summary> The underlying <see cref="ImageBlock"/>. </summary>
        public ImageBlock ImageBlock { get; private set; }

        /// <summary> The <see cref="ClippingMode"/> of the current image. </summary>
        public ClippingMode ClippingMode { get => ImageBlock.ClippingMode; set => ImageBlock.ClippingMode = value; }

        /// <summary> The colour of the current image. </summary>
        public Color? Colour { get => ImageBlock.Colour; set => ImageBlock.Colour = value; }

        /// <summary> The current image. </summary>
        public Image? Image { get => ImageBlock.Image; set => ImageBlock.Image = value; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Initialise the base button first.
            base.OnCreated(attributes);

            // Set components.
            ImageBlock = GetComponent<ImageBlock>();
        }
        #endregion

        #region Image Functions
        public void SetImageFromName(string name) => ImageBlock.SetImageFromName(name);
        #endregion
    }
}
