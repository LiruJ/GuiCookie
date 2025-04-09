using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Input;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Services;
using LiruGameHelper.Signals;
using System.Drawing;

namespace GuiCookie.Core.Roots
{
    /// <summary>
    /// The base class for a UI controller, inherit from this and create a new inherited class via the <see cref="UIManager"/> to create a custom UI controller.
    /// </summary>
    public class Root : IDisposable
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
        public Bounds Bounds { get; private set; }

        public ElementContainer Elements => Bounds.ElementContainer;

        /// <summary>
        /// Gets a value that is <c>true</c> when the mouse is over an element; otherwise, <c>false</c>.
        /// </summary>
        public bool IsMousedOver => ElementInputManager?.MousedOverElement != null;
        #endregion

        #region Constructors
        public Root(ElementManager elementManager, IUIServiceProvider serviceProvider)
        {
            ElementManager = elementManager;
            ElementInputManager = serviceProvider.GetService<ElementInputManager>();

            elementManager.onRootCreated(this);

            if (serviceProvider.TryGetService(out Window? window))
            {
                SignalConnection connection = window!.OnSizeChanged.Connect(onWindowResized);
                connections.Add(connection);
                Bounds = new(elementManager.RootElements!, window.Size);
            }
            else
                Bounds = new Bounds(elementManager.RootElements!, new Point(0, 0));

            foreach ((Type type, object service) in serviceProvider.GetServicesEnumerable())
            {
                if (service is IUpdateableUIService serviceUpdateableService)
                    updateableServices.Add(serviceUpdateableService);
            }
            updateableServices.Sort((left, right) => left.Order.CompareTo(right.Order));
        }
        #endregion

        #region Load Functions
        public void LoadLayout(SheetDataSource layoutSheetData)
        {
            if (Elements.Count != 0)
                throw new InvalidOperationException("Cannot load layout if the root has already been loaded! Reload instead.");

            ElementManager.LoadFromNode(layoutSheetData.RootNode);
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

        #endregion

        #region Disposable Functions
        ~Root()
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
