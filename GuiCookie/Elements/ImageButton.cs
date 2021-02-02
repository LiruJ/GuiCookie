using GuiCookie.Components;
using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GuiCookie.Elements
{
    public class ImageButton : Button, IImageable
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
            // Initialise the base button first.
            base.OnCreated();

            // Set components.
            ImageBlock = GetComponent<ImageBlock>() ?? throw new Exception("Button's image block is missing.");
        }
        #endregion

        #region Image Functions
        public void SetImageFromName(string name) => ImageBlock.SetImageFromName(name);
        #endregion

        #region Draw Functions
        protected override void Draw(IGuiCamera guiCamera)
        {
            // Draw the base button first.
            base.Draw(guiCamera);

            // Draw the image over the button.
            ImageBlock.Draw(guiCamera);
        }
        #endregion
    }
}
