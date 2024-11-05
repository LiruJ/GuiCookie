using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Rendering;
using GuiCookie.MonoGame.Extensions;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.MonoGame.Rendering
{
    public class MonoGameWindow : Window, IDisposable
    {
        #region Dependencies
        private readonly GameWindow gameWindow;
        #endregion

        #region Constructors
        public MonoGameWindow(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;

            gameWindow.ClientSizeChanged += onMonoGameWindowSizeChanged;
        }
        #endregion

        #region Event Functions
        private void onMonoGameWindowSizeChanged(object state, EventArgs e)
        {
            if (state is not GameWindow resizedWindow)
                return;

            // Invoke the event.
            GUIPoint newSize = resizedWindow.ClientBounds.Size.ToGUIPoint();
            onSizeChanged.Invoke(newSize);
        }
        #endregion

        #region Disposal Functions
        protected override void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
                gameWindow.ClientSizeChanged -= onMonoGameWindowSizeChanged;

            base.Dispose(disposing);
        }
        #endregion
    }
}
