using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Input;
using GuiCookie.Ultraviolet.Extensions;
using Ultraviolet.Input;

namespace GuiCookie.Ultraviolet.Input
{
    public class UltravioletMouseState : IMouseState
    {
        #region Position Properties
        public GUIPoint Position { get; private set; }
        #endregion

        #region Fields
        private Core.Input.MouseButton mouseButtonFlag;
        #endregion

        #region Button Properties
        public bool LeftButtonDown => mouseButtonFlag.HasFlag(Core.Input.MouseButton.LeftButton);

        public bool RightButtonDown => mouseButtonFlag.HasFlag(Core.Input.MouseButton.RightButton);

        public bool MiddleButtonDown => mouseButtonFlag.HasFlag(Core.Input.MouseButton.MiddleButton);

        public bool XButton1Down => mouseButtonFlag.HasFlag(Core.Input.MouseButton.XButton1);

        public bool XButton2Down => mouseButtonFlag.HasFlag(Core.Input.MouseButton.XButton2);
        #endregion

        #region Wheel Properties
        public int ScrollValueX { get; private set; } = 0;

        public int ScrollValueY { get; private set; } = 0;
        #endregion

        #region Mouse Functions
        public static void UpdateStates(MouseDevice mouse, UltravioletMouseState currentState, UltravioletMouseState previousState)
        {
            previousState.Position = currentState.Position;
            currentState.Position = mouse.Position.ToGUIPoint();

            previousState.ScrollValueX = currentState.ScrollValueX;
            currentState.ScrollValueX += mouse.WheelDeltaX;
            
            previousState.ScrollValueY = currentState.ScrollValueY;
            currentState.ScrollValueY += mouse.WheelDeltaY;

            previousState.mouseButtonFlag = currentState.mouseButtonFlag;
            currentState.mouseButtonFlag = Core.Input.MouseButton.None;

            if (mouse.IsButtonDown(global::Ultraviolet.Input.MouseButton.Left))
                currentState.mouseButtonFlag |= Core.Input.MouseButton.LeftButton;
            if (mouse.IsButtonDown(global::Ultraviolet.Input.MouseButton.Right))
                currentState.mouseButtonFlag |= Core.Input.MouseButton.RightButton;
            if (mouse.IsButtonDown(global::Ultraviolet.Input.MouseButton.Middle))
                currentState.mouseButtonFlag |= Core.Input.MouseButton.MiddleButton;
            if (mouse.IsButtonDown(global::Ultraviolet.Input.MouseButton.XButton1))
                currentState.mouseButtonFlag |= Core.Input.MouseButton.XButton1;
            if (mouse.IsButtonDown(global::Ultraviolet.Input.MouseButton.XButton2))
                currentState.mouseButtonFlag |= Core.Input.MouseButton.XButton2;
        }
        #endregion
    }
}