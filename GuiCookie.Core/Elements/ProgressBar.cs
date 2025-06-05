using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.Attributes;
using GuiCookie.Core.Styles.DataStructures;
using LiruGameHelper.Parsers;
using System.Drawing;

namespace GuiCookie.Core.Elements
{
    public class ProgressBar(ResourceManager resourceManager) : Element
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
                if (value < 0)
                    return;

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
                if (value == Direction.None)
                    throw new ArgumentException("Cannot set direction of progress bar to none!");

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
                if (minimumValue == value)
                    return;

                // If the given value is greater than the maximum then do nothing.
                if (value > MaximumValue)
                    return;

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
                if (maximumValue == value)
                    return;

                // If the given value is less than the minimum then do nothing.
                if (value < MinimumValue)
                    return;

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
            set => Value = MinimumValue + (Math.Clamp(value, 0, 1) * (MaximumValue - MinimumValue));
        }

        ///// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name for the fill. </summary>
        //public Color? FillColour
        //{
        //    get => fillCache.TryGetVariantAttribute(StyleStateMachine?.Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Colour : null;
        //    set
        //    {
        //        // Try to create the fill attributes if they do not exist already.
        //        tryCreateFillAttributes();

        //        // Set the value for all variants.
        //        //foreach (StyleVariant variant in StyleStateMachine?.Style?.StyleVariantsByName.Values)
        //        //    if (variant.GetNamedAttributeOfType<SliceFrameStyleAttribute>(fillName) is SliceFrameStyleAttribute fillFrame)
        //        //        fillFrame.Colour = value;
        //    }
        //}

        ///// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name for the fill. </summary>
        //public Color? FillTint
        //{
        //    get => fillCache.TryGetVariantAttribute(StyleStateMachine?.Style?.BaseVariant, out SliceFrameStyleAttribute? sliceFrame) ? sliceFrame!.Tint : null;
        //    set
        //    {
        //        // Try to create the fill attributes if they do not exist already.
        //        tryCreateFillAttributes();

        //        // Set the value for all variants.
        //        //foreach (StyleVariant variant in StyleStateMachine?.Style?.StyleVariantsByName.Values)
        //        //    if (variant.GetNamedAttributeOfType<SliceFrameStyleAttribute>(fillName) is SliceFrameStyleAttribute fillFrame)
        //        //        fillFrame.Tint = value;
        //    }
        //}

        ///// <summary> The padding applied to the fill, completely separate of <see cref="Bounds.Padding"/>. </summary>
        //public Sides FillPadding { get; set; }

        ///// <summary> If this is <c>true</c>, the fill is drawn first; otherwise it is drawn last. </summary>
        //public bool DrawFillBehind { get; set; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Set the amount of digits to round to.
            decimalDigits = attributes.GetAttributeOrDefault(decimalDigitsAttributeName, 10);
            layoutDirection = attributes.GetEnumAttributeOrDefault(directionAttributeName, Direction.Horizontal);

            //// Set the graphical data.
            //if (attributes.HasAttribute(fillName + ResourceManager.ColourAttributeName))
            //    FillColour = attributes.GetAttributeOrDefault(fillName + ResourceManager.ColourAttributeName, (Color?)null, Colour.TryParse);
            //if (attributes.HasAttribute(fillName + TintedColour.TintAttributeName))
            //    FillTint = attributes.GetAttributeOrDefault(fillName + TintedColour.TintAttributeName, (Color?)null, Colour.TryParse);

            //FillPadding = attributes.GetAttributeOrDefault(fillPaddingAttributeName, new Sides(0, SideMask.None));
            //DrawFillBehind = attributes.GetAttributeOrDefault(drawBehindAttributeName, false);

            // Set the minimum and maximum values, throw an error if they're invalid.
            SetMinimumAndMaximum(attributes.GetAttributeOrDefault(minimumValueAttributeName, 0.0f), attributes.GetAttributeOrDefault(maximumValueAttributeName, 1.0f));
        }

        public override void OnFullSetup(IReadOnlyAttributeCollection attributes)
        {
            // Set the value.
            Value = attributes.GetAttributeOrDefault(valueAttributeName, 0f);
        }
        #endregion

        #region Calculation Functions
        private void recalculateValue(float value)
        {
            // Keep track of the old value and set the value.
            float oldValue = Value;
            this.value = (float)Math.Round(Math.Clamp(value, MinimumValue, MaximumValue), DecimalDigits);

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
            if (newMinimum > newMaximum)
                throw new ArgumentException($"Progress bar minimum ({newMinimum}) cannot be greater than maximum ({newMaximum}).");

            // Set the values directly.
            minimumValue = newMinimum;
            maximumValue = newMaximum;

            // Set the value to itself to recalculate.
            recalculateValue(Value);
        }
        #endregion

        #region Style Functions
        //public override void OnStyleChanged(Style? style) => fillCache.Refresh(style);

        private SliceFrameStyleAttribute tryCreateFillAttributes()
        {
            //// If the fill frame does not exist, create it.
            //if (!fillCache.TryGetVariantAttribute(StyleStateMachine?.Style?.BaseVariant, out SliceFrameStyleAttribute? fillFrame) && fillFrame != null)
            //{
            //    // Create an empty slice frame.
            //    fillFrame = new SliceFrameStyleAttribute(resourceManager, new AttributeCollection() { { "Name", fillName } });

            //    // Add the fill frame to the base variant of the style. Do the same for the hovered, clicked, and disabled.
            //    StyleStateMachine?.Style?.BaseVariant.AddAttribute(fillFrame);
            //    StyleStateMachine?.Style?.GetStyleVariantFromName(Style.HoveredVariantName).AddAttribute(fillFrame.CreateCopy());
            //    StyleStateMachine?.Style?.GetStyleVariantFromName(Style.ClickedVariantName).AddAttribute(fillFrame.CreateCopy());
            //    StyleStateMachine?.Style?.GetStyleVariantFromName(Style.DisabledVariantName).AddAttribute(fillFrame.CreateCopy());

            //    // Refresh the cache.
            //    fillCache.Refresh(StyleStateMachine?.Style);
            //}

            //// Return the created/found fill frame.
            //return fillFrame;
            return null;
        }
        #endregion

        #region Draw Functions
        protected override void Draw(IGuiCamera guiCamera)
        {
            //// If the fill should be drawn behind, don't draw the components just yet.
            //if (!DrawFillBehind)
            //    base.Draw(guiCamera);

            //// Draw the fill.
            //drawFill(guiCamera);

            //// If the fill should be drawn behind, draw the components after the fill.
            //if (DrawFillBehind)
            //    base.Draw(guiCamera);
        }

        protected virtual void drawFill(IGuiCamera guiCamera)
        {
            //// Do nothing if there is no fill.
            //if (!fillCache.TryGetVariantAttribute(StyleStateMachine?.CurrentStyleVariant, out SliceFrameStyleAttribute? fill))
            //    return;

            //// Calculate the absolute area for the fill to be drawn.
            //Rectangle fillArea = FillPadding.ScaleRectangle(Bounds.AbsoluteTotalArea);

            //// Adjust the fill area's width or height to match the progress.
            //if (LayoutDirection == Direction.Horizontal) fillArea.Width = (int)MathF.Floor(fillArea.Width * NormalisedValue);
            //else fillArea.Height = (int)MathF.Floor(fillArea.Height * NormalisedValue);

            // Draw the fill.
            //NineSliceDrawer.DrawFrameOnDemand(fill, fillArea, guiCamera, fill!.MixedColour);
        }
        #endregion
    }
}