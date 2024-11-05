using GuiCookie.Core;
using GuiCookie.MonoGame.Input;
using GuiCookie.MonoGame.Rendering;
using Microsoft.Xna.Framework;

namespace GuiCookie.MonoGame.Extensions
{
    public static class BuilderExtensions
    {
        #region Functions
        public static UIManagerBuilder WithMonoGameInput(this UIManagerBuilder builder, GameWindow gameWindow)
        {
            MonoGameInputManager inputManager = new(gameWindow);
            return builder.WithInputManager(inputManager);
        }

        public static UIManagerBuilder WithMonoGameWindow(this UIManagerBuilder builder, GameWindow gameWindow)
        {
            MonoGameWindow window = new(gameWindow);
            return builder.WithWindow(window);
        }
        #endregion
    }
}
