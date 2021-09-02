using GuiCookie.DataStructures;
using GuiCookie.Elements;
using GuiCookie.Input;
using LiruGameHelper.Signals;
using Microsoft.Xna.Framework;

namespace GuiCookie.Components
{
    /// <summary> Use this <see cref="Component"/> on an <see cref="Element"/> to handle click detection and mouse events. </summary>
    public class MouseHandler : Component
    {
        #region Constants
        private const string clickTypeAttributeName = "ClickType";
        #endregion

        #region Dependencies
        /// <summary> The <see cref="inputManager"/> used to determine if the handler was clicked. </summary>
        private readonly InputManager inputManager;
        #endregion

        #region Properties
        /// <summary> Quick access for <see cref="InputManager.MousePosition"/>. </summary>
        public Point AbsoluteMousePosition => inputManager.MousePosition;

        /// <summary> The position of the mouse relative to this component's <see cref="Element"/>. </summary>
        public Point RelativeMousePosition => inputManager.MousePosition - Element.Bounds.AbsoluteTotalPosition;

        /// <summary> Is <c>true</c> if the mouse was left clicked on the <see cref="Element"/> and is currently being held down, regardless of mouse position. </summary>
        public bool IsClickDragged { get; private set; }

        /// <summary> Is <c>true</c> if the mouse is over the <see cref="Element"/>. </summary>
        public bool IsMousedOver { get; private set; }

        /// <summary> Is <c>true</c> if the mouse is over the <see cref="Element"/> and the <see cref="InputManager.MousedOverClickable"/> is the <see cref="Element"/>. </summary>
        public bool IsMainMousedOver { get; private set; }

        /// <summary> <c>true</c> if the mouse is over the <see cref="Element"/> and the left click button is pressed. </summary>
        /// <remarks> Will be true for every frame that the mouse is over the element and clicked, hence it is better to add a listener for the LeftClicked signal. </remarks>
        public bool IsLeftClicked { get; private set; }

        /// <summary> <c>true</c> if the mouse is over the <see cref="Element"/> and the right click button is pressed. </summary>
        /// <remarks> Will be true for every frame that the mouse is over the element and clicked, hence it is better to add a listener for the RightClicked signal. </remarks>
        public bool IsRightClicked { get; private set; }
        
        /// <summary> The mode of click detection to use. </summary>
        public ClickType ClickType { get; set; }
        #endregion

        #region Signals
        /// <summary> Is fired when the mouse enters this <see cref="Element"/>. </summary>
        public IConnectableSignal MouseEntered => mouseEntered;
        private readonly Signal mouseEntered = new Signal();

        /// <summary> Is fired when the mouse leaves this <see cref="Element"/>. </summary>
        public IConnectableSignal MouseLeft => mouseLeft;
        private readonly Signal mouseLeft = new Signal();

        /// <summary> Is fired when the mouse is over this <see cref="Element"/> and the left mouse button is released/pressed depending on the <see cref="ClickType"/>. </summary>
        public IConnectableSignal LeftClicked => leftClicked;
        private readonly Signal leftClicked = new Signal();

        /// <summary> Is fired when the mouse is over this <see cref="Element"/> and the right mouse button is released/pressed depending on the <see cref="ClickType"/>. </summary>
        public IConnectableSignal RightClicked => rightClicked;
        private readonly Signal rightClicked = new Signal();
        #endregion

        #region Constructors
        /// <summary> Creates a new <see cref="MouseHandler"/> with the given <paramref name="inputManager"/>. </summary>
        /// <param name="inputManager"> The <see cref="InputManager"/> used for click detection. </param>
        public MouseHandler(InputManager inputManager)
        {
            // Set dependencies.
            this.inputManager = inputManager;
        }
        #endregion

        #region Initialisation Functions
        /// <summary> Sets the relevant data for this <see cref="Component"/>. </summary>
        public override void OnCreated()
        {
            // Set the click type.
            ClickType = Element.Attributes.GetEnumAttributeOrDefault(clickTypeAttributeName, ClickType.OnMouseUp);
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
        public override void Update(GameTime gameTime)
        {
            // If the element is disabled, do nothing.
            if (!Element.Enabled) return;

            // Before updating any of the other properties, check for any changes.

            // If the element is moused over by the input manager, and the mouse is within the bounds of the element, mousedOver is true.
            // Also check against the previous frame's moused over value to check if the mouse has entered or left the element's bounds.
            bool isMousedOver = Element.Bounds.AbsoluteContains(inputManager.MousePosition);
            bool isMainMousedOver = inputManager.MousedOverClickable == Element && isMousedOver;
            if (IsMousedOver && !isMousedOver && Element.Enabled) mouseLeft.Invoke();
            if (!IsMousedOver && isMousedOver && Element.Enabled) mouseEntered.Invoke();

            // If the element is moused over, and the mouse was clicked within the element, fire the clicked events.
            // The left clicked function only fires when there is a change in the mouse state in the previous frame, and the mouse started and ended within the element.
            if (IsMainMousedOver)
                switch (ClickType)
                {
                    case ClickType.OnMouseDown:
                        if (inputManager.IsLeftMouseDown && inputManager.WasLeftMouseUp && Element.Bounds.AbsoluteContains(inputManager.MouseLeftClickPosition)) leftClicked.Invoke();
                        if (inputManager.IsRightMouseDown && inputManager.WasRightMouseUp && Element.Bounds.AbsoluteContains(inputManager.MouseRightClickPosition)) rightClicked.Invoke();
                        break;
                    case ClickType.OnMouseUp:
                        if (inputManager.IsLeftMouseUp && inputManager.WasLeftMouseDown && Element.Bounds.AbsoluteContains(inputManager.MouseLeftClickPosition)) leftClicked.Invoke();
                        if (inputManager.IsRightMouseUp && inputManager.WasRightMouseDown && Element.Bounds.AbsoluteContains(inputManager.MouseRightClickPosition)) rightClicked.Invoke();
                        break;
                }

            // If the mouse is over the element, set the moused over.
            IsMousedOver = isMousedOver;
            IsMainMousedOver = isMainMousedOver;

            // Set the left and right clicked.
            IsClickDragged = Element.Bounds.AbsoluteContains(inputManager.MouseLeftClickPosition) && inputManager.IsLeftMouseDown;
            IsLeftClicked = IsMainMousedOver && inputManager.IsLeftMouseDown;
            IsRightClicked = IsMainMousedOver && inputManager.IsRightMouseDown;
        }
        #endregion
    }
}
