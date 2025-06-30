using GuiCookie.Core.Rendering;

namespace GuiCookie.Core.Services
{
    public interface IDrawableUIService
    {
        int DrawOrder { get; }

        void Draw(IGuiCamera guiCamera);
    }
}
