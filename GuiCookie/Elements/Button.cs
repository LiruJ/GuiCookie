using GuiCookie.Components;
using LiruGameHelper.Signals;
using System;

namespace GuiCookie.Elements
{
    public class Button : Element, IClickable
    {
        #region Components
        /// <summary> The component that handles mouse events. </summary>
        public MouseHandler MouseHandler { get; protected set; }
        #endregion

        #region Signals
        /// <summary> Fired when the button is left clicked. </summary>
        public IConnectableSignal LeftClicked => MouseHandler.LeftClicked;

        /// <summary> Fired when the button is right clicked. </summary>
        public IConnectableSignal RightClicked => MouseHandler.RightClicked;
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        { 
            // Set components.
            MouseHandler = GetComponent<MouseHandler>() ?? throw new Exception("Button's mouse handler is missing.");
        }
        #endregion

        #region Bind Functions
        public void ConnectLeftClick(Action action) => LeftClicked.Connect(action);

        public void ConnectRightClick(Action action) => RightClicked.Connect(action);
        #endregion
    }
}
