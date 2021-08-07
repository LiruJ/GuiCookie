using System;
using System.Collections.Generic;
using System.Text;

namespace GuiCookie.Elements
{
    public class LabelledSlider : Element
    {
        #region Constants
        private const string sliderName = "Slider";

        private const string labelName = "Label";

        private const string prefixAttributeName = "Prefix";

        private const string suffixAttributeName = "Suffix";
        #endregion

        #region Elements
        public SliderBar Slider { get; private set; }

        public ITextable Label { get; private set; }

        public string Prefix { get; set; }

        public string Suffix { get; set; }
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup()
        {
            Slider = GetChildByName<SliderBar>(sliderName);
            Label = GetInterfacedChildByName<ITextable>(labelName);

            Prefix = Attributes.GetAttributeOrDefault(prefixAttributeName, string.Empty);
            Suffix = Attributes.GetAttributeOrDefault(suffixAttributeName, string.Empty);

            if (Slider != null)
                Slider.OnValueChanged.Connect(updateLabel);
        }
        #endregion

        #region Label Functions
        private void updateLabel()
        {
            if (Label == null || Slider == null) return;

            Label.Text = $"{Prefix ?? string.Empty}{Slider.Value}{Suffix ?? string.Empty}";
        }
        #endregion
    }
}
