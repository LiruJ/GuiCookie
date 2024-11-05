using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GuiCookie.Core.Rendering
{
    public interface IGuiCamera
    {
        void DrawString(Font font, string text, Vector2 position, Color? colour = null);

        void DrawString(Font font, StringBuilder stringBuilder, Vector2 position, Color? colour = null);
    }
}
