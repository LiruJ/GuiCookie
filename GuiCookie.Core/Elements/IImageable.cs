using GuiCookie.Core.Rendering;

namespace GuiCookie.Core.Elements
{
    public interface IImageable
    {
        Image? Image { get; set; }

        void SetImageFromName(string name);
    }
}
