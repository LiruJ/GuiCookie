using GuiCookie.Components;
using Microsoft.Xna.Framework;

namespace GuiCookie.Input.DragAndDrop
{
    public interface IDraggable : IComponent
    {
        /// <summary> Is called by the drag and drop manager when nothing is being dragged and the user starts dragging the element containing this component. </summary>
        /// <param name="relativeMousePosition"> The position of the mouse relative to this component's bounds. </param>
        /// <returns> The draggable to be set as the current within the drag and drop manager. </returns>
        IDraggable OnDragBegin(Point relativeMousePosition, out Point newOffset);

        /// <summary> Is called after <see cref="IDropTarget.OnDraggableDropped(IDraggable, Point)"/>, using its return value as the <paramref name="dropTarget"/> parameter. </summary>
        /// <param name="dropTarget"> The <see cref="IDropTarget"/> that this <see cref="IDraggable"/> was dropped onto. </param>
        /// <param name="relativeMousePosition"> The position of the mouse relative to the original <see cref="IDropTarget"/>. </param>
        /// <returns> True if the drop is successful; otherwise, false. If the drop was successful, the <see cref="DragAndDropManager.CurrentDraggable"/> is set to null. </returns>
        bool OnDragEnd(IDropTarget dropTarget, Point relativeMousePosition);
    }
}
