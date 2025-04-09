using GuiCookie.Core.Data;
using GuiCookie.Core.Helpers;
using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.DataStructures
{
    public class Bounds
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
        public ElementContainer ElementContainer { get; }

        /// <summary> The parent bounds, or null if these bounds are for the root. </summary>
        public Bounds Parent => ElementContainer.Parent?.Element != null ? ElementContainer.Parent.Element.Bounds : ElementContainer.Parent != null ? ElementContainer.Root.Bounds : null;

        private Rectangle parentAbsoluteContentArea => Parent == null ? baseRectangle : Parent.AbsoluteContentArea;
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

        #region Constructors
        /// <summary> Create a new <see cref="Bounds"/> with the given <see cref="Attributes"/>. </summary>
        /// <param name="elementAttributes"> The attributes of the containing <see cref="Elements"/>, which is used to determine spacing data. </param>
        public Bounds(ElementContainer elementContainer, IReadOnlyAttributeCollection elementAttributes)
        {
            ElementContainer = elementContainer ?? throw new ArgumentNullException(nameof(elementContainer));

            // Get all associated attributes from the element attributes.
            scaledPosition = elementAttributes.GetAttributeOrDefault(positionAttributeName, new Space(0, Axes.None));
            scaledSize = elementAttributes.GetAttributeOrDefault(sizeAttributeName, new Space(0, Axes.None));
            padding = elementAttributes.GetAttributeOrDefault(paddingAttributeName, new Sides(0, SideMask.None));
            anchor = elementAttributes.GetAttributeOrDefault(anchorAttributeName, new Space(0, Axes.Both));
            pivot = elementAttributes.GetAttributeOrDefault(pivotAttributeName, new Space(0, Axes.Both));
            minimumSize = elementAttributes.GetAttributeOrDefault(minimumSizeAttributeName, new Space(0, Axes.None));
            maximumSize = elementAttributes.GetAttributeOrDefault(maximumSizeAttributeName, new Space(0, Axes.None));
        }

        internal Bounds(ElementContainer elementContainer, Point windowSize)
        {
            ElementContainer = elementContainer ?? throw new ArgumentNullException(nameof(elementContainer));

            baseRectangle = new Rectangle(0, 0, windowSize.X, windowSize.Y);

            // The root fills the entire window unless otherwise stated.
            scaledSize = new Space(windowSize.X, windowSize.Y, Axes.None);

            // The root has no padding unless otherwise specified.
            padding = new Sides(0, SideMask.None);

            // Calculate the bounds from the parsed properties.
            recalculateSize();
            recalculatePosition();
        }

        internal Bounds(ElementContainer elementContainer, Point windowSize, IReadOnlyAttributeCollection rootAttributes)
        {
            ElementContainer = elementContainer ?? throw new ArgumentNullException(nameof(elementContainer));

            baseRectangle = new Rectangle(new Point(), (Size)windowSize);

            // The root fills the entire window unless otherwise stated.
            scaledSize = rootAttributes.GetAttributeOrDefault(sizeAttributeName, new Space(windowSize.X, windowSize.Y, Axes.None));

            // The root has no padding unless otherwise specified.
            padding = rootAttributes.GetAttributeOrDefault(paddingAttributeName, new Sides(0, SideMask.None));

            // Calculate the bounds from the parsed properties.
            recalculateSize();
            recalculatePosition();
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

            ElementContainer.Element?.onPositionChanged();
        }

        internal void recalculateSize()
        {
            totalSize = scaledSize.GetScaledSpace((Point)parentAbsoluteContentArea.Size);
            contentSize = (Point)padding.ScaleRectangle(RelativeTotalArea).Size;

            if (ElementContainer.Element != null && !ElementContainer.Element.validateSizeChanged()) return;

            ElementContainer.Element?.onSizeChanged();
            recalculatePosition();
        }
        #endregion
    }
}