using GuiCookie.Elements;
using GuiCookie.Input.DragAndDrop;
using Microsoft.Xna.Framework;

namespace GuiCookie.Components
{
    /// <summary> The basic draggable component. Always returns itself through <see cref="IDraggable.OnDragBegin(Point)"/>, adds itself to the <see cref="ElementManager"/> or <see cref="IDropTarget"/> through <see cref="IDraggable.OnDragEnd(IDropTarget, Point)"/>. </summary>
    public class Draggable : Component, IDraggable
    {
        #region Dependencies
        private readonly ElementManager elementManager;
        private readonly DragAndDropManager dragAndDropManager;
        #endregion

        #region Constructors
        public Draggable(ElementManager elementManager, DragAndDropManager dragAndDropManager)
        {
            this.elementManager = elementManager ?? throw new System.ArgumentNullException(nameof(elementManager));
            this.dragAndDropManager = dragAndDropManager ?? throw new System.ArgumentNullException(nameof(dragAndDropManager));
        }
        #endregion

        #region Drag Functions
        public IDraggable OnDragBegin(Point relativeMousePosition, out Point newOffset) { newOffset = relativeMousePosition; return this; }

        public bool OnDragEnd(IDropTarget dropTarget, Point relativeMousePosition)
        {
            // If there was no drop target, move this element to the root.
            if (dropTarget == null) Bounds.AbsoluteTotalPosition = dragAndDropManager.DraggableOrigin;
            // Otherwise; add this element to the drop target.
            else dropTarget.Element.AddChild(Element);

            // Default behaviour is to always be droppable.
            return true;
        }
        #endregion
    }
}
