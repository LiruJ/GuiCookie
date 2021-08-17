using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
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
        #endregion

        #region Fields
        protected readonly StyleAttributeCache<SliceFrame> fillFrameCache = new StyleAttributeCache<SliceFrame>("Fill");
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
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Set the amount of digits to round to.
            decimalDigits = Attributes.GetAttributeOrDefault(decimalDigitsAttributeName, 10);
            layoutDirection = Attributes.GetEnumAttributeOrDefault(directionAttributeName, Direction.Horizontal);

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
            //Value = Value;
        }
        #endregion

        #region Style Functions
        public override void OnStyleChanged() => fillFrameCache.Refresh(Style);
        #endregion

        #region Draw Functions
        protected override void Draw(IGuiCamera guiCamera)
        {
            // Draw the components.
            base.Draw(guiCamera);

            // Draw the fill.
            drawFill(guiCamera);
        }

        protected virtual void drawFill(IGuiCamera guiCamera)
        {
            // Do nothing if there is no fill.
            if (!fillFrameCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrame fill)) return;
            
            // Calculate the size of the fill.
            Point fillSize = LayoutDirection == Direction.Horizontal
                ? new Point((int)MathF.Floor(Bounds.ContentSize.X * NormalisedValue), Bounds.ContentSize.Y)
                : new Point(Bounds.ContentSize.X, (int)MathF.Floor(Bounds.ContentSize.Y * NormalisedValue));

            // Calculate the destination of the fill.
            Rectangle fillDestination = new Rectangle(Bounds.AbsoluteContentPosition, fillSize);

            // Draw the fill.
            NineSliceDrawer.DrawFrameOnDemand(fill, fillDestination, guiCamera, fill.FinalColour);
        }
        #endregion
    }
}