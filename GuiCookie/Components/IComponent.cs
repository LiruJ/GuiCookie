using GuiCookie.DataStructures;
using GuiCookie.Elements;

namespace GuiCookie.Components
{
    public interface IComponent
    {
        /// <summary> The element of the component. </summary>
        Element Element { get; }

        /// <summary> The bounds of the component's element. </summary>
        Bounds Bounds { get; }
    }
}
