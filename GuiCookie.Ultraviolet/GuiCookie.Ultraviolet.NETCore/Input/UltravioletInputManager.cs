using GuiCookie.Core.Input;
using GuiCookie.Ultraviolet.Extensions;
using System;
using Ultraviolet;
using Ultraviolet.Input;
using Ultraviolet.Platform;

namespace GuiCookie.Ultraviolet.Input
{
    internal class UltravioletInputManager : InputManager
    {
        #region Dependencies
        private readonly KeyboardDevice keyboard;
        private readonly MouseDevice mouse;
        #endregion

        #region Properties
        public Point2 CurrentCursorPosition => CurrentMouseState.Position.ToPoint2();

        public override UltravioletKeyboardState CurrentKeyboardState { get; }

        public override UltravioletKeyboardState PreviousKeyboardState { get; }

        public override UltravioletMouseState CurrentMouseState { get; }

        public override UltravioletMouseState PreviousMouseState { get; }
        #endregion

        #region Constructors
        public UltravioletInputManager(KeyboardDevice keyboard, MouseDevice mouse) : base(new UltravioletKeyboardState(), new UltravioletKeyboardState(), new UltravioletMouseState(), new UltravioletMouseState())
        {
            this.keyboard = keyboard;
            this.mouse = mouse;

            keyboard.TextInput += (IUltravioletWindow window, KeyboardDevice inputKeyboard) => keyboard.GetTextInput(TextInput);
        }
        #endregion

        #region Update Functions
        public override void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            base.Update(elapsedTime, totalTime);

            UltravioletKeyboardState.UpdateStates(keyboard, CurrentKeyboardState, PreviousKeyboardState);
            UltravioletMouseState.UpdateStates(mouse, CurrentMouseState, PreviousMouseState);
        }
        #endregion
    }
}
