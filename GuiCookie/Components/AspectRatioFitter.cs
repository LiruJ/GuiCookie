using GuiCookie.DataStructures;
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

            switch (Mode)
            {
                case AspectRatioMode.WidthControlsHeight:
                    // Calculate the adjusted dimension.
                    float adjustedHeight = currentSize.X / Ratio;

                    // If the dimension is within the correct aspect ratio, do nothing.
                    if (Math.Abs(adjustedHeight - currentSize.Y) <= allowedDeviance) return true;

                    // Change the size.
                    newSize.Y = (int)Math.Ceiling(adjustedHeight);

                    return false;
                case AspectRatioMode.HeightControlsWidth:
                    // Calculate the adjusted dimension.
                    float adjustedWidth = currentSize.Y * Ratio;

                    // If the dimension is within the correct aspect ratio, do nothing.
                    if (Math.Abs(adjustedWidth - currentSize.X) <= allowedDeviance) return true;

                    // Change the size.
                    newSize.X = (int)Math.Ceiling(adjustedWidth);

                    return false;
                case AspectRatioMode.EnvelopeParent:
                case AspectRatioMode.FitInParent:
                    // Create a point to hold the new size.
                    newSize = Bounds.Parent.TotalSize;

                    // Calculate the ratio of the parent.
                    float parentRatio = (float)Bounds.Parent.TotalSize.X / Bounds.Parent.TotalSize.Y;

                    // If the ratio of this element is bigger than that of the parent, use the width.
                    if (Ratio > parentRatio ^ Mode == AspectRatioMode.EnvelopeParent)
                    {
                        newSize.Y = (int)Math.Ceiling(Bounds.Parent.TotalSize.X / Ratio);
                        if (Math.Abs(newSize.Y - currentSize.Y) <= allowedDeviance) return true;
                    }
                    else
                    {
                        newSize.X = (int)Math.Ceiling(Bounds.Parent.TotalSize.Y * Ratio);
                        if (Math.Abs(newSize.X - currentSize.X) <= allowedDeviance) return true;
                    }

                    return false;
                case AspectRatioMode.None:
                default:
                    return true;
            }
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
