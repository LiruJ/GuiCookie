using GuiCookie.Elements;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Input.DragAndDrop
{
    public class DragAndDropManager
    {
        #region Dependencies
        private readonly InputManager inputManager;
        #endregion

        #region Properties
        /// <summary> The current <see cref="IDraggable"/> that is being dragged. </summary>
        public IDraggable CurrentDraggable { get; private set; }

        /// <summary> The position of the mouse relative to the <see cref="CurrentDraggable"/> when it was first dragged. </summary>
        public Point DraggableOffset { get; private set; }

        /// <summary> The absolute total position of the <see cref="CurrentDraggable"/> when it was first dragged. </summary>
        public Point DraggableOrigin { get; private set; }
        #endregion

        #region Constructors
        public DragAndDropManager(InputManager inputManager)
        {
            this.inputManager = inputManager ?? throw new ArgumentNullException(nameof(inputManager));
        }
        #endregion

        #region Dragging Functions
        /// <summary> 
        /// Begins dragging the given <paramref name="draggable"/> so long as the <see cref="CurrentDraggable"/> is null. Uses the given <paramref name="offset"/> when keeping the draggable pinned to the mouse. 
        /// Calls <see cref="IDraggable.OnDragBegin(Point)"/> with the given <paramref name="offset"/> and sets the <see cref="CurrentDraggable"/> to its return value.
        /// </summary>
        /// <param name="draggable"></param>
        /// <param name="offset"></param>
        public bool BeginDragging(IDraggable draggable, Point offset)
        {
            // Return false if the state is invalid.
            if (draggable == null || CurrentDraggable != null) return false;

            // Begin dragging the given draggable.
            IDraggable newDraggable = draggable.OnDragBegin(offset, out Point newOffset);

            // If the given draggable returned a null value, return false.
            if (newDraggable == null) return false;

            // Set the current draggable and offset.
            CurrentDraggable = newDraggable;
            DraggableOffset = newOffset;
            DraggableOrigin = CurrentDraggable.Bounds.AbsoluteTotalPosition;

            // Return true as the dragging was begun.
            return true;
        }

        /// <summary> Stops dragging the <see cref="CurrentDraggable"/>, attempting to drop it into the given <paramref name="dropTarget"/>. </summary>
        /// <param name="dropTarget">  </param>
        /// <param name="offset">  </param>
        public bool StopDragging(IDropTarget dropTarget, Point offset)
        {
            // Return false if the state is invalid.
            if (CurrentDraggable == null) return false;

            // Tell the given drop target that a draggable was dropped on it, keep the drop target it returns. If the given drop target is null, default to null.
            IDropTarget resolvedDropTarget = dropTarget?.OnDraggableDropped(CurrentDraggable, offset);

            // Tell the current draggable that it is being dropped onto the resolved target, if it rejects the drop then return false.
            if (!CurrentDraggable.OnDragEnd(resolvedDropTarget, offset)) return false;

            // Unset the current draggable as it has now been dropped.
            CurrentDraggable = null;
            DraggableOffset = Point.Zero;
            DraggableOrigin = Point.Zero;

            // Return true as the dropping was successful.
            return true;
        }
        #endregion

        #region Update Functions
        public void Update(ElementManager elementManager)
        {
            // Handle dragging logic based on if there's currently an element being dragged.
            if (CurrentDraggable != null)
            {
                // Get the DropTarget under the mouse, ignoring the currently dragged item.
                IDropTarget dropTarget = inputManager.FindInBoundsWithInterfacedComponent<IDropTarget>(elementManager, CurrentDraggable.Bounds.AbsoluteContentArea, CurrentDraggable.Element);

                // Calculate the offset relative to the drop target.
                Point relativeMousePosition = dropTarget == null ? Point.Zero : inputManager.MousePosition - dropTarget.Bounds.AbsoluteTotalPosition;

                // If the mouse is now up, drop the current draggable.
                if (inputManager.IsLeftMouseUp)
                    StopDragging(dropTarget, relativeMousePosition);
                // Otherwise; keep the dragged element pinned to the mouse.
                else
                {
                    // Move the element.
                    CurrentDraggable.Bounds.AbsoluteTotalPosition = inputManager.MousePosition - DraggableOffset;

                    // If there's a drop target under the mouse, tell it that it's being hovered over.
                    if (dropTarget != null) dropTarget.OnDraggableHovered(CurrentDraggable, relativeMousePosition);
                }
            }
            // Otherwise; if the draggable is null but the mouse is down, check to see if a draggable can be dragged.
            else if (inputManager.IsLeftMouseDown && !inputManager.WasLeftMouseDown)
            {
                // Find the draggable object under the mouse
                IDraggable draggable = inputManager.FindInBoundsWithInterfacedComponent<IDraggable>(elementManager, new Rectangle(inputManager.MousePosition, Point.Zero));

                // If there is no draggable object, do nothing.
                if (draggable == null) return;

                // Calculate the position of the mouse relative to the element.
                Point relativeMousePosition = inputManager.MousePosition - draggable.Bounds.AbsoluteTotalPosition;

                // Begin dragging the draggable.
                BeginDragging(draggable, relativeMousePosition);
            }
        }
        #endregion
    }
}