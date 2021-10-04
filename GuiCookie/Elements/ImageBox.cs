using GuiCookie.Components;
using GuiCookie.DataStructures;
using GuiCookie.Styles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GuiCookie.Elements
{
    /// <summary> Represents an element that displays an image. </summary>
    public class ImageBox : Element, IImageable
    {
        #region Properties
        /// <summary> The underlying <see cref="ImageBlock"/>. </summary>
        public ImageBlock ImageBlock { get; private set; }

        /// <summary> The <see cref="ClippingMode"/> of the current image. </summary>
        public ClippingMode ClippingMode { get => ImageBlock.ClippingMode; set => ImageBlock.ClippingMode = value; }

        /// <summary> The texture of the current image. If this is set to a texture, then the image will be of the entire texture. </summary>
        public Texture2D Texture { get => ImageBlock.Texture; set => ImageBlock.Texture = value; }

        /// <summary> The colour of the current image. </summary>
        public Color? Colour { get => ImageBlock.Colour; set => ImageBlock.Colour = value; }

        /// <summary> The current image. </summary>
        public Image Image { get => ImageBlock.Image; set => ImageBlock.Image = value; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Set components.
            ImageBlock = GetComponent<ImageBlock>();
        }
        #endregion

        #region Image Functions
        public void SetImageFromName(string name) => ImageBlock.SetImageFromName(name);
        #endregion
    }
}
