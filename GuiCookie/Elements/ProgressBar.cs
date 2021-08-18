using GuiCookie.Attributes;
using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Elements
{
    public class ProgressBar : Element
    {
        #region Constants
        private const string directionAttributeName = "Direction";

        private const string minimumValueAttributeName = "MinimumValue";

        private const string maximumValueAttributeName = "MaximumValue";

        private const string valueAttributeName = "Value";

        private const string decimalDigitsAttributeName = "DecimalDigits";

        private const string fillPaddingAttributeName = "FillPadding";

        private const string drawBehindAttributeName = "DrawBehind";

        private const string fillName = "Fill";
        #endregion

        #region Fields
        protected readonly StyleAttributeCache<SliceFrame> fillCache = new StyleAttributeCache<SliceFrame>(fillName);
        #endregion

        #region Backing Fields
        private int decimalDigits;

        private Direction layoutDirection = Direction.Horizontal;

        private float minimumValue;

        private float maximumValue;

        protected float value;
        #endregion

        #region Properties
        /// <summary> The number of decimal digits used when rounding the value. </summary>
        public int DecimalDigits
        {
            get => decimalDigits;
            set
            {
                // If the value is negative, do nothing.
                if (value < 0) return;

                // Set the decimal digits.
                decimalDigits = value;

                // Recalculate the value.
                recalculateValue(Value);
            }
        }

        /// <summary> The direction of the progress bar. </summary>
        public Direction LayoutDirection
        {
            get => layoutDirection;
            set
            {
                // Ensure validity.
                if (value == Direction.None) throw new ArgumentException("Cannot set direction of progress bar to none!");

                // Set the layout direction.
                layoutDirection = value;
            }
        }

        /// <summary> The lowest value this progress bar can be. </summary>
        /// <remarks> If setting both the <see cref="MinimumValue"/> and <see cref="MaximumValue"/> at the same time, use <see cref="SetMinimumAndMaximum(float, float)"/> instead to avoid clamping errors. </remarks>
        public float MinimumValue
        {
            get => minimumValue;
            set
            {
                // If there is no change, do nothing.
                if (minimumValue == value) return;

                // If the given value is greater than the maximum then do nothing.
                if (value > MaximumValue) return;

                // Set the minimum value.
                minimumValue = value;

                // Recalculate the value.
                recalculateValue(Value);
            }
        }

        /// <summary> The highest value this progress bar can be. </summary>
        /// <remarks> If setting both the <see cref="MinimumValue"/> and <see cref="MaximumValue"/> at the same time, use <see cref="SetMinimumAndMaximum(float, float)"/> instead to avoid clamping errors. </remarks>
        public float MaximumValue
        {
            get => maximumValue;
            set
            {
                // If there is no change, do nothing.
                if (maximumValue == value) return;

                // If the given value is less than the minimum then do nothing.
                if (value < MinimumValue) return;

                // Set the maximum value.
                maximumValue = value;

                // Recalculate the value.
                recalculateValue(Value);
            }
        }

        /// <summary> Gets or sets the value of the progress bar. </summary>
        public float Value
        {
            get => value;
            set => recalculateValue(value);
        }

        /// <summary> Gets or sets the <see cref="Value"/> of this progress bar with a normalised value from <c>0</c> to <c>1</c>. </summary>
        public float NormalisedValue
        {
            get => (Value - MinimumValue) / (MaximumValue - MinimumValue);
            set => Value = MinimumValue + (MathHelper.Clamp(value, 0, 1) * (MaximumValue - MinimumValue));
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name for the fill. </summary>
        public Color? FillColour
        {
            get => fillCache.TryGetVariantAttribute(Style.BaseVariant, out SliceFrame sliceFrame) ? sliceFrame.Colour : null;
            set
            {
                // Try to create the fill attributes if they do not exist already.
                tryCreateFillAttributes();

                // Set the value for all variants.
                foreach (StyleVariant variant in Style.StyleVariantsByName.Values)
                    if (variant.GetNamedAttributeOfType<SliceFrame>(fillName) is SliceFrame fillFrame) fillFrame.Colour = value;
            }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name for the fill. </summary>
        public Color? FillTint
        {
            get => fillCache.TryGetVariantAttribute(Style.BaseVariant, out SliceFrame sliceFrame) ? sliceFrame.Tint : null;
            set 
            {
                // Try to create the fill attributes if they do not exist already.
                tryCreateFillAttributes();

                // Set the value for all variants.
                foreach (StyleVariant variant in Style.StyleVariantsByName.Values)
                    if (variant.GetNamedAttributeOfType<SliceFrame>(fillName) is SliceFrame fillFrame) fillFrame.Tint = value;
            }
        }

        /// <summary> The padding applied to the fill, completely separate of <see cref="Bounds.Padding"/>. </summary>
        public Sides FillPadding { get; set; }

        /// <summary> If this is <c>true</c>, the fill is drawn first; otherwise it is drawn last. </summary>
        public bool DrawFillBehind { get; set; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Set the amount of digits to round to.
            decimalDigits = Attributes.GetAttributeOrDefault(decimalDigitsAttributeName, 10);
            layoutDirection = Attributes.GetEnumAttributeOrDefault(directionAttributeName, Direction.Horizontal);

            // Set the graphical data.
            if (Attributes.HasAttribute(fillName + ResourceManager.ColourAttributeName)) 
                FillColour = Attributes.GetAttributeOrDefault(fillName + ResourceManager.ColourAttributeName, (Color?)null, Colour.TryParse);
            if (Attributes.HasAttribute(fillName + TintedColour.TintAttributeName))
                FillTint = Attributes.GetAttributeOrDefault(fillName + TintedColour.TintAttributeName, (Color?)null, Colour.TryParse);

            FillPadding = Attributes.GetAttributeOrDefault(fillPaddingAttributeName, new Sides(0, SideMask.None));
            DrawFillBehind = Attributes.GetAttributeOrDefault(drawBehindAttributeName, false);

            // Set the minimum and maximum values, throw an error if they're invalid.
            SetMinimumAndMaximum(Attributes.GetAttributeOrDefault(minimumValueAttributeName, 0.0f), Attributes.GetAttributeOrDefault(maximumValueAttributeName, 1.0f));
        }

        public override void OnFullSetup()
        {
            // Set the value.
            Value = Attributes.GetAttributeOrDefault(valueAttributeName, 0f);
        }
        #endregion

        #region Calculation Functions
        private void recalculateValue(float value)
        {
            // Keep track of the old value and set the value.
            float oldValue = Value;
            this.value = (float)Math.Round(MathHelper.Clamp(value, MinimumValue, MaximumValue), DecimalDigits);

            // Call the protected function.
            onValueRecalculated(oldValue);
        }

        /// <summary> Called when the <see cref="Value"/> is recalculated. </summary>
        /// <param name="oldValue"> The value before it was recalculated. This may be the same as the value. </param>
        protected virtual void onValueRecalculated(float oldValue) { }

        /// <summary> Sets both the <see cref="MinimumValue"/> and <see cref="MaximumValue"/> to the values provided, allowing them to both be set at the same time with no clamping. </summary>
        /// <param name="newMinimum"> The new minimum value. </param>
        /// <param name="newMaximum"> The new maximum value. </param>
        public void SetMinimumAndMaximum(float newMinimum, float newMaximum)
        {
            // Ensure validity.
            if (newMinimum > newMaximum) throw new ArgumentException($"Progress bar minimum ({newMinimum}) cannot be greater than maximum ({newMaximum}).");

            // Set the values directly.
            minimumValue = newMinimum;
            maximumValue = newMaximum;

            // Set the value to itself to recalculate.
            recalculateValue(Value);
        }
        #endregion

        #region Style Functions
        public override void OnStyleChanged() => fillCache.Refresh(Style);

        private SliceFrame tryCreateFillAttributes()
        {
            // If the fill frame does not exist, create it.
            if (!fillCache.TryGetVariantAttribute(Style.BaseVariant, out SliceFrame fillFrame))
            {
                // Create an empty slice frame.
                fillFrame = new SliceFrame(Root.StyleManager.ResourceManager, new AttributeCollection() { { "Name", fillName } });

                // Add the fill frame to the base variant of the style. Do the same for the hovered, clicked, and disabled.
                Style.BaseVariant.AddAttribute(fillFrame);
                Style.GetStyleVariantFromName(Style.HoveredVariantName).AddAttribute(fillFrame.CreateCopy());
                Style.GetStyleVariantFromName(Style.ClickedVariantName).AddAttribute(fillFrame.CreateCopy());
                Style.GetStyleVariantFromName(Style.DisabledVariantName).AddAttribute(fillFrame.CreateCopy());

                // Refresh the cache.
                fillCache.Refresh(Style);
            }

            // Return the created/found fill frame.
            return fillFrame;
        }
        #endregion

        #region Draw Functions
        protected override void Draw(IGuiCamera guiCamera)
        {
            // If the fill should be drawn behind, don't draw the components just yet.
            if (!DrawFillBehind)
                base.Draw(guiCamera);

            // Draw the fill.
            drawFill(guiCamera);

            // If the fill should be drawn behind, draw the components after the fill.
            if (DrawFillBehind)
                base.Draw(guiCamera);
        }

        protected virtual void drawFill(IGuiCamera guiCamera)
        {
            // Do nothing if there is no fill.
            if (!fillCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrame fill)) return;

            // Calculate the absolute area for the fill to be drawn.
            Rectangle fillArea = FillPadding.ScaleRectangle(Bounds.AbsoluteTotalArea);

            // Adjust the fill area's width or height to match the progress.
            if (LayoutDirection == Direction.Horizontal) fillArea.Width = (int)MathF.Floor(fillArea.Width * NormalisedValue);
            else fillArea.Height = (int)MathF.Floor(fillArea.Height * NormalisedValue);

            // Draw the fill.
            NineSliceDrawer.DrawFrameOnDemand(fill, fillArea, guiCamera, fill.MixedColour);
        }
        #endregion
    }
}