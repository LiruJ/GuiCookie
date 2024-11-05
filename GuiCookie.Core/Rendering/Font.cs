using System.Numerics;
using System.Text;

namespace GuiCookie.Core.Rendering
{
    public abstract class Font
    {
        public abstract Vector2 MeasureString(string text);

        public abstract Vector2 MeasureString(StringBuilder stringBuilder);
    }
}
