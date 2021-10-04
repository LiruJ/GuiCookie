using GuiCookie.DataStructures;
using LiruGameHelperMonoGame.Helpers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
{
    public class AspectRatioFitter : Component
    {
        #region Constants
        private const float allowedDeviance = 2.5f;
        private const string modeAttributeName = "RatioMode";
        private const string valueAttributeName = "Ratio";
        #endregion

        #region Fields
        private float? ratio = null;
        #endregion

        #region Properties
        public AspectRatioMode Mode { get; set; } = AspectRatioMode.None;

        public float Ratio
        {
            get => ratio ?? 1;
            set
            {
                ratio = value;
                ValidateSizeChanged();
            }
        }
        #endregion

        #region Constructors

        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Get the aspect ratio properties.
            Mode = Element.Attributes.GetEnumAttributeOrDefault(modeAttributeName, AspectRatioMode.None);
            ratio = Element.Attributes.GetAttributeOrDefault(valueAttributeName, (float?)null);
        }

        public override void OnSetup()
        {
            // Set the ratio to the supplied attribute, or if none exists, it's calculated from the bound size.
            if (!ratio.HasValue) ratio = Element.Attributes.GetAttributeOrDefault(valueAttributeName, (float)Bounds.TotalSize.X / Bounds.TotalSize.Y);
        }

        public override void OnPostSetup()
        {
            if (ratio.HasValue) ValidateSizeChanged();
        }
        #endregion

        #region Size Functions
        /// <summary> Checks the current size against the calculated size. If the sizes roughly match, returns true, otherwise false. If false is returned <paramref name="newSize"/> is filled with the desired size of the element. </summary>
        /// <param name="currentSize"></param>
        /// <param name="newSize"></param>
        /// <returns></returns>
        private bool validateSize(Point currentSize, out Point newSize)
        {
            newSize = currentSize;

            return Mode switch
            {
                AspectRatioMode.WidthControlsHeight => !AspectRatioHelpers.AdjustHeight(ref newSize, Ratio, allowedDeviance),
                AspectRatioMode.HeightControlsWidth => !AspectRatioHelpers.AdjustWidth(ref newSize, Ratio, allowedDeviance),
                AspectRatioMode.EnvelopeParent => !AspectRatioHelpers.FitAroundBounds(ref newSize, Bounds.Parent.TotalSize, Ratio, allowedDeviance),
                AspectRatioMode.FitInParent => !AspectRatioHelpers.FitInBounds(ref newSize, Bounds.Parent.TotalSize, Ratio, allowedDeviance),
                _ => true,
            };
        }

        public override bool ValidateSizeChanged()
        {
            if (!ratio.HasValue) return true;

            if (!validateSize(Bounds.TotalSize, out Point newSize))
            {
                Bounds.TotalSize = newSize;
                return false;
            }
            else return true;
        }
        #endregion
    }
}
