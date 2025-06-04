using GuiCookie.Core.Input;
using GuiCookie.MonoGame.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GuiCookie.MonoGame.Input
{
    public class MonoGameInputManager : InputManager
    {
        #region Fields
        private readonly MonoGameKeyboardState currentKeyboardState;

        private readonly MonoGameKeyboardState previousKeyboardState;

        private readonly MonoGameMouseState currentMouseState;

        private readonly MonoGameMouseState previousMouseState;
        #endregion

        #region Properties
        public Point CurrentCursorPosition => currentMouseState.Position.ToMonoGamePoint();
        #endregion

        #region Constructors
        public MonoGameInputManager(GameWindow gameWindow) 
            : this(gameWindow, new MonoGameKeyboardState(), new MonoGameKeyboardState(), new MonoGameMouseState(), new MonoGameMouseState())
        {
            
        }

        public MonoGameInputManager(GameWindow gameWindow, MonoGameKeyboardState currentKeyboardState, MonoGameKeyboardState previousKeyboardState, MonoGameMouseState currentMouseState, MonoGameMouseState previousMouseState)
            : base(currentKeyboardState, previousKeyboardState, currentMouseState, previousMouseState)
        {
            gameWindow.TextInput += (object? sender, TextInputEventArgs e) => TextInput.Append(e.Character);

            this.currentKeyboardState = currentKeyboardState;
            this.previousKeyboardState = previousKeyboardState;
            this.currentMouseState = currentMouseState;
            this.previousMouseState = previousMouseState;
        }
        #endregion

        #region Update Functions
        public override void PreUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            base.Update(elapsedTime, totalTime);
            
            previousKeyboardState.KeyboardState = currentKeyboardState.KeyboardState;
            currentKeyboardState.KeyboardState = Keyboard.GetState();

            previousMouseState.MouseState = currentMouseState.MouseState;
            currentMouseState.MouseState = Mouse.GetState();
        }
        #endregion
    }
}
