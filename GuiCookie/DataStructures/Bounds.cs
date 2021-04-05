using GuiCookie.Attributes;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.DataStructures
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

        private Point totalSize = Point.Zero;

        private Point contentSize = Point.Zero;

        private Space minimumSize = Space.Empty;

        private Space maximumSize = Space.Empty;

        private Sides padding = new Sides(0, SideMask.None);

        private Rectangle baseRectangle = Rectangle.Empty;
        #endregion

        #region Position Fields
        private Space scaledPosition = Space.Empty;

        private Point absolutePosition = Point.Zero;

        private Point relativePosition = Point.Zero;

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
                Vector2 newScaledSize = value.ToVector2();
                Axes relativeAxes = Axes.None;

                // If the x is relative, set the relative axis and divide the size by the size of the parent's content width to make it relative. Do the same for the y axis.
                if (ScaledSize.IsXRelative)
                {
                    relativeAxes |= Axes.X;
                    newScaledSize.X /= parentAbsoluteContentArea.Size.X;
                }
                if (ScaledSize.IsYRelative)
                {
                    relativeAxes |= Axes.Y;
                    newScaledSize.Y /= parentAbsoluteContentArea.Size.Y;
                }

                // Set the scaled size, which then calculates the changes.
                ScaledSize = new Space(newScaledSize.X, newScaledSize.Y, relativeAxes);
            }
        }

        /// <summary> The size of the content area. </summary>
        public Point ContentSize
        {
            get => contentSize;
            set => TotalSize = padding.InverseScaleRectangle(new Rectangle(Point.Zero, value)).Size;
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
                Vector2 newScaledPosition = value.ToVector2();
                Axes relativeAxes = Axes.None;

                // If the x is relative, set the relative axis and divide the position by the size of the parent's content width to make it relative. Do the same for the y axis.
                if (ScaledPosition.IsXRelative)
                {
                    relativeAxes |= Axes.X;
                    newScaledPosition.X /= parentAbsoluteContentArea.Size.X;
                }
                if (ScaledPosition.IsYRelative)
                {
                    relativeAxes |= Axes.Y;
                    newScaledPosition.Y /= parentAbsoluteContentArea.Size.Y;
                }

                // Set the scaled position, which then calculates the changes.
                ScaledPosition = new Space(newScaledPosition.X, newScaledPosition.Y, relativeAxes);
            }
        }

        /// <summary> The position of the bounds within the root. </summary>
        public Point AbsoluteTotalPosition
        {
            get => absolutePosition;
            set => RelativeTotalPosition = value - parentAbsoluteContentArea.Location;
        }

        public Rectangle RelativeTotalArea
        {
            get => new Rectangle(RelativeTotalPosition, TotalSize);
            set { TotalSize = value.Size; RelativeTotalPosition = value.Location; }
        }

        public Rectangle AbsoluteTotalArea
        {
            get => new Rectangle(AbsoluteTotalPosition, TotalSize); 
            set { TotalSize = value.Size; AbsoluteTotalPosition = value.Location; }
        }

        public Point AbsoluteContentPosition
        {
            get => padding.ScaleRectangle(AbsoluteTotalArea).Location;
            set => AbsoluteTotalPosition = value - padding.ScaleRectangle(new Rectangle(Point.Zero, TotalSize)).Location; 
        }

        public Point RelativeContentPosition
        {
            get => padding.ScaleRectangle(RelativeTotalArea).Location;
            set => RelativeTotalPosition = value - padding.ScaleRectangle(new Rectangle(Point.Zero, TotalSize)).Location;
        }

        public Rectangle AbsoluteContentArea 
        { 
            get => new Rectangle(AbsoluteContentPosition, ContentSize);
            set { AbsoluteContentPosition = value.Location; ContentSize = value.Size; } 
        }

        public Rectangle RelativeContentArea 
        { 
            get => new Rectangle(RelativeContentPosition, ContentSize);
            set { RelativeContentPosition = value.Location; ContentSize = value.Size; } 
        }
        #endregion

        #region Constructors
        /// <summary> Create a new <see cref="Bounds"/> with the given <see cref="Attributes"/>. </summary>
        /// <param name="elementAttributes"> The attributes of the containing <see cref="Elements"/>, which is used to determine spacing data. </param>
        public Bounds(ElementContainer elementContainer, IReadOnlyAttributes elementAttributes)
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

        internal Bounds(ElementContainer elementContainer, Point windowSize, IReadOnlyAttributes rootAttributes)
        {
            ElementContainer = elementContainer ?? throw new ArgumentNullException(nameof(elementContainer));

            baseRectangle = new Rectangle(Point.Zero, windowSize);

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
        public bool AbsoluteContains(Point point) => AbsoluteTotalArea.Contains(point);

        public bool RelativeContains(Point point) => RelativeTotalArea.Contains(point);
        #endregion

        #region Calculation Functions
        internal void recalculatePosition()
        {
            relativePosition = (scaledPosition.GetScaledSpace(parentAbsoluteContentArea.Size) + anchor.GetScaledSpace(parentAbsoluteContentArea.Size)) - pivot.GetScaledSpace(TotalSize);
            absolutePosition = parentAbsoluteContentArea.Location + relativePosition;

            ElementContainer.Element?.onPositionChanged();
        }

        internal void recalculateSize()
        {
            totalSize = scaledSize.GetScaledSpace(parentAbsoluteContentArea.Size);
            contentSize = padding.ScaleRectangle(RelativeTotalArea).Size;

            if (ElementContainer.Element != null && !ElementContainer.Element.validateSizeChanged()) return;

            ElementContainer.Element?.onSizeChanged();
            recalculatePosition();
        }
        #endregion
    }
}
