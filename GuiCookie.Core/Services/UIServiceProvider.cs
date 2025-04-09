namespace GuiCookie.Core.Services
{
    public class UIServiceProvider : IUIServiceProvider
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

        #region Helper Functions
        public UIServiceProvider AddSelf()
        {
            services.Add(typeof(IUIServiceProvider), this);
            services.Add(typeof(UIServiceProvider), this);
            services.Add(typeof(IServiceProvider), this);
            return this;
        }

        public void RemoveSelf()
        {
            services.Remove(typeof(IUIServiceProvider));
            services.Remove(typeof(UIServiceProvider));
            services.Remove(typeof(IServiceProvider));
        }
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

        public IEnumerator<(Type type, object service)> GetServicesEnumerator()
            => services.Select(x => (x.Key, x.Value)).GetEnumerator();

        public IEnumerable<(Type type, object service)> GetServicesEnumerable()
            => services.Select(x => (x.Key, x.Value)).AsEnumerable();
        #endregion

        #region Clone Functions
        public IUIServiceProvider Clone()
        {
            UIServiceProvider clone = new();
            foreach (var service in services)
                clone.services.Add(service.Key, service.Value);
            return clone;
        }

        public static UIServiceProvider Clone(IUIServiceProvider serviceProvider)
        {
            UIServiceProvider clone = new();
            foreach ((Type type, object service) in serviceProvider.GetServicesEnumerable())
                clone.services.Add(type, service);
            return clone;
        }
        #endregion
    }
}