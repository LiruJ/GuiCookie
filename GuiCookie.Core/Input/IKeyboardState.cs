namespace GuiCookie.Core.Input
{
    public interface IKeyboardState
    {
        #region Key Functions
        /// <summary> Gets a value that is <c>true</c> when the given <paramref name="key"/> is pressed on the current frame; otherwise, <c>false</c>. </summary>
        /// <param name="key"> The <see cref="GUIKey"/> to check. </param>
        bool IsKeyDown(GUIKey key);

        /// <summary> Gets a value that is <c>true</c> when the given <paramref name="key"/> is unpressed on the current frame; otherwise, <c>false</c>. </summary>
        /// <param name="key"> The <see cref="GUIKey"/> to check. </param>
        bool IsKeyUp(GUIKey key);
        #endregion
    }
}
