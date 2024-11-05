using GuiCookie.Core.Input;
using System;
using System.Collections.Generic;
using Ultraviolet.Input;

namespace GuiCookie.Ultraviolet.Input
{
    public class UltravioletKeyboardState : IKeyboardState
    {
        #region Constants
        private static readonly Key[] keyTypes;
        #endregion

        #region Fields
        private HashSet<Key> pressedKeys = new();
        #endregion

        #region Properties
        public IReadOnlySet<Key> PressedKeys => pressedKeys;
        #endregion

        #region Constructors
        static UltravioletKeyboardState()
        {
            keyTypes = (Key[])Enum.GetValues(typeof(Key));
        }
        #endregion

        #region Key Functions
        public bool IsKeyDown(Key key) => pressedKeys.Contains(key);

        public bool IsKeyDown(GUIKey key) => pressedKeys.Contains((Key)key);

        public bool IsKeyUp(Key key) => !pressedKeys.Contains(key);

        public bool IsKeyUp(GUIKey key) => !pressedKeys.Contains((Key)key);

        public static void UpdateStates(KeyboardDevice keyboard, UltravioletKeyboardState currentState, UltravioletKeyboardState previousState)
        {
            // Swap the sets around between the states. This means that the previous state will automatically have the old state.
            (previousState.pressedKeys, currentState.pressedKeys) = (currentState.pressedKeys, previousState.pressedKeys);

            // Set the current state based on the keyboard.
            currentState.pressedKeys.Clear();
            foreach (Key key in keyTypes)
                if (keyboard.IsKeyDown(key))
                    currentState.pressedKeys.Add(key);
        }
        #endregion
    }
}
