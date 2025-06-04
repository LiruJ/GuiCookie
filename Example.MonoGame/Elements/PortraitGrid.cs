using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Templates;
using GuiCookie.MonoGame.Extensions;
using GuiCookie.MonoGame.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Example.MonoGame.Elements
{
    public class PortraitGrid(TemplateManager templateManager) : Element
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
        //private readonly Texture2D portraitSheet = content.Load<Texture2D>("Portraits");
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup(IReadOnlyAttributeCollection attributes)
        {
            grid = GetChildByName("Grid").GetComponent<GridLayout>();
            xSpaceSlider = GetChildByName<LabelledSlider>("XSpaceSlider");
            ySpaceSlider = GetChildByName<LabelledSlider>("YSpaceSlider");


            Template cellTemplate = templateManager.GetTemplateFromName("ImageLabel");

            //int x = 0, y = 0;
            //for (int portraitIndex = 0; portraitIndex < portraitCount; portraitIndex++)
            //{
            //    ImageBlock portraitImage = Root.ElementManager.CreateElementFromTemplate(cellTemplate, null, grid.Element).GetComponent<ImageBlock>();
            //    portraitImage.ClippingMode = ClippingMode.Stretch;

            //    Rectangle source = new(x * portraitDimension, y * portraitDimension, portraitDimension, portraitDimension);

            //    portraitImage.Image = new MonoGameImage(portraitSheet, source.ToDrawingRectangle());

            //    if ((x + 1) * portraitDimension >= portraitSheet.Width)
            //    {
            //        x = 0;
            //        y++;
            //    }
            //    else x++;
            //}
        }

        public override void OnPostFullSetup()
        {
            grid.Spacing = new System.Drawing.Point((int)xSpaceSlider.Slider.Value, (int)ySpaceSlider.Slider.Value);

            xSpaceSlider?.Slider?.ConnectValueChanged(() => grid.Spacing = new System.Drawing.Point((int)xSpaceSlider.Slider.Value, grid.Spacing.Y));
            ySpaceSlider?.Slider?.ConnectValueChanged(() => grid.Spacing = new System.Drawing.Point(grid.Spacing.X, (int)ySpaceSlider.Slider.Value));

        }
        #endregion
    }
}
