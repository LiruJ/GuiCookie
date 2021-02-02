using GuiCookie.Components;
using Microsoft.Xna.Framework;

namespace GuiCookie.Input.DragAndDrop
{
    public interface IDropTarget : IComponent
    {
        /// <summary> Is called every frame that the <paramref name="draggable"/> is being held over this <see cref="IDropTarget"/> but without being dropped. </summary>
        /// <param name="draggable"> The draggable that is being hovered. </param>
        /// <param name="relativeMousePosition"> The mouse position relative to this drop target's element. </param>
        void OnDraggableHovered(IDraggable draggable, Point relativeMousePosition);

        /// <summary>
        /// Is called when an <see cref="IDraggable"/> component is dragged and dropped within the bounds of this component's element.
        /// Note that this function should not change the parents/children of any of the elements, that can be done in <see cref="IDraggable.OnDragEnd(IDropTarget, Point)"/>, which is called directly after this function.
        /// </summary>
        /// <param name="draggable"> The draggable that has been dropped. </param>
        /// <param name="relativeMousePosition"> The mouse position relative to this drop target's element. </param>
        /// <returns> The target that should be given the <paramref name="draggable"/>. </returns>
        IDropTarget OnDraggableDropped(IDraggable draggable, Point relativeMousePosition);
    }
}