using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Helpers
{
    public static class PointExtensions
    {
        public static Point Add(Point left, Point right) => new(left.X + right.X, left.Y + right.Y);

        public static Point Subtract(Point left, Point right) => new(left.X - right.X, left.Y - right.Y);

        public static Point Multiply(Point left, Point right) => new(left.X * right.X, left.Y * right.Y);

        public static Point Divide(Point left, Point right) => new(left.X / right.X, left.Y / right.Y);

        public static Vector2 ToVector2(this Point point) => new(point.X, point.Y);

        public static Vector2 ToVector2(this Size size) => new(size.Width, size.Height);
    }
}
