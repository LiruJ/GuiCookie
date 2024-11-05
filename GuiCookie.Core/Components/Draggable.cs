using GuiCookie.Core.Input.DragAndDrop;
using System.Drawing;

namespace GuiCookie.Core.Components
{
    /// <summary> The basic draggable component. Always returns itself through <see cref="IDraggable.OnDragBegin(Point)"/>, adds itself to the <see cref="ElementManager"/> or <see cref="IDropTarget"/> through <see cref="IDraggable.OnDragEnd(IDropTarget, Point)"/>. </summary>
    public class Draggable(DragAndDropManager dragAndDropManager) : Component, IDraggable
    {
        #region Dependencies
        private readonly DragAndDropManager dragAndDropManager = dragAndDropManager ?? throw new ArgumentNullException(nameof(dragAndDropManager));
        #endregion

        #region Drag Functions
        public IDraggable OnDragBegin(Point relativeMousePosition, out Point newOffset) { newOffset = relativeMousePosition; return this; }

        public bool OnDragEnd(IDropTarget dropTarget, Point relativeMousePosition)
        {
            // If there was no drop target, move this element to the root.
            if (dropTarget == null)
                Bounds.AbsoluteTotalPosition = dragAndDropManager.DraggableOrigin;
            // Otherwise; add this element to the drop target.
            else 
                dropTarget.Element.AddChild(Element);

            // Default behaviour is to always be droppable.
            return true;
        }
        #endregion
    }
}
