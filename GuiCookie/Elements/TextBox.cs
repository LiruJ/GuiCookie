using GuiCookie.Components;
using Microsoft.Xna.Framework;

namespace GuiCookie.Elements
{
    /// <summary> Represents an element that displays text. </summary>
    public class TextBox : Element, ITextable
    {
        #region Properties
        /// <summary> The underlying <see cref="TextBlock"/>. </summary>
        public TextBlock TextBlock { get; private set; }

        /// <summary> The text of the box. </summary>
        public string Text { get => TextBlock.Text; set => TextBlock.Text = value; }

        /// <summary> The colour of the text. </summary>
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
