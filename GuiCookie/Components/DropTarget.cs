using GuiCookie.Input.DragAndDrop;
using Microsoft.Xna.Framework;

namespace GuiCookie.Components
{
    /// <summary> The basic drop target component. Always returns itself through the <see cref="IDropTarget.OnDraggableDropped(IDraggable, Point)"/> function. </summary>
    public class DropTarget : Component, IDropTarget
    {
        public IDropTarget OnDraggableDropped(IDraggable draggable, Point relativeMousePosition) => this;

        public void OnDraggableHovered(IDraggable draggable, Point relativeMousePosition) { }
    }
}
