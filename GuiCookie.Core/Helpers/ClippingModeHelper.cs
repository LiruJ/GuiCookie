using GuiCookie.Core.DataStructures;
using System.Drawing;

namespace GuiCookie.Core.Helpers
{
    public static class ClippingModeHelper
    {
        public static void CalculateClippingValues(ClippingMode clippingMode, Point position, Size size,
                                                   Rectangle imageSource, bool isCentred, out Rectangle source,
                                                   out Rectangle destination, out Point offset, out float scale)
        {
            source = imageSource;
            scale = 1;
            offset = isCentred ? (size.ToVector2() / 2.0f - imageSource.Size.ToVector2() / 2.0f).ToPoint() : new Point(0, 0);

            switch (clippingMode)
            {
                case ClippingMode.None:
                    destination = new Rectangle(PointExtensions.Add(position, offset), imageSource.Size);
                    break;
                case ClippingMode.Clip:
                    // The source rectangle is the intersection between the desired source size and the actual content size.
                    source = Rectangle.Intersect(imageSource, new Rectangle(PointExtensions.Subtract(imageSource.Location, offset), size));

                    // A new offset has to be calculated with the new source size.
                    offset = isCentred ? ((size.ToVector2() / 2.0f) - (source.Size.ToVector2() / 2.0f)).ToPoint() : new Point(0, 0);

                    destination = new Rectangle(PointExtensions.Add(position, offset), source.Size);
                    break;
                case ClippingMode.Squeeze:
                    Point fittedSize = new(Math.Min(imageSource.Width, size.Width), Math.Min(imageSource.Height, size.Height));

                    float ratioX = (float)fittedSize.X / imageSource.Width;
                    float ratioY = (float)fittedSize.Y / imageSource.Height;

                    Size squeezedSize = ratioX < ratioY ? new Size(fittedSize.X, (int)(imageSource.Height * ratioX)) : new Size((int)(imageSource.Width * ratioY), fittedSize.Y);

                    // Set the scale.
                    scale = ratioX < ratioY ? ratioX : ratioY;

                    // A new offset has to be calculated with the squeezed size.
                    offset = isCentred ? ((size.ToVector2() / 2.0f) - (squeezedSize.ToVector2() / 2.0f)).ToPoint() : new Point(0, 0);

                    destination = new Rectangle(PointExtensions.Add(position, offset), squeezedSize);
                    break;
                case ClippingMode.Stretch:
                    destination = new Rectangle(position, size);
                    offset = new Point(0, 0);
                    break;
                default:
                    throw new Exception("Invalid clipping mode.");
            }
        }
    }
}
