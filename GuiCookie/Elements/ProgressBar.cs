using GuiCookie.Components;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Elements
{
    public class ProgressBar : Element
    {

        #region Components
        private ImageBlock imageBlock;
        #endregion

        #region Fields
        private float progress;
        #endregion

        #region Properties
        public float Progress
        {
            get => progress;
            set
            {
                progress = Math.Min(1, Math.Max(0, value));
                OnSizeChanged();
            }
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            imageBlock = GetComponent<ImageBlock>();

            imageBlock.ClippingMode = DataStructures.ClippingMode.Clip;
        }
        #endregion

        #region Progress Functions
        protected override void OnSizeChanged()
        {
            if (imageBlock != null) imageBlock.Size = new Vector2(Bounds.ContentSize.X * progress, Bounds.ContentSize.Y);
        }
        #endregion
    }
}
