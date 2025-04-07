using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Input;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Services;
using System.Reflection;

namespace GuiCookie.Core
{
    public class UIManagerBuilder
    {
        #region Fields
        private readonly List<IUIService> uiServices = [];

        private readonly List<IUpdateableUIService> updateableUIServices = [];

        private readonly UIServiceProvider serviceProvider = new();
        #endregion

        #region With Functions
        public UIManagerBuilder WithInputManager(InputManager inputManager)
        {
            uiServices.Add(inputManager);
            updateableUIServices.Add(inputManager);

            serviceProvider.AddService(inputManager);

            return this;
        }

        public UIManagerBuilder WithRegisteredComponentNamespace(Assembly assembly, string namespacePath)
        {
            throw new NotImplementedException();
            return this;
        }

        public UIManagerBuilder WithWindow(Window window)
        {

            uiServices.Add(window);

            serviceProvider.AddService(window);

            return this;
        }
        #endregion

        #region Build Functions
        public UIManager Build()
        {

            return new(serviceProvider);
        }
        #endregion
    }
}
