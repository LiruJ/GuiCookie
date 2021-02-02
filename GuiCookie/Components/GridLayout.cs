using GuiCookie.DataStructures;
using GuiCookie.Elements;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
{
    /// <summary> Helps with aligning elements to a grid, but does not do so itself. Also does not have any dimensional data, meaning that the grid can extend infinitely. See <see cref="CellLayout"/> for such behaviour. </summary>
    public class GridLayout : Component
    {
        #region Properties
        /// <summary> The size of a grid cell in pixels. </summary>
        public Point CellSize { get; set; }
        #endregion

        #region Initialisation Functions
        public override void OnSetup()
        {
            // Bind the child added/removed signals to recalculate the layout.
            Element.OnChildAdded.Connect((_) => { recalculateLayout(); });
            Element.OnChildRemoved.Connect((_) => { recalculateLayout(); });

            // Recalculate the children to start with.
            recalculateLayout();
        }
        #endregion

        #region Style Functions
        public override void OnSizeChanged()
        {
            recalculateLayout();
        }
        #endregion

        #region Layout Functions
        /// <summary> Adjusts the given pixel position (relative to this component's element) so that it aligns with the grid. </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Point AdjustPosition(Point position)
        {
            Vector2 gridCellPosition = position.ToVector2() / CellSize.ToVector2();
            return new Point((int)Math.Floor(gridCellPosition.X), (int)Math.Floor(gridCellPosition.Y)) * CellSize;
        }

        /// <summary> Calculates the size in pixels from the given <paramref name="size"/> in grid cells. </summary>
        /// <param name="size"> The size in grid cells to adjust. </param>
        /// <returns> The adjusted size. </returns>
        public Point AdjustSize(Point size) => size * CellSize;

        private void recalculateLayout()
        {
            // The current width and height of the calculated row and column.
            float currentWidth = 0;
            float currentHeight = 0;

            // The height of the current row.
            float rowHeight = 0;

            // Go over each child element in the parent element.
            foreach (Element childElement in Element)
            {
                // Reposition the current child element.
                childElement.Bounds.ScaledPosition = new Space(currentWidth, currentHeight, Axes.None);

                // Add the width of the just added child to the current width.
                currentWidth += childElement.Bounds.TotalSize.X;

                // If the height of the just added child is greater than the height of the current row, save it.
                rowHeight = Math.Max(rowHeight, childElement.Bounds.TotalSize.Y);

                // If the width of the current row is greater than the size of the containing element, start on the next row.
                if (currentWidth >= Element.Bounds.ContentSize.X)
                {
                    // Add the row height to the current height.
                    currentHeight += rowHeight;

                    // Reset the current width and row height.
                    currentWidth = 0;
                    rowHeight = 0;
                }
            }
        }
        #endregion
    }
}
