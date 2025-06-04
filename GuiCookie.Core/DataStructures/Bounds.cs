using GuiCookie.Core.Data;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Helpers;
using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.DataStructures
{
    public class Bounds(Element element)
    {
        #region Constants
        private const string pivotAttributeName = "Pivot";
        private const string anchorAttributeName = "Anchor";
        private const string positionAttributeName = "Position";
        private const string sizeAttributeName = "Size";
        private const string maximumSizeAttributeName = "MaximumSize";
        private const string minimumSizeAttributeName = "MinimumSize";
        private const string paddingAttributeName = "Padding";
        #endregion

        #region Size Fields
        private Space scaledSize = Space.Empty;

        private Point totalSize = new();

        private Point contentSize = new();

        private Space minimumSize = Space.Empty;

        private Space maximumSize = Space.Empty;

        private Sides padding = new(0, SideMask.None);

        private Rectangle baseRectangle = Rectangle.Empty;
        #endregion

        #region Position Fields
        private Space scaledPosition = Space.Empty;

        private Point absolutePosition = new();

        private Point relativePosition = new();

        private Space anchor = Space.Empty;

        private Space pivot = Space.Empty;
        #endregion

        #region Properties
        /// <summary> The parent bounds, or null if there is no parent. </summary>
        public Bounds? Parent => element.Parent?.Bounds;

        private Rectangle parentAbsoluteContentArea => Parent?.AbsoluteContentArea ?? baseRectangle;
        #endregion

        #region Size Properties
        /// <summary> The size in either absolute (pixels) or relative (percentage) measurements. </summary>
        public Space ScaledSize
        {
            get => scaledSize;
            set { scaledSize = value; recalculateSize(); }
        }

        /// <summary> The minimum size of the bounds. </summary>
        public Space MinimumSize
        {
            get => minimumSize;
            set { minimumSize = value; recalculateSize(); }
        }

        /// <summary> The maximum size of the bounds. </summary>
        public Space MaximumSize
        {
            get => maximumSize;
            set { maximumSize = value; recalculateSize(); }
        }

        /// <summary> The space around the sides of the overall bounds. </summary>
        public Sides Padding
        {
            get => padding;
            set { padding = value; recalculateSize(); }
        }

        /// <summary> The total size of the entire bounds. </summary>
        public Point TotalSize
        {
            get => totalSize;
            set
            {
                // Create the information required to create a new space.
                Vector2 newScaledSize = new(value.X, value.Y);
                Axes relativeAxes = Axes.None;

                // If the x is relative, set the relative axis and divide the size by the size of the parent's content width to make it relative. Do the same for the y axis.
                if (ScaledSize.IsXRelative)
                {
                    relativeAxes |= Axes.X;
                    newScaledSize.X /= parentAbsoluteContentArea.Size.Width;
                }
                if (ScaledSize.IsYRelative)
                {
                    relativeAxes |= Axes.Y;
                    newScaledSize.Y /= parentAbsoluteContentArea.Size.Height;
                }

                // Set the scaled size, which then calculates the changes.
                ScaledSize = new Space(newScaledSize.X, newScaledSize.Y, relativeAxes);
            }
        }

        /// <summary> The size of the content area. </summary>
        public Point ContentSize
        {
            get => contentSize;
            set => TotalSize = (Point)padding.InverseScaleRectangle(new Rectangle(new Point(), (Size)value)).Size;
        }
        #endregion

        #region Position Properties
        /// <summary> The offset of the position relative to the size. </summary>
        public Space Anchor
        {
            get => anchor;
            set { anchor = value; recalculatePosition(); }
        }

        public Space Pivot
        {
            get => pivot;
            set { pivot = value; recalculatePosition(); }
        }

        /// <summary> The scaled position, using either pixel or percentage measurements. </summary>
        public Space ScaledPosition
        {
            get => scaledPosition;
            set { scaledPosition = value; recalculatePosition(); }
        }

        /// <summary> The position of the bounds relative to the position of the parent content area. </summary>
        public Point RelativeTotalPosition
        {
            get => relativePosition;
            set
            {
                // Create the information required to create a new space.
                Vector2 newScaledPosition = new(value.X, value.Y);
                Axes relativeAxes = Axes.None;

                // If the x is relative, set the relative axis and divide the position by the size of the parent's content width to make it relative. Do the same for the y axis.
                if (ScaledPosition.IsXRelative)
                {
                    relativeAxes |= Axes.X;
                    newScaledPosition.X /= parentAbsoluteContentArea.Size.Width;
                }
                if (ScaledPosition.IsYRelative)
                {
                    relativeAxes |= Axes.Y;
                    newScaledPosition.Y /= parentAbsoluteContentArea.Size.Height;
                }

                // Set the scaled position, which then calculates the changes.
                ScaledPosition = new Space(newScaledPosition.X, newScaledPosition.Y, relativeAxes);
            }
        }

        /// <summary> The position of the bounds within the root. </summary>
        public Point AbsoluteTotalPosition
        {
            get => absolutePosition;
            set => RelativeTotalPosition = PointExtensions.Subtract(value, parentAbsoluteContentArea.Location);
        }

        public Rectangle RelativeTotalArea
        {
            get => new(RelativeTotalPosition, (Size)TotalSize);
            set
            {
                TotalSize = (Point)value.Size;
                RelativeTotalPosition = value.Location;
            }
        }

        public Rectangle AbsoluteTotalArea
        {
            get => new(AbsoluteTotalPosition, (Size)TotalSize);
            set
            {
                TotalSize = (Point)value.Size;
                AbsoluteTotalPosition = value.Location;
            }
        }

        public Point AbsoluteContentPosition
        {
            get => padding.ScaleRectangle(AbsoluteTotalArea).Location;
            set => AbsoluteTotalPosition = PointExtensions.Subtract(value, padding.ScaleRectangle(new Rectangle(new(), (Size)TotalSize)).Location);
        }

        public Point RelativeContentPosition
        {
            get => padding.ScaleRectangle(RelativeTotalArea).Location;
            set => RelativeTotalPosition = PointExtensions.Subtract(value, padding.ScaleRectangle(new Rectangle(new(), (Size)TotalSize)).Location);
        }

        public Rectangle AbsoluteContentArea
        {
            get => new(AbsoluteContentPosition, (Size)ContentSize);
            set { AbsoluteContentPosition = value.Location; ContentSize = (Point)value.Size; }
        }

        public Rectangle RelativeContentArea
        {
            get => new(RelativeContentPosition, (Size)ContentSize);
            set { RelativeContentPosition = value.Location; ContentSize = (Point)value.Size; }
        }
        #endregion

        #region Load Functions
        public void LoadFromAttributes(IReadOnlyAttributeCollection attributes)
        {
            // Get all associated attributes from the element attributes.
            scaledPosition = attributes.GetAttributeOrDefault(positionAttributeName, scaledPosition);
            scaledSize = attributes.GetAttributeOrDefault(sizeAttributeName, scaledSize);
            padding = attributes.GetAttributeOrDefault(paddingAttributeName, padding);
            anchor = attributes.GetAttributeOrDefault(anchorAttributeName, anchor);
            pivot = attributes.GetAttributeOrDefault(pivotAttributeName, pivot);
            minimumSize = attributes.GetAttributeOrDefault(minimumSizeAttributeName, minimumSize);
            maximumSize = attributes.GetAttributeOrDefault(maximumSizeAttributeName, maximumSize);
        }
        #endregion

        #region Contains Functions
        public bool AbsoluteContains(Point position) => AbsoluteTotalArea.Contains(position);

        public bool RelativeContains(Point position) => RelativeTotalArea.Contains(position);
        #endregion

        #region Calculation Functions
        internal void recalculatePosition()
        {
            relativePosition = (scaledPosition.GetScaledSpace((Point)parentAbsoluteContentArea.Size) + (Size)anchor.GetScaledSpace((Point)parentAbsoluteContentArea.Size)) - (Size)pivot.GetScaledSpace(TotalSize);
            absolutePosition = relativePosition + (Size)parentAbsoluteContentArea.Location;

            element?.onPositionChanged();
        }

        internal void recalculateSize()
        {
            totalSize = scaledSize.GetScaledSpace((Point)parentAbsoluteContentArea.Size);
            contentSize = (Point)padding.ScaleRectangle(RelativeTotalArea).Size;

            if (element != null && !element.validateSizeChanged()) return;

            element?.onSizeChanged();
            recalculatePosition();
        }
        #endregion
    }
}