
namespace GuiCookie.Core.Services
{
    public interface IUIServiceProvider
    {
        object? GetService(Type serviceType);
        T? GetService<T>() where T : class;
        bool TryGetService<T>(out T? service) where T : class;
    }
}