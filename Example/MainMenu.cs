﻿using Example.DataStructures;
using Example.Elements;
using GuiCookie;
using GuiCookie.Elements;
using System;

namespace Example
{
    public class MainMenu : Root
    {
        #region Dependencies
        private readonly Random random = null;
        #endregion

        #region Elements
        private ListPane testList = null;

        private TextButton disableButton = null;

        private TextButton targetButton = null;
        #endregion

        public MainMenu(Random random)
        {
            this.random = random;
        }

        protected override void PostInitialise()
        {
            testList = GetElementFromTag<ListPane>("List");

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
    }
}
