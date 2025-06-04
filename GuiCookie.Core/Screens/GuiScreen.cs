using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Input;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Services;
using LiruGameHelper.Signals;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;

namespace GuiCookie.Core.Screens
{
    public class GuiScreen : IDisposable
    {
        #region Dependencies
        public ElementManager ElementManager { get; }

        public ElementInputManager? ElementInputManager { get; }
        #endregion

        #region Fields
        private bool disposedValue;

        private readonly List<SignalConnection> connections = [];

        private readonly List<IUpdateableUIService> updateableServices = [];
        #endregion

        #region Properties
        /// <summary>
        /// The bounds of the game window.
        /// </summary>
        public Bounds Bounds => ElementManager.RootElement.Bounds;

        /// <summary>
        /// Gets a value that is <c>true</c> when the mouse is over an element; otherwise, <c>false</c>.
        /// </summary>
        public bool IsMousedOver => ElementInputManager?.MousedOverElement != null;
        #endregion

        #region Constructors
        public GuiScreen(ElementManager elementManager, IServiceProvider serviceProvider)
        {
            ElementManager = elementManager;
            ElementInputManager = serviceProvider.GetService<ElementInputManager>();

            Window? window = serviceProvider.GetService<Window>();
            if (window != null)
            {
                SignalConnection connection = window!.OnSizeChanged.Connect(onWindowResized);
                connections.Add(connection);

                elementManager.RootElement.Bounds.TotalSize = window.Size;
            }

            foreach (object? service in serviceProvider.GetServices(typeof(IUpdateableUIService)))
            {
                if (service is IUpdateableUIService serviceUpdateableService)
                    updateableServices.Add(serviceUpdateableService);
            }
            updateableServices.Sort((left, right) => left.Order.CompareTo(right.Order));
        }
        #endregion

        #region Screen Functions
        /// <summary>
        /// Is called when the window resizes, handles moving and resizing elements.
        /// </summary>
        /// <param name="newSize"> The new size of the window. </param>
        private void onWindowResized(Point newSize)
        {
            Bounds.TotalSize = newSize;

            foreach (Element element in ElementManager)
            {
                element.Bounds.recalculateSize();
                element.Bounds.recalculatePosition();
            }
        }
        #endregion

        #region Update Functions
        public void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            foreach (IUpdateableUIService service in updateableServices)
                service.PreUpdate(elapsedTime, totalTime);
            foreach (IUpdateableUIService service in updateableServices)
                service.Update(elapsedTime, totalTime);
            foreach (IUpdateableUIService service in updateableServices)
                service.PostUpdate(elapsedTime, totalTime);
        }
        #endregion

        #region Draw Functions
        public void Draw(IGuiCamera guiCamera)
        {
            ElementManager.RootElement.InternalDraw(guiCamera);
        }
        #endregion

        #region Disposable Functions
        ~GuiScreen()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {

            }

            foreach (SignalConnection connection in connections)
                connection.Disconnect();
            connections.Clear();

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
