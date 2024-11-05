using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Helpers
{
    public static class VectorExtensions
    {
        public static Vector2 Floor(this Vector2 value) => new(MathF.Floor(value.X), MathF.Floor(value.Y));

        public static Point ToPoint(this Vector2 value) => new((int)value.X, (int)value.Y);

        public static Size ToSize(this Vector2 value) => new((int)value.X, (int)value.Y);
    }
}
