using GuiCookie.Core.DataStructures;
using Microsoft.Xna.Framework;

namespace GuiCookie.MonoGame.Extensions
{
    public static class PointExtensions
    {
        public static Point ToMonoGamePoint(this System.Drawing.Point point) => new(point.X, point.Y);

        public static System.Drawing.Point ToDrawingPoint(this Point point) => new(point.X, point.Y);
    }
}
