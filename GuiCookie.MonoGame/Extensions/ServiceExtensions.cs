using GuiCookie.Core.Input;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Services;
using GuiCookie.Core.Styles;
using GuiCookie.MonoGame.Input;
using GuiCookie.MonoGame.Rendering;
using GuiCookie.MonoGame.Styles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GuiCookie.MonoGame.Extensions
{
    public static class ServiceExtensions
    {
        #region Functions
        public static UIServiceProvider AddMonoGameInput(this UIServiceProvider serviceProvider, GameWindow gameWindow)
        {
            MonoGameInputManager inputManager = new(gameWindow);
            serviceProvider.AddService(inputManager);
            serviceProvider.AddService<InputManager>(inputManager);
            return serviceProvider;
        }

        public static UIServiceProvider AddMonoGameWindow(this UIServiceProvider serviceProvider, GameWindow gameWindow)
        {
            MonoGameWindow window = new(gameWindow);
            serviceProvider.AddService(window);
            serviceProvider.AddService<Window>(window);
            return serviceProvider;
        }

        public static UIServiceProvider AddMonoGameResources(this UIServiceProvider serviceProvider, ContentManager contentManager)
        {
            MonoGameResourceManager resourceManager = new(contentManager);
            serviceProvider.AddService(resourceManager);
            serviceProvider.AddService<ResourceManager>(resourceManager);
            return serviceProvider;
        }
        #endregion
    }
}
