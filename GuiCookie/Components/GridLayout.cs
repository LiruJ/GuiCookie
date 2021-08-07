using GuiCookie.DataStructures;
using GuiCookie.Elements;
using GuiCookie.Helpers;
using GuiCookie.Rendering;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
{
    /// <summary> Holds elements in a 2D array and maintains a grid layout. </summary>
    public class GridLayout : Component
    {
        #region Constants
        private const string cellSizeAttributeName = "CellSize";

        private const string gridSizeAttributeName = "GridSize";

        private const string spacingAttributeName = "Spacing";
        #endregion

        #region Dependencies
        private readonly ElementManager elementManager;
        #endregion

        #region Fields
        /// <summary> Is <c>true</c> if the layout needs to recalculate next frame, otherwise; <c>false</c>. </summary>
        private bool isDirty = true;
        #endregion

        #region Backing Fields
        private Point? cellSize = null;

        private Point? gridSize = null;

        private Point cachedGridSize = Point.Zero;

        private Point spacing = Point.Zero;
        #endregion

        #region Properties
        /// <summary> The size of a grid cell in pixels. </summary>
        public Point CellSize
        {
            get => cellSize ?? Vector2.Floor(Bounds.ContentSize.ToVector2() / GridSize.ToVector2()).ToPoint();
            set
            {
                // If either axis of the value is invalid, do nothing.
                if (value.X <= 0 || value.Y <= 0) return;

                // If the value is the same as the current value, do nothing.
                if (value == cellSize) return;

                // Set the cell size.
                cellSize = value;

                // Dirty the layout.
                MakeDirty();
            }
        }

        /// <summary>
        /// If set to <c>false</c>; sets the internal cell size variable to null so that <see cref="GridSize"/> is automatically calculated from <see cref="CellSize"/>.
        /// Note that if <see cref="HasExplicitGridSize"/> is <c>false</c>, setting this value to false will have no effect.
        /// </summary>
        public bool HasExplicitCellSize
        {
            get => cellSize.HasValue;
            set
            {
                // Only handle a false value, and only make the change if there is a grid size to fall back on.
                if (!value && HasExplicitGridSize)
                {
                    // Clear the value.
                    cellSize = null;

                    // Dirty the layout.
                    MakeDirty();
                }
            }
        }

        /// <summary> How many cells wide and tall this layout is. </summary>
        public Point GridSize
        {
            get
            {
                // If the attribute has been set, use that.
                if (gridSize.HasValue) return gridSize.Value;

                // Otherwise; if the layout is dirty, recalculate and return the grid size.
                if (isDirty) recalculateGridSize();
                return cachedGridSize;
            }
            set
            {
                // If either axis of the value is invalid, do nothing.
                if (value.X <= 0 || value.Y <= 0) return;

                // If the value is the same as the current value, do nothing.
                if (value == gridSize) return;

                // Set the grid size.
                gridSize = value;

                // Dirty the layout.
                MakeDirty();
            }
        }

        /// <summary>
        /// If set to <c>false</c>; sets the internal grid size variable to null so that <see cref="GridSize"/> is automatically calculated from <see cref="CellSize"/>.
        /// Note that if <see cref="HasExplicitCellSize"/> is <c>false</c>, setting this value to false will have no effect.
        /// </summary>
        public bool HasExplicitGridSize
        {
            get => gridSize.HasValue;
            set
            {
                // Clear the value.
                gridSize = null;

                // Dirty the layout.
                MakeDirty();
            }
        }

        /// <summary> The number of columns (vertical) that this cell layout uses. </summary>
        public int ColumnCount
        {
            get => GridSize.X;
            set => GridSize = new Point(value, RowCount);
        }

        /// <summary> The number of rows (horizontal) that this cell layout uses. </summary>
        public int RowCount
        {
            get => GridSize.Y;
            set => GridSize = new Point(ColumnCount, value);
        }

        public Point Spacing
        {
            get => spacing;
            set
            {
                // Set the spacing.
                spacing = value;

                // Dirty the layout.
                MakeDirty();
            }
        }
        #endregion

        #region Constructors
        public GridLayout(ElementManager elementManager)
        {
            this.elementManager = elementManager;
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Load the grid and cell size.
            gridSize = Element.Attributes.GetAttributeOrDefault(gridSizeAttributeName, (Point?)null, ToPoint.TryParse);
            cellSize = Element.Attributes.GetAttributeOrDefault(cellSizeAttributeName, (Point?)null, ToPoint.TryParse);

            // A grid layout cannot function without at least one defined dimensional property, so throw an exception if that is the case.
            if (gridSize == null && cellSize == null)
                throw new Exception($"{nameof(GridLayout)} of {(string.IsNullOrWhiteSpace(Element.Name) ? Element.ToString() : Element.Name)} does not have a {gridSizeAttributeName} or {cellSizeAttributeName} defined. Must have at least one.");

            // Load the spacing.
            spacing = Element.Attributes.GetAttributeOrDefault(spacingAttributeName, Point.Zero, ToPoint.TryParse);
        }

        public override void OnSetup()
        {
            // Bind the child added/removed signals to recalculate the layout.
            Element.OnChildAdded.Connect((_) => MakeDirty());
            Element.OnChildRemoved.Connect((_) => MakeDirty());
        }
        #endregion

        #region Calculation Functions
        public override void OnSizeChanged() => MakeDirty();

        /// <summary> Makes this layout dirty, meaning it will be recalculated next time it is drawn, updated, or certain properties are accessed. </summary>
        public void MakeDirty() => isDirty = true;

        private void recalculateGridSize()
        {
            // If the grid size attribute has been set, there's no need to calculate the grid size.
            if (gridSize.HasValue) return;

            // Pre-calculate the cell size.
            Point cellSize = CellSize;

            // If there is no spacing, don't bother doing the calculations the long way.
            if (Spacing == Point.Zero) 
                cachedGridSize = Vector2.Floor(Bounds.ContentSize.ToVector2() / cellSize.ToVector2()).ToPoint();
            // Otherwise; do the calculations the long way.
            else
            {
                // Reset the previous value.
                cachedGridSize = Point.Zero;

                // Count the number of grid cells on the X and Y axis.
                int currentWidth = 0;
                while (currentWidth < Bounds.ContentSize.X)
                {
                    currentWidth += cellSize.X;

                    if (currentWidth < Bounds.ContentSize.X)
                        cachedGridSize.X++;

                    currentWidth += Spacing.X;
                }

                int currentHeight = 0;
                while (currentHeight < Bounds.ContentSize.Y)
                {
                    currentHeight += cellSize.Y;

                    if (currentHeight < Bounds.ContentSize.Y)
                        cachedGridSize.Y++;

                    currentHeight += Spacing.Y;
                }
            }

            // Force update the layout. This means that the grid size will accurately represent the grid,
            // and means that successive calls to GridSize.get while the layout is dirty will use the cached variable.
            isDirty = false;
            recalculateLayout();
        }

        private void recalculateLayout()
        {
            // Pre-calculate dimensions.
            Point cellSize = CellSize;
            Point gridSize = GridSize;

            // Snap each child to the grid.
            int x = 0, y = 0;
            foreach (Element child in Element)
            {
                // Snap the child to the grid.
                GridLayoutHelper.SnapToGrid(new Point(x, y), cellSize, Spacing, child.Bounds);

                // Handle incrementing the cell position.
                if (x + 1 >= gridSize.X)
                {
                    x = 0;
                    y++;
                }
                else x++;
            }

            // Undirty the layout.
            isDirty = false;
        }
        #endregion

        #region Update Functions
        public override void Draw(IGuiCamera guiCamera) { if (isDirty) recalculateLayout(); }

        public override void Update(GameTime gameTime) { if (isDirty) recalculateLayout(); }
        #endregion
    }
}