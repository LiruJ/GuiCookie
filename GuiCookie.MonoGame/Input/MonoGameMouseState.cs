using GuiCookie.Core.Input;
using GuiCookie.MonoGame.Extensions;
using Microsoft.Xna.Framework.Input;
using System.Drawing;

namespace GuiCookie.MonoGame.Input
{
    public class MonoGameMouseState : IMouseState
    {
        #region Position Properties
        public Point Position => MouseState.Position.ToDrawingPoint();
        #endregion

        #region Mouse Properties
        public MouseState MouseState { get; set; }
        #endregion

        #region Button Properties
        public bool LeftButtonDown => MouseState.LeftButton == ButtonState.Pressed;

        public bool RightButtonDown => MouseState.RightButton == ButtonState.Pressed;

        public bool MiddleButtonDown => MouseState.MiddleButton == ButtonState.Pressed;

        public bool XButton1Down => MouseState.XButton1 == ButtonState.Pressed;

        public bool XButton2Down => MouseState.XButton2 == ButtonState.Pressed;
        #endregion

        #region Wheel Properties
        public int ScrollValueX => MouseState.HorizontalScrollWheelValue;

        public int ScrollValueY => MouseState.ScrollWheelValue;
        #endregion
    }
}