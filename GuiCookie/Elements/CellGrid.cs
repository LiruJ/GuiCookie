using GuiCookie.Components;
using LiruGameHelper.Signals;
using Microsoft.Xna.Framework;

namespace GuiCookie.Elements
{
    public class CellGrid : Element, IClickable
    {
        #region Components
        protected CellLayout cellLayout;
        protected MouseHandler mouseHandler;
        #endregion

        #region Signals
        public IConnectableSignal<Point> CellLeftClicked => cellLeftClicked;
        private readonly Signal<Point> cellLeftClicked = new Signal<Point>();

        public IConnectableSignal<Point> CellRightClicked => cellRightClicked;
        private readonly Signal<Point> cellRightClicked = new Signal<Point>();

        public IConnectableSignal LeftClicked => mouseHandler.LeftClicked;

        public IConnectableSignal RightClicked => mouseHandler.RightClicked;
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            cellLayout = GetComponent<CellLayout>();
            mouseHandler = GetComponent<MouseHandler>();

            // If a both components were given, bind left click to select a cell.
            if (mouseHandler != null && cellLayout != null)
            {
                mouseHandler.LeftClicked.Connect(() => { if (Enabled) cellLeftClicked.Invoke(cellLayout.RelativeToCellPosition(mouseHandler.RelativeMousePosition)); });
                mouseHandler.RightClicked.Connect(() => { if (Enabled) cellRightClicked.Invoke(cellLayout.RelativeToCellPosition(mouseHandler.RelativeMousePosition)); });
            }
        }
        #endregion
    }
}
