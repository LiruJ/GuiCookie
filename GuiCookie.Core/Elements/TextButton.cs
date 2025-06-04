using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using System.Drawing;

namespace GuiCookie.Core.Elements
{
    /// <summary> Represents a button with a <see cref="TextBlock"/> component. </summary>
    public class TextButton : Button, ITextable
    {
        #region Properties
        /// <summary> The text block that holds the actual text. </summary>
        public TextBlock TextBlock { get; private set; }

        /// <summary> The text of the button. </summary>
        public string Text { get => TextBlock.Text; set => TextBlock.Text = value; }

        /// <summary> The colour of the button's text. </summary>
        public Color? Colour { get => TextBlock.Colour; set => TextBlock.Colour = value; }
        #endregion

        #region Initialisation Functions
        /// <summary> Calls <see cref="Button.OnCreated"/> then sets the <see cref="TextBlock"/> property. </summary>
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Initialise the base first.
            base.OnCreated(attributes);

            // Set components.
            TextBlock = GetComponent<TextBlock>();
        }
        #endregion
    }
}