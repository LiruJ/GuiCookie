using GuiCookie.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Helpers
{
    /// <summary> Helps with aligning elements to a grid. </summary>
    public static class GridLayoutHelper
    {
        #region Layout Functions
        /// <summary> Converts the given <paramref name="pixelPosition"/> so that it aligns with the grid defined by the given <paramref name="cellSize"/>. </summary>
        /// <param name="pixelPosition"> The grid position to adjust. </param>
        /// <param name="cellSize"> The size of a cell. </param>
        /// <returns> The snapped pixel position. </returns>
        public static Point SnapPixelPosition(Point pixelPosition, Point cellSize) => CellToPixelPosition(PixelToCellPosition(pixelPosition, cellSize), cellSize);

        /// <summary> Converts the given <paramref name="cellPosition"/> and returns a pixel position snapped to the grid defined by the given <paramref name="cellSize"/>. </summary>
        /// <param name="cellPosition"> The position of the cell. </param>
        /// <param name="cellSize"> The size of a cell. </param>
        /// <returns> The snapped pixel position. </returns>
        public static Point CellToPixelPosition(Vector2 cellPosition, Point cellSize) => CellToPixelPosition(Vector2.Floor(cellPosition).ToPoint(), cellSize);

        /// <summary> Converts the given <paramref name="cellPosition"/> into a pixel position that has been snapped to the grid defined by the given <paramref name="cellSize"/>. </summary>
        /// <param name="cellPosition"> The position of the cell. </param>
        /// <param name="cellSize"> The size of a cell. </param>
        /// <returns> The snapped pixel position. </returns>
        public static Point CellToPixelPosition(Point cellPosition, Point cellSize) => cellPosition * cellSize;

        /// <summary> Creates an absolute <see cref="Space"/> with a pixel position that has been snapped to the grid defined by the given <paramref name="cellSize"/>. </summary>
        /// <param name="cellPosition"> The position of the cell. </param>
        /// <param name="cellSize"> The size of a cell. </param>
        /// <returns> The snapped absolute position space. </returns>
        public static Space CellToPixelSpace(Point cellPosition, Point cellSize, Point? spacing = null) => new Space(CellToPixelPosition(cellPosition, cellSize + spacing ?? Point.Zero));

        /// <summary> Converts the given <paramref name="pixelPosition"/> into a cell position on the grid defined by the given <paramref name="cellSize"/>. </summary>
        /// <param name="pixelPosition"> The position of the pixel. </param>
        /// <param name="cellSize"> The size of a cell. </param>
        /// <returns> The converted cell position. </returns>
        public static Point PixelToCellPosition(Point pixelPosition, Point cellSize) => Vector2.Floor(pixelPosition.ToVector2() / cellSize.ToVector2()).ToPoint();

        /// <summary> Calculates the size in pixels from the given <paramref name="sizeInCells"/> in grid cells. </summary>
        /// <param name="sizeInCells"> The size in grid cells to adjust. </param>
        /// <param name="cellSize"> The size of a cell. </param>
        /// <returns> The adjusted size. </returns>
        public static Point AdjustSize(Point sizeInCells, Point cellSize) => sizeInCells * cellSize;

        /// <summary> Calculates the size in pixels from the given <paramref name="sizeInCells"/> in grid cells. </summary>
        /// <param name="sizeInCells"> The size in grid cells to adjust. </param>
        /// <param name="cellSize"> The size of a cell. </param>
        /// <returns> The adjusted size. </returns>
        public static Space AdjustSizeToSpace(Point sizeInCells, Point cellSize) => new Space(AdjustSize(sizeInCells, cellSize));

        public static void SnapToGrid(Point cellPosition, Point cellSize, Point spacing, Bounds bounds)
        {
            // Throw an exception if no bounds were given.
            if (bounds == null) throw new ArgumentNullException(nameof(bounds));

            // Set the position and size of the bounds.
            bounds.ScaledPosition = CellToPixelSpace(cellPosition, cellSize, spacing);
            bounds.ScaledSize = AdjustSizeToSpace(new Point(1), cellSize);
        }
        #endregion
    }
}
