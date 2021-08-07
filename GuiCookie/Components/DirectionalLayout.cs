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

        private const string spacingAttributeName = "Spacing";
        #endregion

        #region Fields
        /// <summary> Is <c>true</c> if the layout needs to recalculate next frame, otherwise; <c>false</c>. </summary>
        private bool isDirty = true;
        #endregion

        #region Backing Fields
        private Direction layoutDirection = Direction.Vertical;

        private Point desiredSize;

        private int spacing;
        #endregion

        #region Properties
        /// <summary> The direction of the layout. Has no effect if value is <see cref="Direction.None"/>. </summary>
        public Direction LayoutDirection
        {
            get => layoutDirection;
            set
            {
                // Ensure validity.
                if (value == Direction.None) return;

                // Set the direction.
                layoutDirection = value;

                // Make the layout dirty.
                MakeDirty();
            }
        }

        /// <summary> How large this layout wants to be, taking into account all child sizes and spacing. </summary>
        /// <remarks> Note that this may be larger than the element itself if the children are relative. </remarks>
        public Point DesiredSize 
        {
            get { if (isDirty) recalculateLayout(); return desiredSize; } 
            private set => desiredSize = value; 
        }

        /// <summary> How many pixels are between each element. </summary>
        public int Spacing 
        {
            get => spacing;
            set
            {
                // Don't set the spacing to a negative value.
                if (value < 0) return;

                // If the value is the same, do nothing.
                if (value == spacing) return;

                // Set the spacing.
                spacing = value;

                // Mark the layout as dirty.
                MakeDirty();
            }
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Parse the direction.
            LayoutDirection = Element.Attributes.GetEnumAttributeOrDefault(layoutAttributeName, Direction.Vertical);

            // Parse the spacing.
            Spacing = Element.Attributes.GetAttributeOrDefault(spacingAttributeName, 0);

            Element.OnChildAdded.Connect((c) => MakeDirty());
            Element.OnChildRemoved.Connect((c) => MakeDirty());
        }
        #endregion

        #region Calculation Functions
        public override void OnSizeChanged() => MakeDirty();

        /// <summary> Makes this layout dirty, meaning it will be recalculated next time it is drawn, updated, or certain properties are accessed. </summary>
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

                // Increment the position.
                currentPosition += (int)Math.Floor(LayoutDirection == Direction.Horizontal ? child.Bounds.ScaledSize.GetScaledX(Bounds.ContentSize.X) : child.Bounds.ScaledSize.GetScaledY(Bounds.ContentSize.Y));

                // Add spacing.
                currentPosition += Spacing;

                // Add to the desired size.
                desiredSize = new Point(
                    LayoutDirection == Direction.Vertical ? Math.Max(desiredSize.X, child.Bounds.TotalSize.X) : desiredSize.X + child.Bounds.TotalSize.X,
                    LayoutDirection == Direction.Horizontal ? Math.Max(desiredSize.Y, child.Bounds.TotalSize.Y) : desiredSize.Y + child.Bounds.TotalSize.Y
                    );
            }

            // Remove a single spacing from the desired size, as the final element does not need spacing.
            if (Element.ChildCount > 0)
            {
                if (LayoutDirection == Direction.Horizontal) desiredSize.X -= Spacing;
                else if (LayoutDirection == Direction.Vertical) desiredSize.Y -= Spacing;
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