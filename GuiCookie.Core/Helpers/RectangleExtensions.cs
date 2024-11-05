using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Helpers
{
    public static class RectangleExtensions
    {
        public static Point GetCentre(this Rectangle rectangle) => new (rectangle.Left + (rectangle.Width / 2), rectangle.Top + (rectangle.Height / 2));

        public static Vector2 GetCentreF(this Rectangle rectangle) => new (rectangle.Left + (rectangle.Width / 2f), rectangle.Top + (rectangle.Height / 2f));
    }
}
