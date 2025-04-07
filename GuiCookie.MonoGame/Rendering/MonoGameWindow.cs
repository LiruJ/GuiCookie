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

        #region Properties
        public override System.Drawing.Point Size => gameWindow.ClientBounds.Size.ToDrawingPoint();
        #endregion

        #region Constructors
        public MonoGameWindow(GameWindow gameWindow)
        {
            this.gameWindow = gameWindow;

            gameWindow.ClientSizeChanged += onMonoGameWindowSizeChanged;
        }
        #endregion

        #region Event Functions
        private void onMonoGameWindowSizeChanged(object? state, EventArgs e)
        {
            if (state is not GameWindow resizedWindow)
                return;

            // Invoke the event.
            onSizeChanged.Invoke(resizedWindow.ClientBounds.Size.ToDrawingPoint());
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
