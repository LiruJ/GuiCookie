using System.Drawing;

namespace GuiCookie.Core.Components
{
    public interface ISizeCalculator
    {
        Point DesiredSize { get; }

        void MakeDirty();
    }
}