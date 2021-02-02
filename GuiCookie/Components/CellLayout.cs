using GuiCookie.Attributes;
using GuiCookie.Elements;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
{
    /// <summary> Holds elements in a 2D array and maintains a grid layout. </summary>
    public class CellLayout : Component
    {
        #region Constants
        private const string rowsAttributeName = "Rows";
        private const string columnsAttributeName = "Columns";
        #endregion

        #region Dependencies
        private readonly ElementManager elementManager;
        #endregion

        #region Indexers
        public Element this[Point cellPosition]
        {
            get => this[cellPosition.X, cellPosition.Y];
            set => this[cellPosition.X, cellPosition.Y] = value;
        }

        public Element this[int x, int y]
        {
            get => IsInRange(x, y) ? elementGrid[x][y] : null;
            set
            {
                if (IsInRange(x, y))
                {
                    // Set the element.
                    elementGrid[x][y] = value;

                    // Put the element into the grid.
                    enforceToGrid(x, y, value);
                }
            }
        }
        #endregion

        #region Components
        private CellLayout cellLayout = null;
        #endregion

        #region Fields
        private Element[][] elementGrid;

        private Point dimensionSize;

        private AttributeCollection cellAttributes;

        private string cellTemplateName;
        #endregion

        #region Properties
        public Vector2 CellSize => Bounds.ContentSize.ToVector2() / DimensionSize.ToVector2();

        public int Columns { get => DimensionSize.X; set => DimensionSize = new Point(value, Rows); }

        public int Rows { get => DimensionSize.Y; set => DimensionSize = new Point(Columns, value); }

        public Point DimensionSize
        {
            get => dimensionSize;
            set
            {
                // Ensure the new size is valid.
                if (value.X < 1 || value.Y < 1) return;

                // Save the old size.
                Point oldSize = dimensionSize;

                // Set the dimension size.
                dimensionSize = value;

                // Ensure a change will be made.
                //if (dimensionSize == value) return;

                // Before the arrays are resized, destroy any children who are out of bounds. This will skip if the width is going up.
                for (int x = value.X; x < oldSize.X; x++)
                {
                    for (int y = 0; y < oldSize.Y; y++)
                    {
                        this[x, y].Destroy();
                        this[x, y] = null;
                    }
                }

                // If the new height is smaller than the old height, destroy any children that are out of bounds.
                if (value.Y < oldSize.Y)
                    // Go over the area of the new size, as the previous step will have already destroyed any that were out of the width.
                    for (int x = 0; x < Math.Min(oldSize.X, value.X); x++)
                        for (int y = 0; y < oldSize.Y; y++)
                        {
                            this[x, y].Destroy();
                            this[x, y] = null;
                        }

                // Resize the array, and repopulate any new grid cells.
                Array.Resize(ref elementGrid, value.X);
                for (int x = 0; x < value.X; x++)
                    Array.Resize(ref elementGrid[x], value.Y);

                repopulate(oldSize, value);
            }
        }
        #endregion

        #region Constructors
        public CellLayout(ElementManager elementManager)
        {
            this.elementManager = elementManager;
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            int columns = Element.Attributes.GetAttributeOrDefault(columnsAttributeName, 0);
            int rows = Element.Attributes.GetAttributeOrDefault(rowsAttributeName, 0);

            dimensionSize = new Point(columns, rows);
        }

        public override void OnSetup()
        {
            cellLayout = Element.GetComponent<CellLayout>() ?? throw new Exception("Cell layout component is missing cell grid component.");
        }
        #endregion

        #region Range Functions
        public bool IsInRange(Point cellPosition) => IsInRange(cellPosition.X, cellPosition.Y);

        public bool IsInRange(int x, int y) => x >= 0 && x < Columns && y >= 0 && y < Rows;
        #endregion

        #region Cell Functions
        public T At<T>(Point cellPosition) where T : Element => this[cellPosition] as T;

        public T At<T>(int x, int y) where T : Element => this[x, y] as T;

        public void Populate(AttributeCollection cellAttributes, string cellTemplateName)
        {
            if (this.cellAttributes != null || !string.IsNullOrWhiteSpace(this.cellTemplateName)) throw new Exception("Can only populate once.");
            this.cellAttributes = cellAttributes;
            this.cellTemplateName = cellTemplateName;

            // Set the size to itself, calling the set function which resizes the internal arrays.
            DimensionSize = dimensionSize;

            // Go over each cell.
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    // If nothing is in this cell, create a new element with the given cell attributes.
                    if (this[x, y] == null) this[x, y] = elementManager.CreateElementFromTemplateName(cellTemplateName, cellAttributes, Element);
        }

        private void repopulate(Point oldSize, Point newSize)
        {
            // If the new size is smaller than the old size, nothing needs to be done.
            if (newSize.X <= oldSize.X && newSize.Y <= oldSize.Y) return;

            // Go over each cell of the new size and populate any new ones.
            for (int x = 0; x < newSize.X; x++)
                for (int y = 0; y < newSize.Y; y++)
                    if (this[x, y] == null)
                        this[x, y] = elementManager.CreateElementFromTemplateName(cellTemplateName, cellAttributes, Element);
                    else enforceToGrid(x, y, this[x, y]);
        }

        private void enforceToGrid(int x, int y, Element element)
        {
            // Set the position of the element to align it to the grid.
            element.Bounds.ScaledPosition = new DataStructures.Space((float)x / Columns, (float)y / Rows, DataStructures.Axes.Both);

            // Change the size of the element to fit into the grid.
            element.Bounds.ScaledSize = new DataStructures.Space(1.0f / Columns, 1.0f / Rows, DataStructures.Axes.Both);
        }

        public Point RelativeToCellPosition(Point relativePosition)
        {
            // Get the floating point position.
            Vector2 floatingPosition = relativePosition.ToVector2() / CellSize;
            
            // Return the rounded position.
            return new Point((int)Math.Floor(floatingPosition.X), (int)Math.Floor(floatingPosition.Y));
        }

        public Point AbsoluteToCellPosition(Point absolutePosition) => RelativeToCellPosition(absolutePosition - Element.Bounds.AbsoluteTotalPosition);
        #endregion
    }
}