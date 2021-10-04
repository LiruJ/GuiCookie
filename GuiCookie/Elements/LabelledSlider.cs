namespace GuiCookie.Elements
{
    /// <summary> Represents a slider and a text label displaying the current value. </summary>
    public class LabelledSlider : Element
    {
        #region Constants
        private const string sliderName = "Slider";

        private const string labelName = "Label";

        private const string prefixAttributeName = "Prefix";

        private const string suffixAttributeName = "Suffix";

        private const string isPercentageAttributeName = "IsPercentage";
        #endregion

        #region Elements
        /// <summary> The <see cref="SliderBar"/>. </summary>
        public SliderBar Slider { get; private set; }

        /// <summary> The text label. </summary>
        public ITextable Label { get; private set; }
        #endregion

        #region Backing Fields
        private string prefix;

        private string suffix;

        private bool isPercentage;
        #endregion

        #region Properties
        /// <summary> The string attached to the start of the label. </summary>
        public string Prefix
        {
            get => prefix;
            set
            {
                // Set the value.
                prefix = value;

                // Recalculate the label.
                calculateLabel();
            }
        }

        /// <summary> The string attached to the end of the label. </summary>
        public string Suffix
        {
            get => suffix;
            set
            {
                // Set the value.
                suffix = value;

                // Recalculate the label.
                calculateLabel();
            }
        }

        /// <summary> If this is <c>true</c>, then the label will use the <see cref="ProgressBar.NormalisedValue"/> property and format the text as a percentage. </summary>
        /// <remarks> Note that the <c>%</c> character is automatically added and is not needed as a suffix. </remarks>
        public bool IsPercentage
        {
            get => isPercentage; 
            set
            {
                // Set the value.
                isPercentage = value;

                // Recalculate the label.
                calculateLabel();
            }
        }
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup()
        {
            // Get the elements.
            Slider = GetChildByName<SliderBar>(sliderName);
            Label = GetInterfacedChildByName<ITextable>(labelName);

            // Set the attributes.
            prefix = Attributes.GetAttributeOrDefault(prefixAttributeName, string.Empty);
            suffix = Attributes.GetAttributeOrDefault(suffixAttributeName, string.Empty);
            isPercentage = Attributes.GetAttributeOrDefault(isPercentageAttributeName, false);

            // Connect to the slider signal.
            Slider?.OnValueChanged.Connect(calculateLabel);

            // Calculate the label.
            calculateLabel();
        }
        #endregion

        #region Label Functions
        private void calculateLabel()
        {
            // If there is no label or slider, do nothing.
            if (Label == null || Slider == null) return;

            // Update the label text.
            Label.Text = IsPercentage
                ? $"{Prefix ?? string.Empty}{Slider.NormalisedValue:P0}{Suffix ?? string.Empty}"
                : $"{Prefix ?? string.Empty}{Slider.Value}{Suffix ?? string.Empty}";
        }
        #endregion
    }
}
