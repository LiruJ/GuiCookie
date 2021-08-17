using GuiCookie.DataStructures;
using GuiCookie.Elements;
using System;

namespace Example.Elements
{
    public class CounterSlider : LabelledSlider
    {
        #region Elements
        /// <summary> The counter that changes the minimum value. </summary>
        public NumberCounter MinimumCounter { get; private set; }

        /// <summary> The counter that changes the maximum value. </summary>
        public NumberCounter MaximumCounter { get; private set; }
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup()
        {
            // Initialise the labelled slider.
            base.OnFullSetup();

            // Set the counters.
            MinimumCounter = GetChildByName<NumberCounter>("MinimumCounter");
            MaximumCounter = GetChildByName<NumberCounter>("MaximumCounter");

            // Initialise the counter values.
            MinimumCounter.Value = (int)MathF.Floor(Slider.MinimumValue);
            MaximumCounter.Value = (int)MathF.Floor(Slider.MaximumValue);

            // Bind the counters to change the min/max of the slider.
            MinimumCounter.OnValueChanged.Connect(() => { Slider.MinimumValue = MinimumCounter.Value; MinimumCounter.Value = (int)MathF.Floor(Slider.MinimumValue); resizeHandle(); });
            MaximumCounter.OnValueChanged.Connect(() => { Slider.MaximumValue = MaximumCounter.Value; MaximumCounter.Value = (int)MathF.Floor(Slider.MaximumValue); resizeHandle(); });
        }

        public override void OnPostFullSetup() => resizeHandle();
        #endregion

        #region Calculation Functions
        private void resizeHandle()
        {
            // If the minimum is equal to the maximum, do nothing.
            if (Slider.MinimumValue == Slider.MaximumValue) return;

            // Resize and reposition the handle.
            Slider.Handle.Bounds.ScaledSize = new Space(1f / ((Slider.MaximumValue - Slider.MinimumValue) + 1), 1f, Axes.Both);
            Slider.Handle.CalculateSliderPosition();
        }
        #endregion
    }
}
