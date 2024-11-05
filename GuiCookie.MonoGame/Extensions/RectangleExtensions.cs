using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.MonoGame.Extensions
{
    public static class RectangleExtensions
    {
        public static Rectangle ToMonoGameRectangle(this System.Drawing.Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        public static System.Drawing.Rectangle ToDrawingRectangle(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        public static bool Contains(this System.Drawing.Rectangle rectangle, Point position) => rectangle.Contains(position.X, position.Y);

        public static bool Contains(this System.Drawing.Rectangle rectangle, Vector2 position) => rectangle.Contains((int)MathF.Floor(position.X), (int)MathF.Floor(position.Y));

        public static bool Contains(this System.Drawing.Rectangle rectangle, Rectangle other) => rectangle.Contains(other.ToDrawingRectangle());
    }
}
