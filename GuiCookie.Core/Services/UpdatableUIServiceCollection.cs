using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace GuiCookie.Core.Services
{
    public class UpdatableUIServiceCollection : IEnumerable<IUpdatableUIService>
    {
        #region Backing Fields
        private readonly List<IUpdatableUIService> updatableServices = [];
        #endregion

        #region Properties
        public IReadOnlyList<IUpdatableUIService> UpdatableServices => updatableServices;
        #endregion

        #region Constructors
        private UpdatableUIServiceCollection(List<IUpdatableUIService> updatableServices)
        {
            this.updatableServices = updatableServices;
            this.updatableServices.Sort((left, right) => left.Order.CompareTo(right.Order));
        }
        #endregion

        #region Enumerators
        public IEnumerator<IUpdatableUIService> GetEnumerator() 
            => ((IEnumerable<IUpdatableUIService>)updatableServices).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)updatableServices).GetEnumerator();
        #endregion

        #region Service Extensions
        public static void AddToServices(ServiceCollection services) 
            => services.AddSingleton((sp) => createFromServices(services, sp));

        private static UpdatableUIServiceCollection createFromServices(ServiceCollection services, IServiceProvider serviceProvider)
        {
            List<IUpdatableUIService> updatableServices = [];
            foreach (var item in services)
                if (item.ServiceType.IsAssignableTo(typeof(IUpdatableUIService))
                    && serviceProvider.GetService(item.ServiceType) is IUpdatableUIService updatableService
                    && !updatableServices.Contains(updatableService))
                    updatableServices.Add(updatableService);
               

            return new(updatableServices);
        }
        #endregion
    }
}
