using Microsoft.Xna.Framework;

namespace GuiCookie.Components
{
    public interface ISizeCalculator
    {
        Point DesiredSize { get; }

        void MakeDirty();
    }
}