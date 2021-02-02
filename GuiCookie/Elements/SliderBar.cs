using GuiCookie.Components;
using GuiCookie.DataStructures;
using LiruGameHelper.Signals;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Elements
{
    public class SliderBar : Element
    {
        #region Constants
        private const string directionAttributeName = "Direction";
        private const string minimumValueAttributeName = "MinimumValue";
        private const string maximumValueAttributeName = "MaximumValue";
        private const string valueAttributeName = "Value";
        private const string decimalDigitsAttributeName = "DecimalDigits";
        #endregion

        #region Components
        private MouseHandler mouseHandler;
        #endregion

        #region Fields
        private SliderHandle sliderHandle;

        private float value;
        #endregion

        #region Backing Fields
        private Direction layoutDirection = Direction.Horizontal;
        #endregion

        #region Properties
        public int DecimalDigits { get; set; }

        public Direction LayoutDirection
        {
            get => layoutDirection;
            set
            {
                // Ensure validity.
                if (value == Direction.None) throw new ArgumentException("Cannot set direction of slider bar to none!");

                layoutDirection = value;
            }
        }

        public float MinimumValue { get; set; }

        public float MaximumValue { get; set; }

        public float Value
        {
            get => value;
            set
            {
                // Set the value.
                float oldValue = this.value;
                this.value = (float)Math.Round(MathHelper.Clamp(value, MinimumValue, MaximumValue), DecimalDigits);

                // If the value changed, invoke the signal and update the slider knob.
                if (oldValue != value)
                {
                    sliderHandle.CalculateSliderPosition();
                    onValueChanged.Invoke();
                }
            }
        }

        public float NormalisedValue
        {
            get => (Value - MinimumValue) / (MaximumValue - MinimumValue);
            set => Value = MinimumValue + (MathHelper.Clamp(value, 0, 1) * (MaximumValue - MinimumValue));
        }
        #endregion

        #region Signals
        public IConnectableSignal OnValueChanged => onValueChanged;
        private readonly Signal onValueChanged = new Signal();
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Set the amount of digits to round to.
            DecimalDigits = Attributes.GetAttributeOrDefault(decimalDigitsAttributeName, 10);
            LayoutDirection = Attributes.GetEnumAttributeOrDefault(directionAttributeName, Direction.Horizontal);

            // Set the minimum and maximum values, throw an error if they're invalid.
            MinimumValue = Attributes.GetAttributeOrDefault(minimumValueAttributeName, 0.0f);
            MaximumValue = Attributes.GetAttributeOrDefault(maximumValueAttributeName, 1.0f);
            if (MinimumValue >= MaximumValue) throw new Exception("Minimum value must be lower than maximum value.");
        }

        public override void OnFullSetup()
        {
            mouseHandler = GetComponent<MouseHandler>();

            // Get the handle element, throw an exception if it does not exist.
            sliderHandle = GetChild<SliderHandle>() is SliderHandle handle ? handle : throw new Exception("Slider bar is missing slider handle child.");

            // Set the value.
            Value = Attributes.GetAttributeOrDefault(valueAttributeName, 0.5f);
        }
        #endregion

        #region Bind Functions
        public void ConnectValueChanged(Action action) => OnValueChanged.Connect(action);
        #endregion

        #region Resize Functions
        protected override void OnSizeChanged()
        {
            sliderHandle?.CalculateSliderPosition();
        }
        #endregion

        #region Update Functions
        protected override void Update(GameTime gameTime)
        {
            if (mouseHandler.IsClickDragged)
            {
                // Calculate the useable size of the bar.
                float size = LayoutDirection == Direction.Horizontal ? Bounds.TotalSize.X - sliderHandle.Bounds.TotalSize.X : Bounds.TotalSize.Y - sliderHandle.Bounds.TotalSize.Y;

                // Calculate the position of the mouse within the usable area of the bar.
                float mousePos = LayoutDirection == Direction.Horizontal ?
                    MathHelper.Clamp(mouseHandler.RelativeMousePosition.X, sliderHandle.Bounds.TotalSize.X / 2.0f, Bounds.TotalSize.X - (sliderHandle.Bounds.TotalSize.X / 2.0f)) - (sliderHandle.Bounds.TotalSize.X / 2.0f) :
                    MathHelper.Clamp(mouseHandler.RelativeMousePosition.Y, sliderHandle.Bounds.TotalSize.Y / 2.0f, Bounds.TotalSize.Y - (sliderHandle.Bounds.TotalSize.Y / 2.0f)) - (sliderHandle.Bounds.TotalSize.Y / 2.0f);

                // Calculate the value based on what part of the bar is being clicked.
                NormalisedValue = (mousePos / size);
            }
        }
        #endregion
    }
}
