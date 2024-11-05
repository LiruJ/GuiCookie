using GuiCookie.Core.Services;
using LiruGameHelper.Signals;
using System.Drawing;

namespace GuiCookie.Core.Rendering
{
    public abstract class Window : IUIService, IDisposable
    {
        #region Properties
        protected bool disposedValue { get; private set; }

        #endregion

        #region Signals
        public IConnectableSignal<Point> OnSizeChanged => onSizeChanged;
        protected readonly Signal<Point> onSizeChanged = new();
        #endregion

        #region Constructors

        #endregion

        #region Functions
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                onSizeChanged.DisconnectAll();

            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}