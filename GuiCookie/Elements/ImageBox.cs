using GuiCookie.Components;
using GuiCookie.DataStructures;
using GuiCookie.Styles;
using Microsoft.Xna.Framework.Graphics;

namespace GuiCookie.Elements
{
    public class ImageBox : Element, IImageable
    {
        #region Properties
        public ImageBlock ImageBlock { get; private set; }

        public ClippingMode ClippingMode { get => ImageBlock.ClippingMode; set => ImageBlock.ClippingMode = value; }

        public Texture2D Texture { get => ImageBlock.Texture; set => ImageBlock.Texture = value; }

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
