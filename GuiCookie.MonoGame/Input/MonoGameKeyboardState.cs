using GuiCookie.Core.Input;
using Microsoft.Xna.Framework.Input;

namespace GuiCookie.MonoGame.Input
{
    public class MonoGameKeyboardState : IKeyboardState
    {
        #region Properties
        public KeyboardState KeyboardState { get; set; }
        #endregion

        #region Key Functions
        public bool IsKeyDown(GUIKey key) => KeyboardState.IsKeyDown(key);
        public bool IsKeyDown(Keys key) => KeyboardState.IsKeyDown(key);

        public bool IsKeyUp(GUIKey key) => KeyboardState.IsKeyUp(key);
        public bool IsKeyUp(Keys key) => KeyboardState.IsKeyUp(key);
        #endregion
    }
}
