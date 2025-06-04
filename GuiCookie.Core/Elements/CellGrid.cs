using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.Helpers;
using LiruGameHelper.Signals;
using System.Drawing;

namespace GuiCookie.Core.Elements
{
    public class CellGrid : Element, IClickable
    {
        #region Components
        protected GridLayout cellLayout;
        protected MouseHandler mouseHandler;
        #endregion

        #region Signals
        public IConnectableSignal<Point> CellLeftClicked => cellLeftClicked;
        private readonly Signal<Point> cellLeftClicked = new();

        public IConnectableSignal<Point> CellRightClicked => cellRightClicked;
        private readonly Signal<Point> cellRightClicked = new();

        public IConnectableSignal LeftClicked => mouseHandler.LeftClicked;

        public IConnectableSignal RightClicked => mouseHandler.RightClicked;
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            cellLayout = GetComponent<GridLayout>();
            mouseHandler = GetComponent<MouseHandler>();

            //
            //  TODO: CORRECT THIS WITH SPACING.
            //

            // If both components were given, bind left click to select a cell.
            if (mouseHandler != null && cellLayout != null)
            {
                mouseHandler.LeftClicked.Connect(() => { if (Enabled) cellLeftClicked.Invoke(GridLayoutHelper.PixelToCellPosition(mouseHandler.RelativeMousePosition, cellLayout.CellSize)); });
                mouseHandler.RightClicked.Connect(() => { if (Enabled) cellRightClicked.Invoke(GridLayoutHelper.PixelToCellPosition(mouseHandler.RelativeMousePosition, cellLayout.CellSize)); });
            }
        }
        #endregion
    }
}
