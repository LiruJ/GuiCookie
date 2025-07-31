using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Resources;
using GuiCookie.Core.Templates;
using GuiCookie.MonoGame.Rendering;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace Example.MonoGame.Elements
{
    public class PortraitGrid(TemplateManager templateManager, ElementManager elementManager, ResourceManager resourceManager, ContentManager contentManager) : Element
    {
        #region Constants
        /// <summary> The number of portraits that exist in the sheet. </summary>
        private const int portraitCount = 82;

        /// <summary> The width/height of a portrait. </summary>
        private const int portraitDimension = 200;
        #endregion

        #region Elements
        private GridLayout grid = null;

        private LabelledSlider xSpaceSlider = null;

        private LabelledSlider ySpaceSlider = null;
        #endregion

        #region Fields
        /// <summary> The spritesheet that holds every single portrait. </summary>
        private Texture2D portraitSheet;
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // TODO: Have some way of loading images that aren't defined as resources, so this isn't MonoGame specific.
            portraitSheet = contentManager.Load<Texture2D>("Gui/Images/Portraits");
        }
        public override void OnPostFullSetup()
        {
            grid = GetChildByName("Grid").GetComponent<GridLayout>();
            xSpaceSlider = GetChildByName<LabelledSlider>("XSpaceSlider");
            ySpaceSlider = GetChildByName<LabelledSlider>("YSpaceSlider");


            Template cellTemplate = templateManager.GetTemplateFromName("ImageLabel");

            int x = 0, y = 0;
            for (int portraitIndex = 0; portraitIndex < portraitCount; portraitIndex++)
            {
                ImageBlock portraitImage = elementManager.CreateElementFromTemplate(cellTemplate, null, grid.Element).GetComponent<ImageBlock>();
                portraitImage.ClippingMode = ClippingMode.Stretch;

                Rectangle source = new(x * portraitDimension, y * portraitDimension, portraitDimension, portraitDimension);

                portraitImage.Image = new MonoGameImage(portraitSheet, source);

                if ((x + 1) * portraitDimension >= portraitSheet.Width)
                {
                    x = 0;
                    y++;
                }
                else x++;
            }

            grid.Spacing = new Point((int)xSpaceSlider.Slider.Value, (int)ySpaceSlider.Slider.Value);

            xSpaceSlider?.Slider?.ConnectValueChanged(() => grid.Spacing = new Point((int)xSpaceSlider.Slider.Value, grid.Spacing.Y));
            ySpaceSlider?.Slider?.ConnectValueChanged(() => grid.Spacing = new Point(grid.Spacing.X, (int)ySpaceSlider.Slider.Value));
        }
        #endregion
    }
}
