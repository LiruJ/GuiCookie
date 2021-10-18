using GuiCookie;
using GuiCookie.Elements;

namespace Example
{
    public class MainMenu : Root
    {
        #region Elements
        private TextButton disableButton = null;

        private TextButton targetButton = null;
        #endregion

        #region Initialisation Functions
        protected override void PostInitialise()
        {
            disableButton = GetElementFromTag<TextButton>("DisableButton");
            targetButton = GetElementFromTag<TextButton>("TargetButton");

            disableButton.ConnectLeftClick(() =>
            {
                targetButton.Enabled = !targetButton.Enabled;
                
                if (targetButton.Enabled)
                {
                    targetButton.Text = "Enabled";
                    disableButton.Text = "Disable Button";
                }
                else
                {
                    targetButton.Text = "Disabled";
                    disableButton.Text = "Enable Button";
                }
            });
        }
        #endregion
    }
}