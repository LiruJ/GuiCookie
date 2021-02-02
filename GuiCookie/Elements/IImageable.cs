using GuiCookie.Styles;
using Microsoft.Xna.Framework.Graphics;

namespace GuiCookie.Elements
{
    public interface IImageable
    {
        Texture2D Texture { get; set; }

        Image Image { get; set; }

        void SetImageFromName(string name);
    }
}
