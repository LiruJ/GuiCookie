using LiruGameHelper.Signals;

namespace GuiCookie.Core.Elements
{
    public interface IClickable
    {
        IConnectableSignal LeftClicked { get; }
        IConnectableSignal RightClicked { get; }
    }
}
