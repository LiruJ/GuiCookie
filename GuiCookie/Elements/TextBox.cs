using GuiCookie.Components;
using Microsoft.Xna.Framework;

namespace GuiCookie.Elements
{
    public class TextBox : Element, ITextable
    {
        #region Properties
        public TextBlock TextBlock { get; private set; }

        /// <summary> The text of the box. </summary>
        public string Text { get => TextBlock.Text; set => TextBlock.Text = value; }

        public Color? Colour { get => TextBlock.Colour; set => TextBlock.Colour = value; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Set components.
            TextBlock = GetComponent<TextBlock>();
        }
        #endregion
    }
}
