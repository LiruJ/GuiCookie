using GuiCookie.Core.Input;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Resources;
using GuiCookie.MonoGame.Input;
using GuiCookie.MonoGame.Rendering;
using GuiCookie.MonoGame.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GuiCookie.MonoGame.Extensions
{
    public static class ServiceExtensions
    {
        #region Functions
        public static ServiceCollection AddMonoGameInput(this ServiceCollection services, GameWindow gameWindow)
        {
            MonoGameInputManager inputManager = new(gameWindow);
            services.AddSingleton(inputManager);
            services.AddSingleton<InputManager>(inputManager);
            return services;
        }

        public static ServiceCollection AddMonoGameWindow(this ServiceCollection services, GameWindow gameWindow)
        {
            MonoGameWindow window = new(gameWindow);
            services.AddSingleton<Window>(window);
            services.AddSingleton(window);
            return services;
        }

        public static ServiceCollection AddMonoGameResources(this ServiceCollection services, ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            MonoGameResourceManager resourceManager = new(contentManager, graphicsDevice);
            services.AddSingleton<ResourceManager>(resourceManager);
            services.AddSingleton(resourceManager);
            return services;
        }
        #endregion
    }
}
