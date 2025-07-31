using GuiCookie.Core.Components;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Services;
using System.Drawing;

namespace GuiCookie.Core.Input
{
    /// <summary>
    /// A manager responsible for keeping track of the interaction between the user's inputs and the UI's elements. E.g. clicking, hovering, typing into a textbox, etc.
    /// </summary>
    public class ElementInputManager(InputManager inputManager, ElementManager elementManager) : IUpdatableUIService
    {
        #region Backing Fields
        private bool isClickableDirty = true;

        private IClickable? mousedOverClickable = null;

        private bool isElementDirty = true;

        private Element? mousedOverElement = null;
        #endregion

        #region Properties
        public InputManager InputManager { get; } = inputManager;

        public ElementManager ElementManager { get; } = elementManager;

        public int UpdateOrder { get; } = 5;

        /// <summary> 
        /// The deepest <see cref="Element"/> that the mouse is currently hovering over.
        /// </summary>
        public Element? MousedOverElement
        {
            get
            {
                if (isElementDirty)
                {
                    mousedOverElement = findMousedOverElement(ElementManager);
                    isElementDirty = false;
                }
                return mousedOverElement;
            }
        }

        /// <summary>
        /// The deepest <see cref="IClickable"/> that the mouse is currently hovering over.
        /// </summary>
        public IClickable? MousedOverClickable
        {
            get
            {
                if (isClickableDirty)
                {
                    mousedOverClickable = FindMousedOverBase<IClickable>(ElementManager);
                    isClickableDirty = false;
                }
                return mousedOverClickable;
            }
        }
        #endregion

        #region Update Functions
        public virtual void PreUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            isClickableDirty = true;
            mousedOverClickable = null;

            isElementDirty = true;
            mousedOverElement = null;
        }

        public virtual void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            
        }

        public virtual void PostUpdate(TimeSpan elapsedTime, TimeSpan totalTime)
        {

        }
        #endregion

        #region Element Functions
        /// <summary> Finds the elements that the mouse is currently over, or <c>null</c> if none was found. </summary>
        /// <param name="container"> The collection of elements whose children to check. </param>
        /// <returns> The moused over element; or <c>null</c> if none was found. </returns>
        /// <remarks> Recursively calls upon this function in order to perform the search. The recursion happens before the component check, so elements with no children are checked first, then their parents, and so on. </remarks>
        private Element? findMousedOverElement(IEnumerable<Element> container)
        {
            foreach (Element element in container)
            {
                // If this element is moused over, check its children.
                if (isMouseOverable(element) && element.Bounds.AbsoluteContains(InputManager.MousePosition))
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
        public T? FindMousedOverBase<T>(IEnumerable<Element> container, Element? ignoreElement = null) where T : class
        {
            foreach (Element element in container)
            {
                // If this element is moused over, check its children.
                if (isMouseOverable(element, ignoreElement) && element.Bounds.AbsoluteContains(InputManager.MousePosition))
                {
                    // If the element has children, recursively search them.
                    if (element.HasChildren)
                    {
                        T? childT = FindMousedOverBase<T>(element, ignoreElement);
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
        public T? FindMousedOverWithComponent<T>(IEnumerable<Element> container, Element? ignoreElement = null) where T : Component
        {
            foreach (Element element in container)
            {
                // If this element is moused over, check its children. Ignore it if it's the ignore element.
                if (isMouseOverable(element, ignoreElement) && element.Bounds.AbsoluteContains(InputManager.MousePosition))
                {
                    // If the element has children, recursively search them.
                    T? component;
                    if (element.HasChildren)
                    {
                        component = FindMousedOverWithComponent<T>(element, ignoreElement);
                        if (component != null)
                            return component;
                    }

                    // If the element has a component of the given type, return it.
                    if (element.TryGetComponent(out component))
                        return component;
                }
            }

            // If the code ever gets to this point, it means no valid element was found, so return null.
            return null;
        }

        public T? FindInBoundsWithInterfacedComponent<T>(Element container, Rectangle bounds, Element? ignoreElement = null) where T : class
            => FindInBoundsWithInterfacedComponent<T>(container.ElementContainer, bounds, ignoreElement);

        /// <summary> Finds the <see cref="Component"/> belonging to an <see cref="Element"/> furthest removed from the root-level that is moused over and implements the given <typeparamref name="T"/> interface. </summary>
        /// <typeparam name="T"> The type of interface to find. </typeparam>
        /// <param name="container"> The element container to check first. </param>
        /// <param name="bounds"> The bounds to check. </param>
        /// <param name="ignoreElement"> The element to ignore, or null if no elements are to be ignored. </param>
        /// <returns> The interfaced version of the moused over component. </returns>
        /// <remarks> Recursively calls upon this function in order to perform the search. The recursion happens before the component check, so elements with no children are checked first, then their parents, and so on. </remarks>
        internal T? FindInBoundsWithInterfacedComponent<T>(ElementContainer container, Rectangle bounds, Element? ignoreElement = null) where T : class
        {
            if (!typeof(T).IsInterface)
                return null;

            Element element;
            for (int i = container.Count - 1; i >= 0; i--)
            {
                element = container.GetChildByIndex(i);

                // If this element is moused over, check its children. Ignore it if it's the ignore element.
                if (isMouseOverable(element, ignoreElement) && (bounds.IntersectsWith(element.Bounds.AbsoluteTotalArea) || bounds.Contains(element.Bounds.AbsoluteTotalArea)))
                {
                    // If the element has children, recursively search them.
                    T? component;
                    if (element.HasChildren)
                    {
                        component = FindInBoundsWithInterfacedComponent<T>(element, bounds, ignoreElement);
                        if (component != null)
                            return component;
                    }

                    // If the element has a component of the given type, return it.
                    if (element.TryGetInterfacedComponent(out component))
                        return component;
                }
            }

            // If the code ever gets to this point, it means no valid element was found, so return null.
            return null;
        }

        private static bool isMouseOverable(Element element, Element? ignoreElement = null)
            => (ignoreElement == null || element != ignoreElement) && element.BlocksMouse && element.Visible;
        #endregion
    }
}
