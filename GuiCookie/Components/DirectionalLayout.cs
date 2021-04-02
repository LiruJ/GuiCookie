using GuiCookie.DataStructures;
using GuiCookie.Elements;
using GuiCookie.Rendering;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
{
    public class DirectionalLayout : Component
    {
        #region Constants
        private const string layoutAttributeName = "Layout";
        #endregion

        #region Fields
        private bool isDirty = true;
        #endregion

        #region Backing Fields
        private Direction layoutDirection = Direction.Vertical;
        private Point desiredSize;
        #endregion

        #region Properties
        public Direction LayoutDirection
        {
            get => layoutDirection;
            set
            {
                // Ensure validity.
                if (value == Direction.None) throw new ArgumentException("Cannot set direction of directonal layout to none!");

                layoutDirection = value;
            }
        }

        public Point DesiredSize { get { if (isDirty) recalculateLayout(); return desiredSize; } private set => desiredSize = value; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            LayoutDirection = Element.Attributes.GetEnumAttributeOrDefault(layoutAttributeName, Direction.Vertical);

            Element.OnChildAdded.Connect((c) => MakeDirty());
            Element.OnChildRemoved.Connect((c) => MakeDirty());
        }
        #endregion

        #region Event Functions
        public override void OnSizeChanged() => MakeDirty();
        #endregion

        #region Calculation Functions
        public void MakeDirty() => isDirty = true;

        private void recalculateLayout()
        {
            // If the layout is not dirty, do nothing.
            if (!isDirty) return;

            int currentPosition = 0;
            desiredSize = Point.Zero;

            // Go over each child and position it.
            foreach (Element child in Element)
            {
                // Reposition the child.
                child.Bounds.RelativeTotalPosition = LayoutDirection == Direction.Horizontal ? new Point(currentPosition, 0) : new Point(0, currentPosition);

                // If the dimension of the child's size is relative, make it absolute.
                if (LayoutDirection == Direction.Horizontal && child.Bounds.ScaledSize.IsXRelative)
                    child.Bounds.ScaledSize = new Space(child.Bounds.TotalSize.X, child.Bounds.ScaledSize.IsYRelative ? (float)child.Bounds.TotalSize.Y / Bounds.ContentSize.Y : child.Bounds.TotalSize.Y, child.Bounds.ScaledSize.Axes ^ Axes.X);
                else if (LayoutDirection == Direction.Vertical && child.Bounds.ScaledSize.IsYRelative)
                    child.Bounds.ScaledSize = new Space(child.Bounds.ScaledSize.IsXRelative ? (float)child.Bounds.TotalSize.X / Bounds.ContentSize.X : child.Bounds.TotalSize.X, child.Bounds.TotalSize.Y, child.Bounds.ScaledSize.Axes ^ Axes.Y);

                // Increment the position.
                currentPosition += (int)Math.Floor(LayoutDirection == Direction.Horizontal ? child.Bounds.ScaledSize.GetScaledX(Bounds.ContentSize.X) : child.Bounds.ScaledSize.GetScaledY(Bounds.ContentSize.Y));

                // Add to the desired size.
                desiredSize = new Point(
                    LayoutDirection == Direction.Vertical ? Math.Max(desiredSize.X, child.Bounds.TotalSize.X) : desiredSize.X + child.Bounds.TotalSize.X,
                    LayoutDirection == Direction.Horizontal ? Math.Max(desiredSize.Y, child.Bounds.TotalSize.Y) : desiredSize.Y + child.Bounds.TotalSize.Y
                    );
            }

            // Since the layout has been recalculated, it is no longer dirty.
            isDirty = false;
        }
        #endregion

        #region Update/Draw Functions
        public override void Draw(IGuiCamera guiCamera) { if (isDirty) recalculateLayout(); }

        public override void Update(GameTime gameTime) { if (isDirty) recalculateLayout(); }
        #endregion
    }
}
