using GuiCookie.Core.DataStructures;
using Microsoft.Xna.Framework;

namespace GuiCookie.MonoGame.Extensions
{
    public static class GUIPointExtensions
    {
        public static Point ToPoint(this GUIPoint guiPoint) => new(guiPoint.X, guiPoint.Y);

        public static GUIPoint ToGUIPoint(this Point point) => new(point.X, point.Y);
    }
}
