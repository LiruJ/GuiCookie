using GuiCookie.Core.DataStructures;
using System.Drawing;

namespace GuiCookie.Core.Input
{
    public interface IMouseState
    {
        #region Position Properties
        Point Position { get; }
        #endregion

        #region Button Properties
        bool LeftButtonDown { get; }

        bool RightButtonDown { get; }

        bool MiddleButtonDown { get; }

        bool XButton1Down { get; }

        bool XButton2Down { get; }
        #endregion

        #region Wheel Properties
        int ScrollValueX { get; }

        int ScrollValueY { get; }
        #endregion
    }
}
