using GuiCookie.Core.Input;
using GuiCookie.MonoGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GuiCookie.MonoGame.Input
{
    public class MonoGameInputManager : InputManager
    {
        #region Properties
        public Point CurrentCursorPosition => CurrentMouseState.Position.ToPoint();

        public override MonoGameKeyboardState CurrentKeyboardState { get; }

        public override MonoGameKeyboardState PreviousKeyboardState { get; }

        public override MonoGameMouseState CurrentMouseState { get; }

        public override MonoGameMouseState PreviousMouseState { get; }
        #endregion

        #region Constructors
        public MonoGameInputManager(GameWindow gameWindow) : base(new MonoGameKeyboardState(), new MonoGameKeyboardState(), new MonoGameMouseState(), new MonoGameMouseState())
        {
            gameWindow.TextInput += (object sender, TextInputEventArgs e) => TextInput.Append(e.Character);
        }
        #endregion

        #region Update Functions
        public override void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            base.Update(elapsedTime, totalTime);
            
            PreviousKeyboardState.KeyboardState = CurrentKeyboardState.KeyboardState;
            CurrentKeyboardState.KeyboardState = Keyboard.GetState();

            PreviousMouseState.MouseState = CurrentMouseState.MouseState;
            CurrentMouseState.MouseState = Mouse.GetState();
        }
        #endregion
    }
}
