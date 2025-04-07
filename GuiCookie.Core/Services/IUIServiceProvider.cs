

namespace GuiCookie.Core.Services
{
    public interface IUIServiceProvider : IServiceProvider
    {
        //object? GetService(Type serviceType);
        T? GetService<T>() where T : class;
        bool TryGetService<T>(out T? service) where T : class;
        IUIServiceProvider Clone();
        IEnumerator<(Type type, object service)> GetServicesEnumerator();
        IEnumerable<(Type type, object service)> GetServicesEnumerable();
    }
}