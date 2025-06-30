using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace GuiCookie.Core.Services
{
    public class DrawableUIServiceCollection : IEnumerable<IDrawableUIService>
    {
        #region Backing Fields
        private readonly List<IDrawableUIService> drawableServices = [];
        #endregion

        #region Properties
        public IReadOnlyList<IDrawableUIService> DrawableServices => drawableServices;
        #endregion

        #region Constructors
        private DrawableUIServiceCollection(List<IDrawableUIService> drawableServices)
        {
            this.drawableServices = drawableServices;
            this.drawableServices.Sort((left, right) => left.DrawOrder.CompareTo(right.DrawOrder));
        }
        #endregion

        #region Enumerators
        public IEnumerator<IDrawableUIService> GetEnumerator() 
            => ((IEnumerable<IDrawableUIService>)drawableServices).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)drawableServices).GetEnumerator();
        #endregion

        #region Service Extensions
        public static void AddToServices(ServiceCollection services) 
            => services.AddSingleton((sp) => createFromServices(services, sp));

        private static DrawableUIServiceCollection createFromServices(ServiceCollection services, IServiceProvider serviceProvider)
        {
            List<IDrawableUIService> drawableServices = [];
            foreach (var item in services)
                if (item.ServiceType.IsAssignableTo(typeof(IDrawableUIService))
                    && serviceProvider.GetService(item.ServiceType) is IDrawableUIService drawableService
                    && !drawableServices.Contains(drawableService))
                    drawableServices.Add(drawableService);
            return new(drawableServices);
        }
        #endregion
    }
}
