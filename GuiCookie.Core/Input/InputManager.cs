using GuiCookie.Core.Services;
using System.Drawing;
using System.Text;

namespace GuiCookie.Core.Input
{
    public abstract class InputManager(IKeyboardState currentKeyboardState, IKeyboardState previousKeyboardState, IMouseState currentMouseState, IMouseState previousMouseState) : IUpdatableUIService
    {
        #region Properties
        public int UpdateOrder { get; } = 0;
        #endregion

        #region State Properties
        /// <summary>
        /// The current position of the mouse.
        /// </summary>
        public Point MousePosition => CurrentMouseState.Position;

        /// <summary>
        /// Gets the value representing the position of the <see cref="Mouse"/> when the left click button was pressed, only changing when the button is released then pressed again.
        /// </summary>
        public Point MouseLeftClickPosition { get; private set; }

        /// <summary> 
        /// Gets the value representing the position of the <see cref="Mouse"/> when the right click button was pressed, only changing when the button is released then pressed again.
        /// </summary>
        public Point MouseRightClickPosition { get; private set; }

        /// <summary>
        /// The <see cref="IKeyboardState"/> of the current frame.
        /// </summary>
        public IKeyboardState CurrentKeyboardState { get; } = currentKeyboardState;

        /// <summary>
        /// The <see cref="IKeyboardState"/> of the previous frame.
        /// </summary>
        public virtual IKeyboardState PreviousKeyboardState { get; } = previousKeyboardState;

        /// <summary>
        /// The <see cref="IMouseState"/> of the current frame.
        /// </summary>
        public virtual IMouseState CurrentMouseState { get; } = currentMouseState;

        /// <summary>
        /// The <see cref="IMouseState"/> of the previous frame.
        /// </summary>
        public virtual IMouseState PreviousMouseState { get; } = previousMouseState;

        /// <summary>
        /// The text input made in this frame.
        /// </summary>
        public StringBuilder TextInput { get; } = new StringBuilder(16);

        public bool IsLeftMouseDown => CurrentMouseState.LeftButtonDown;

        public bool IsLeftMouseUp => !CurrentMouseState.LeftButtonDown;

        public bool WasLeftMouseDown => PreviousMouseState.LeftButtonDown;

        public bool WasLeftMouseUp => !PreviousMouseState.LeftButtonDown;

        public bool IsRightMouseDown => CurrentMouseState.RightButtonDown;

        public bool IsRightMouseUp => !CurrentMouseState.RightButtonDown;

        public bool WasRightMouseDown => PreviousMouseState.RightButtonDown;

        public bool WasRightMouseUp => !PreviousMouseState.RightButtonDown;

        public int ScrollDeltaX => CurrentMouseState.ScrollValueX - PreviousMouseState.ScrollValueX;

        public int ScrollDeltaY => CurrentMouseState.ScrollValueY - PreviousMouseState.ScrollValueY;
        #endregion

        #region Key Functions
        /// <summary>
        /// Gets a value that is <c>true</c> when the given <paramref name="key"/> is pressed on the current frame; otherwise, <c>false</c>.
        /// </summary>
        /// <param name="key"> The <see cref="KeyType"/> to check. </param>
        public virtual bool IsKeyDown(GUIKey key) => CurrentKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Gets a value that is <c>true</c> when the given <paramref name="key"/> is unpressed on the current frame; otherwise, <c>false</c>.
        /// </summary>
        /// <param name="key"> The <see cref="KeyType"/> to check. </param>
        public virtual bool IsKeyUp(GUIKey key) => CurrentKeyboardState.IsKeyUp(key);

        /// <summary>
        /// Gets a value that is <c>true</c> when the given <paramref name="key"/> was pressed on the previous frame; otherwise, <c>false</c>.
        /// </summary>
        /// <param name="key"> The <see cref="KeyType"/> to check. </param>
        public virtual bool WasKeyDown(GUIKey key) => PreviousKeyboardState.IsKeyDown(key);

        /// <summary>
        /// Gets a value that is <c>true</c> when the given <paramref name="key"/> was unpressed on the previous frame; otherwise, <c>false</c>.
        /// </summary>
        /// <param name="key"> The <see cref="KeyType"/> to check. </param>
        public virtual bool WasKeyUp(GUIKey key) => PreviousKeyboardState.IsKeyUp(key);
        #endregion

        #region Update Functions
        public virtual void PreUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            TextInput.Clear();
        }

        public virtual void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {

        }

        public virtual void PostUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {

        }

        protected virtual void UpdateClickedProperties(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            if (IsLeftMouseDown && WasLeftMouseUp)
                MouseLeftClickPosition = CurrentMouseState.Position;
            if (IsRightMouseDown && WasRightMouseUp)
                MouseRightClickPosition = CurrentMouseState.Position;
        }
        #endregion
    }
}
