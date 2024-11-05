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

        private bool hasBuilt = false;
        #endregion

        #region With Functions
        public UIManagerBuilder WithInputManager(InputManager inputManager)
        {
            assertHasNotBuilt();

            uiServices.Add(inputManager);
            updateableUIServices.Add(inputManager);

            serviceProvider.AddService(inputManager);

            return this;
        }

        public UIManagerBuilder WithRegisteredComponentNamespace(Assembly assembly, string namespacePath)
        {
            assertHasNotBuilt();

            throw new NotImplementedException();
            return this;
        }

        public UIManagerBuilder WithWindow(Window window)
        {
            assertHasNotBuilt();

            uiServices.Add(window);

            serviceProvider.AddService(window);

            return this;
        }
        #endregion

        #region Build Functions
        public UIManager Build()
        {
            assertHasNotBuilt();

            hasBuilt = true;

            return new(serviceProvider);
        }
        
        private void assertHasNotBuilt()
        {
            if (hasBuilt)
                throw new Exception("Cannot build twice from the same builder!");
        }
        #endregion
    }
}
