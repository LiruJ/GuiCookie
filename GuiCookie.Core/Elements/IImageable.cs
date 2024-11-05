using GuiCookie.Core.Rendering;

namespace GuiCookie.Core.Elements
{
    public interface IImageable
    {
        Image Texture { get; set; }

        Image Image { get; set; }

        void SetImageFromName(string name);
    }
}
