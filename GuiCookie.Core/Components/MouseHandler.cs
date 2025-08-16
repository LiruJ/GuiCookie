using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Helpers;
using GuiCookie.Core.Input;
using LiruGameHelper.Signals;
using System.Drawing;

namespace GuiCookie.Core.Components
{
    /// <summary>
    /// Use this <see cref="Component"/> on an <see cref="Element"/> to handle click detection and mouse events.
    /// </summary>
    /// <param name="elementInputManager"> The <see cref="ElementInputManager"/> used for click detection. </param>
    public class MouseHandler(ElementInputManager elementInputManager) : Component
    {
        #region Constants
        public const string ClickTypeAttributeName = "ClickType";
        #endregion

        #region Properties
        /// <summary>
        /// The position of the mouse relative to this component's <see cref="Element"/>.
        /// </summary>
        public Point RelativeMousePosition => PointExtensions.Subtract(elementInputManager.InputManager.MousePosition, Element.Bounds.AbsoluteTotalPosition);

        /// <summary>
        /// Is <c>true</c> if the mouse was left clicked on the <see cref="Element"/> and is currently being held down, regardless of mouse position.
        /// </summary>
        public bool IsClickDragged { get; private set; }

        /// <summary>
        /// Is <c>true</c> if the mouse is over the <see cref="Element"/>.
        /// </summary>
        public bool IsMousedOver { get; private set; }

        /// <summary>
        /// Is <c>true</c> if the mouse is over the <see cref="Element"/> and the <see cref="InputManager.MousedOverClickable"/> is the <see cref="Element"/>.
        /// </summary>
        public bool IsMainMousedOver { get; private set; }

        /// <summary>
        /// <c>true</c> if the mouse is over the <see cref="Element"/> and the left click button is pressed.
        /// </summary>
        /// <remarks> Will be true for every frame that the mouse is over the element and clicked, hence it is better to add a listener for the LeftClicked signal. </remarks>
        public bool IsLeftClicked { get; private set; }

        /// <summary>
        /// <c>true</c> if the mouse is over the <see cref="Element"/> and the right click button is pressed.
        /// </summary>
        /// <remarks> Will be true for every frame that the mouse is over the element and clicked, hence it is better to add a listener for the RightClicked signal. </remarks>
        public bool IsRightClicked { get; private set; }

        /// <summary>
        /// The mode of click detection to use.
        /// </summary>
        public ClickType ClickType { get; set; }
        #endregion

        #region Signals
        /// <summary> Is fired when the mouse enters this <see cref="Element"/>. </summary>
        public IConnectableSignal MouseEntered => mouseEntered;
        private readonly Signal mouseEntered = new();

        /// <summary> Is fired when the mouse leaves this <see cref="Element"/>. </summary>
        public IConnectableSignal MouseLeft => mouseLeft;
        private readonly Signal mouseLeft = new();

        /// <summary> Is fired when the mouse is over this <see cref="Element"/> and the left mouse button is released/pressed depending on the <see cref="ClickType"/>. </summary>
        public IConnectableSignal LeftClicked => leftClicked;
        private readonly Signal leftClicked = new();

        /// <summary> Is fired when the mouse is over this <see cref="Element"/> and the right mouse button is released/pressed depending on the <see cref="ClickType"/>. </summary>
        public IConnectableSignal RightClicked => rightClicked;
        private readonly Signal rightClicked = new();
        #endregion

        #region Initialisation Functions
        /// <summary> Sets the relevant data for this <see cref="Component"/>. </summary>
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Set the click type.
            ClickType = attributes.GetEnumAttributeOrDefault(ClickTypeAttributeName, ClickType.OnMouseUp);
        }
        #endregion

        #region Event Functions
        public override void OnDestroyed()
        {
            mouseEntered.DisconnectAll();
            mouseLeft.DisconnectAll();
            leftClicked.DisconnectAll();
            rightClicked.DisconnectAll();
        }
        #endregion

        #region Update Functions
        /// <summary> Updates the mouse properties, and fires any relevant <see cref="Signal"/>s. </summary>
        /// <param name="gameTime"> The current time of the game. </param>
        public override void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            // If the element is disabled, do nothing.
            if (!Element.Enabled) return;

            // Before updating any of the other properties, check for any changes.

            // If the element is moused over by the input manager, and the mouse is within the bounds of the element, mousedOver is true.
            // Also check against the previous frame's moused over value to check if the mouse has entered or left the element's bounds.
            bool isMousedOver = Element.Bounds.AbsoluteContains(elementInputManager.InputManager.MousePosition);
            bool isMainMousedOver = elementInputManager.MousedOverClickable == Element && isMousedOver;
            if (IsMousedOver && !isMousedOver)
                mouseLeft.Invoke();
            if (!IsMousedOver && isMousedOver)
                mouseEntered.Invoke();


            // If the element is moused over, and the mouse was clicked within the element, fire the clicked events.
            // The left clicked function only fires when there is a change in the mouse state in the previous frame, and the mouse started and ended within the element.
            if (IsMainMousedOver)
                switch (ClickType)
                {
                    case ClickType.OnMouseDown:
                        if (elementInputManager.InputManager.IsLeftMouseDown && elementInputManager.InputManager.WasLeftMouseUp && Element.Bounds.AbsoluteContains(elementInputManager.InputManager.MouseLeftClickPosition))
                        leftClicked.Invoke();
                        if (elementInputManager.InputManager.IsRightMouseDown && elementInputManager.InputManager.WasRightMouseUp && Element.Bounds.AbsoluteContains(elementInputManager.InputManager.MouseRightClickPosition))
                            rightClicked.Invoke();
                        break;
                    case ClickType.OnMouseUp:
                        if (elementInputManager.InputManager.IsLeftMouseUp && elementInputManager.InputManager.WasLeftMouseDown && Element.Bounds.AbsoluteContains(elementInputManager.InputManager.MouseLeftClickPosition))
                            leftClicked.Invoke();
                        if (elementInputManager.InputManager.IsRightMouseUp && elementInputManager.InputManager.WasRightMouseDown && Element.Bounds.AbsoluteContains(elementInputManager.InputManager.MouseRightClickPosition))
                            rightClicked.Invoke();
                        break;
                }

            // If the mouse is over the element, set the moused over.
            IsMousedOver = isMousedOver;
            IsMainMousedOver = isMainMousedOver;

            // Set the left and right clicked.
            IsClickDragged = Element.Bounds.AbsoluteContains(elementInputManager.InputManager.MouseLeftClickPosition) && elementInputManager.InputManager.IsLeftMouseDown;
            IsLeftClicked = IsMainMousedOver && elementInputManager.InputManager.IsLeftMouseDown;
            IsRightClicked = IsMainMousedOver && elementInputManager.InputManager.IsRightMouseDown;
        }
        #endregion
    }
}
