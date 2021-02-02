using GuiCookie.Components;

namespace GuiCookie.Elements
{
    public class TextBox : Element, ITextable
    {
        #region Components
        public TextBlock TextBlock { get; private set; }
        #endregion

        #region Properties
        /// <summary> The text of the box. </summary>
        public string Text { get => TextBlock.Text; set => TextBlock.Text = value; }
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
