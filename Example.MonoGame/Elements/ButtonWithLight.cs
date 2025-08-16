using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using System;

namespace Example.MonoGame.Elements
{
    public class ButtonWithLight : Element
    {
        #region Elements
        public Button Button { get; set; }

        public IndicatorLight Light { get; set; }
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup(IReadOnlyAttributeCollection attributes)
        {
            base.OnFullSetup(attributes);

            Button = GetChild<Button>() ?? throw new InvalidOperationException("Missing button child");
            Light = GetChild<IndicatorLight>() ?? throw new InvalidOperationException("Missing light child");

            if (attributes.TryGetAttribute(TextBlock.TextAttributeName, out string? buttonText) && Button is TextButton textButton)
                textButton.Text = buttonText;
            if (attributes.TryGetAttribute(MouseHandler.ClickTypeAttributeName, out ClickType clickType, Enum.TryParse))
                Button.MouseHandler.ClickType = clickType;

            Button.LeftClicked.Connect(Light.Interact);
        }
        #endregion
    }
}
