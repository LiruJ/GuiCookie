using GuiCookie.Core.DataStructures;
using Ultraviolet;

namespace GuiCookie.Ultraviolet.Extensions
{
    public static class GUIPointExtensions
    {
        public static Point2 ToPoint2(this GUIPoint guiPoint) => new(guiPoint.X, guiPoint.Y);

        public static GUIPoint ToGUIPoint(this Point2 point) => new(point.X, point.Y);
    }
}
