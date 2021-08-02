using GuiCookie.Components;
using GuiCookie.DataStructures;
using GuiCookie.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GuiCookie.Input
{
    /// <summary> Keeps internal buffers of both <see cref="Keyboard"/> and <see cref="Mouse"/> states and supplies easy methods of checking against those states. </summary>
    public class InputManager
    {
        #region Fields
        /// <summary> The <see cref="KeyboardState"/> of the current frame. </summary>
        private KeyboardState currentKeyboardState;

        /// <summary> The <see cref="KeyboardState"/> of the previous frame. </summary>
        private KeyboardState previousKeyboardState;

        /// <summary> The <see cref="MouseState"/> of the current frame. </summary>
        private MouseState currentMouseState;

        /// <summary> The <see cref="MouseState"/> of the previous frame. </summary>
        private MouseState previousMouseState;
        #endregion

        #region Properties
        /// <summary> The current position of the <see cref="Mouse"/>. </summary>
        public Point MousePosition { get; private set; }

        /// <summary> Gets the value representing the position of the <see cref="Mouse"/> when the left click button was pressed, only changing when the button is released then pressed again. </summary>
        public Point MouseLeftClickPosition { get; private set; }

        /// <summary> Gets the value representing the position of the <see cref="Mouse"/> when the right click button was pressed, only changing when the button is released then pressed again. </summary>
        public Point MouseRightClickPosition { get; private set; }

        /// <summary> The deepest <see cref="Element"/> that the <see cref="Mouse"/> is currently hovering over. </summary>
        public Element MousedOverElement { get; private set; }

        /// <summary> The deepest <see cref="IClickable"/> that the <see cref="Mouse"/> is currently hovering over. </summary>
        public IClickable MousedOverClickable { get; private set; }
        #endregion

        #region Key Properties
        /// <summary> Gets a value that is <c>true</c> when the given <paramref name="key"/> is pressed on the current frame; otherwise, <c>false</c>. </summary>
        /// <param name="key"> The <see cref="Key"/> to check. </param>
        public bool IsKeyDown(Keys key) => currentKeyboardState.IsKeyDown(key);

        /// <summary> Gets a value that is <c>true</c> when the given <paramref name="key"/> is unpressed on the current frame; otherwise, <c>false</c>. </summary>
        /// <param name="key"> The <see cref="Key"/> to check. </param>
        public bool IsKeyUp(Keys key) => !IsKeyDown(key);

        /// <summary> Gets a value that is <c>true</c> when the given <paramref name="key"/> was pressed on the previous frame; otherwise, <c>false</c>. </summary>
        /// <param name="key"> The <see cref="Key"/> to check. </param>
        public bool WasKeyDown(Keys key) => previousKeyboardState.IsKeyDown(key);

        /// <summary> Gets a value that is <c>true</c> when the given <paramref name="key"/> was unpressed on the previous frame; otherwise, <c>false</c>. </summary>
        /// <param name="key"> The <see cref="Key"/> to check. </param>
        public bool WasKeyUp(Keys key) => !WasKeyDown(key);
        #endregion

        #region Mouse Properties
        /// <summary> Gets a value that is <c>true</c> when the left mouse button is clicked on the current frame; otherwise, <c>false</c>. </summary>
        public bool IsLeftMouseDown => currentMouseState.LeftButton == ButtonState.Pressed;

        /// <summary> Gets a value that is <c>true</c> when the left mouse button is unclicked on the current frame; otherwise, <c>false</c>. </summary>
        public bool IsLeftMouseUp => !IsLeftMouseDown;

        /// <summary> Gets a value that is <c>true</c> when the left mouse button was clicked on the previous frame; otherwise, <c>false</c>. </summary>
        public bool WasLeftMouseDown => previousMouseState.LeftButton == ButtonState.Pressed;

        /// <summary> Gets a value that is <c>true</c> when the left mouse button was unclicked on the previous frame; otherwise, <c>false</c>. </summary>
        public bool WasLeftMouseUp => !WasLeftMouseDown;

        /// <summary> Gets a value that is <c>true</c> when the right mouse button is clicked on the current frame; otherwise, <c>false</c>. </summary>
        public bool IsRightMouseDown => currentMouseState.RightButton == ButtonState.Pressed;

        /// <summary> Gets a value that is <c>true</c> when the right mouse button is unclicked on the current frame; otherwise, <c>false</c>. </summary>
        public bool IsRightMouseUp => !IsRightMouseDown;

        /// <summary> Gets a value that is <c>true</c> when the right mouse button was clicked on the previous frame; otherwise, <c>false</c>. </summary>
        public bool WasRightMouseDown => previousMouseState.RightButton == ButtonState.Pressed;

        /// <summary> Gets a value that is <c>true</c> when the right mouse button was unclicked on the previous frame; otherwise, <c>false</c>. </summary>
        public bool WasRightMouseUp => !WasRightMouseDown;
        #endregion

        #region Constructors
        /// <summary> Creates a new <see cref="InputManager"/> using the default Monogame input. </summary>
        public InputManager() { }
        #endregion

        #region Update Functions
        /// <summary> Updates the internal <see cref="KeyboardState"/>s and <see cref="MouseState"/>s, giving a current snapshot of the inputs. </summary>
        public void UpdateMouseAndKeyboardStates()
        {
            // Set the previous keyboard and mouse states to the current states, before they are changed.
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // Get the states of the keyboard and mouse.
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            // Get the position of the mouse.
            MousePosition = currentMouseState.Position;

            // If the a mouse button was up last update and down this update, track the position.
            if (IsLeftMouseDown && WasLeftMouseUp) MouseLeftClickPosition = MousePosition;
            if (IsRightMouseDown && WasRightMouseUp) MouseRightClickPosition = MousePosition;
        }
        
        /// <summary> Updates the <see cref="Elements"/> properties, giving a current snapshot of the selected <see cref="Elements"/>s. </summary>
        /// <param name="elementManager"></param>
        public void UpdateElementStates(ElementManager elementManager)
        {
            // Set the moused over element and clickable.
            MousedOverElement = findMousedOverElement(elementManager);
            MousedOverClickable = FindMousedOverBase<IClickable>(elementManager);
        }

        /// <summary> Finds the <see cref="Elements"/> that the <see cref="Mouse"/> is currently over, or <c>null</c> if none was found. </summary>
        /// <param name="container"> The <see cref="IEnumerable<Element>"/> whose <see cref="Elements"/>s to check. </param>
        /// <returns> The moused over <see cref="Elements"/>; or <c>null</c> if none was found. </returns>
        /// <remarks> Recursively calls upon this function in order to perform the search. The recursion happens before the component check, so elements with no children are checked first, then their parents, and so on. </remarks>
        private Element findMousedOverElement(IEnumerable<Element> container)
        {
            foreach (Element element in container)
            {
                // If this element is moused over, check its children.
                if (isMouseOverable(element) && element.Bounds.AbsoluteContains(MousePosition))
                {
                    // If the element has children, find the moused over child element; otherwise, set the moused over element to this element and return.
                    if (element.HasChildren) return findMousedOverElement(element) ?? element;
                    // If the element has children and none are moused over, default to the containing element.
                    else return element;
                }
            }

            // If the code ever gets to this point, it means no element was moused over at all, so return null.
            return null;
        }

        /// <summary> Finds the first visible and mouse blocking <see cref="Element"/> in the given <paramref name="container"/> that is of the given type <typeparamref name="T"/>, or null if none was found. </summary>
        /// <typeparam name="T"> The type of <see cref="Element"/> to get. </typeparam>
        /// <param name="container"> The <see cref="IEnumerable<Element>"/> from which to begin the search. </param>
        /// <param name="ignoreElement"> The element to ignore, or null if no elements are to be ignored. </param>
        /// <returns></returns>
        /// <remarks> Recursively calls upon this function in order to perform the search. The recursion happens before the component check, so elements with no children are checked first, then their parents, and so on. </remarks>
        public T FindMousedOverBase<T>(IEnumerable<Element> container, Element ignoreElement = null) where T : class
        {
            foreach (Element element in container)
            {
                // If this element is moused over, check its children.
                if (isMouseOverable(element, ignoreElement) && element.Bounds.AbsoluteContains(MousePosition))
                {
                    // If the element has children, recursively search them.
                    if (element.HasChildren)
                    {
                        T childT = FindMousedOverBase<T>(element, ignoreElement);
                        if (childT != null) 
                            return childT;
                    }

                    // If the element is of the given type, return it.
                    if (element is T t) return t;
                }
            }

            // If the code ever gets to this point, it means no element was moused over at all, so return null.
            return null;
        }

        /// <summary> Finds the <see cref="Component"/> belonging to an <see cref="Element"/> furthest removed from the root-level that is moused over. </summary>
        /// <typeparam name="T"> The type of <see cref="Component"/> to find. </typeparam>
        /// <param name="container"> The element container to check first. </param>
        /// <param name="ignoreElement"> The element to ignore, or null if no elements are to be ignored. </param>
        /// <returns> The <typeparamref name="T"/> version of the moused over component. </returns>
        /// <remarks> Recursively calls upon this function in order to perform the search. The recursion happens before the component check, so elements with no children are checked first, then their parents, and so on. </remarks>
        public T FindMousedOverWithComponent<T>(IEnumerable<Element> container, Element ignoreElement = null) where T : Component
        {
            foreach (Element element in container)
            {
                // If this element is moused over, check its children. Ignore it if it's the ignore element.
                if (isMouseOverable(element, ignoreElement) && element.Bounds.AbsoluteContains(MousePosition))
                {
                    // If the element has children, recursively search them.
                    T component;
                    if (element.HasChildren)
                    {
                        component = FindMousedOverWithComponent<T>(element, ignoreElement);
                        if (component != null) 
                            return component;
                    }

                    // If the element has a component of the given type, return it.
                    if (element.TryGetComponent(out component)) return component;
                }
            }

            // If the code ever gets to this point, it means no valid element was found, so return null.
            return null;
        }

        public T FindInBoundsWithInterfacedComponent<T>(ElementManager container, Rectangle bounds, Element ignoreElement = null) where T : class
            => FindInBoundsWithInterfacedComponent<T>(container.ElementContainer, bounds, ignoreElement);

        public T FindInBoundsWithInterfacedComponent<T>(Element container, Rectangle bounds, Element ignoreElement = null) where T : class
            => FindInBoundsWithInterfacedComponent<T>(container.ElementContainer, bounds, ignoreElement);

        /// <summary> Finds the <see cref="Component"/> belonging to an <see cref="Element"/> furthest removed from the root-level that is moused over and implements the given <typeparamref name="T"/> interface. </summary>
        /// <typeparam name="T"> The type of interface to find. </typeparam>
        /// <param name="container"> The element container to check first. </param>
        /// <param name="bounds"> The bounds to check. </param>
        /// <param name="ignoreElement"> The element to ignore, or null if no elements are to be ignored. </param>
        /// <returns> The interfaced version of the moused over component. </returns>
        /// <remarks> Recursively calls upon this function in order to perform the search. The recursion happens before the component check, so elements with no children are checked first, then their parents, and so on. </remarks>
        internal T FindInBoundsWithInterfacedComponent<T>(ElementContainer container, Rectangle bounds, Element ignoreElement = null) where T : class
        {
            if (!typeof(T).IsInterface) return null;

            Element element;
            for (int i = container.Count - 1; i >= 0; i--)
            {
                element = container.GetChildByIndex(i);
                
                // If this element is moused over, check its children. Ignore it if it's the ignore element.
                if (isMouseOverable(element, ignoreElement) && (bounds.Intersects(element.Bounds.AbsoluteTotalArea) || bounds.Contains(element.Bounds.AbsoluteTotalArea)))
                {
                    // If the element has children, recursively search them.
                    T component;
                    if (element.HasChildren)
                    {
                        component = FindInBoundsWithInterfacedComponent<T>(element, bounds, ignoreElement);
                        if (component != null)
                            return component;
                    }

                    // If the element has a component of the given type, return it.
                    if (element.TryGetInterfacedComponent(out component)) return component;
                }
            }

            // If the code ever gets to this point, it means no valid element was found, so return null.
            return null;
        }
        #endregion

        #region Helper Functions
        private bool isMouseOverable(Element element, Element ignoreElement = null) 
            => (ignoreElement == null || element != ignoreElement) && element.BlocksMouse && element.Visible;
        #endregion
    }
}