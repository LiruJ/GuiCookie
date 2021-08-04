using GuiCookie.Components;
using GuiCookie.Elements;
using GuiCookie.Styles;
using GuiCookie.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Elements
{
    public class PortraitGrid : Element
    {
        #region Constants
        /// <summary> The number of portraits that exist in the sheet. </summary>
        private const int portraitCount = 82;

        /// <summary> The width/height of a portrait. </summary>
        private const int portraitDimension = 200;
        #endregion

        #region Elements
        private GridLayout grid = null;
        #endregion

        #region Fields
        /// <summary> The spritesheet that holds every single portrait. </summary>
        private readonly Texture2D portraitSheet = null;
        #endregion

        #region Constructors
        public PortraitGrid(ContentManager content)
        {
            portraitSheet = content.Load<Texture2D>("Portraits");
        }
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup()
        {
            grid = GetChildByName("Grid").GetComponent<GridLayout>();

            Template cellTemplate = Root.TemplateManager.GetTemplateFromName("ImageLabel");

            int x = 0, y = 0;
            for (int portraitIndex = 0; portraitIndex < portraitCount; portraitIndex++)
            {
                ImageBlock portraitImage = Root.ElementManager.CreateElementFromTemplate(cellTemplate, null, grid.Element).GetComponent<ImageBlock>();
                portraitImage.ClippingMode = GuiCookie.DataStructures.ClippingMode.Stretch;


                Rectangle source = new Rectangle(x * portraitDimension, y * portraitDimension, portraitDimension, portraitDimension);

                portraitImage.Image = new Image(portraitSheet, source);

                if ((x + 1) * portraitDimension >= portraitSheet.Width)
                {
                    x = 0;
                    y++;
                }
                else x++;
            }
        }
        #endregion
    }
}
