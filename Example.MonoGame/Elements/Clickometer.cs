using GuiCookie.Core.Data;
using GuiCookie.Core.Elements;
using System;

namespace Example.MonoGame.Elements
{
    public class Clickometer : Element
    {
        #region Elements
        private Button button;

        private ProgressBar progressBar;
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup(IReadOnlyAttributeCollection attributes)
        {
            // Set the elements.
            button = GetChildByName<Button>("Button");
            progressBar = GetChildByName<ProgressBar>("ProgressBar");

            // Connect the button to make a bit of progress.
            button.ConnectLeftClick(() => progressBar.Value += 0.05f);
        }
        #endregion

        #region Update Functions
        protected override void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            // Decay the progress over time.
            if (progressBar.Value > 0) progressBar.Value -= MathF.Min(MathF.Pow(progressBar.Value + 0.15f, 2), 0.5f) * (float)elapsedTime.TotalSeconds;
        }
        #endregion
    }
}
