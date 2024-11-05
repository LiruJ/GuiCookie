namespace GuiCookie.Core.Services
{
    public class UIServiceProvider : IServiceProvider, IUIServiceProvider
    {
        #region Fields
        private readonly Dictionary<Type, object> services = [];
        #endregion

        #region Properties
        public IReadOnlyDictionary<Type, object> Services => services;
        #endregion

        #region Add Functions
        public bool TryAddService<T>(T service) where T : class => services.TryAdd(typeof(T), service);

        public void AddService<T>(T service) where T : class => services.Add(typeof(T), service);
        #endregion

        #region Get Functions
        public T? GetService<T>() where T : class => (T?)GetService(typeof(T));

        public object? GetService(Type serviceType) => services.GetValueOrDefault(serviceType);

        public bool TryGetService<T>(out T? service) where T : class
        {
            bool hasService = services.TryGetValue(typeof(T), out object? serviceObject) && serviceObject != null;
            service = (T?)(hasService ? serviceObject : null);
            return hasService;
        }
        #endregion
    }
}