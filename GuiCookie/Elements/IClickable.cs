using LiruGameHelper.Signals;

namespace GuiCookie.Elements
{
    public interface IClickable
    {
        IConnectableSignal LeftClicked { get; }
        IConnectableSignal RightClicked { get; }
    }
}
