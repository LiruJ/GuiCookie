using GuiCookie.Components;

namespace GuiCookie.Elements
{
    public class TextButton : Button, ITextable
    {
        #region Components
        /// <summary> The text block that holds the actual text. </summary>
        private TextBlock textBlock;
        #endregion

        #region Properties
        /// <summary> The text of the button. </summary>
        public string Text { get => textBlock.Text; set => textBlock.Text = value; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Initialise the base first.
            base.OnCreated();

            // Set components.
            textBlock = GetComponent<TextBlock>();
        }
        #endregion
    }
}
