using GuiCookie.Components;
using Microsoft.Xna.Framework;

namespace GuiCookie.Elements
{
    public class TextButton : Button, ITextable
    {
        #region Properties
        /// <summary> The text block that holds the actual text. </summary>
        public TextBlock TextBlock { get; private set; }

        /// <summary> The text of the button. </summary>
        public string Text { get => TextBlock.Text; set => TextBlock.Text = value; }

        public Color? Colour { get => TextBlock.Colour; set => TextBlock.Colour = value; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Initialise the base first.
            base.OnCreated();

            // Set components.
            TextBlock = GetComponent<TextBlock>();
        }
        #endregion
    }
}
