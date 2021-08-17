using GuiCookie.DataStructures;
using GuiCookie.Elements;
using GuiCookie.Rendering;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
{
    public class TitledPanel : Component
    {
        #region Elements
        public ITextable TitleBar { get; private set; }

        public Element ContentArea { get; private set; }

        public ISizeCalculator ContentSizeCalculator { get; private set; }
        #endregion

        #region Fields
        private bool isDirty = true;
        #endregion

        #region Initialisation Functions
        public override void OnPostSetup()
        {
            // Get the elements.
            TitleBar = Element.GetInterfacedChildByName<ITextable>("TitleBar");
            ContentArea = Element.GetChildByName("Content");
            ContentSizeCalculator = ContentArea != null && (ContentArea is ISizeCalculator sizeCalculator || ContentArea.TryGetInterfacedComponent(out sizeCalculator)) ? sizeCalculator : null;
        }
        #endregion

        #region Calculation Functions
        public override void OnSizeChanged() => isDirty = true;

        private void recalculateContentArea()
        {
            // Undirty the panel immediately.
            isDirty = false;

            // If either the title or the content is missing, do nothing.
            if (TitleBar == null || ContentArea == null) return;

            // If the content has a size calculator, use it to determine the size of the whole panel.
            if (ContentSizeCalculator != null)
            {
                // Calculate the total desired height of the content.
                int contentHeight = ContentArea.Bounds.Padding.InverseScaleRectangle(new Rectangle(0, 0, 0, ContentSizeCalculator.DesiredSize.Y)).Height;

                // Resize the content to fit the calculated size.
                ContentArea.Bounds.ScaledSize = new Space(1.0f, contentHeight, Axes.X);

                // Resize the panel to fit both the title and the content.
                Bounds.ContentSize = new Point(Bounds.ContentSize.X, ContentArea.Bounds.TotalSize.Y + TitleBar.Bounds.TotalSize.Y);

                // Resizing the bounds will cause it to be dirty again. Undirty it immediately.
                isDirty = false;

                // Cascade up.
                if (Element.Parent is ISizeCalculator sizeCalculator || Element.Parent.TryGetInterfacedComponent(out sizeCalculator)) sizeCalculator.MakeDirty();
            }
            // Otherwise; calculate the desired height of the content area.
            else ContentArea.Bounds.ScaledSize = new Space(1.0f, Bounds.ContentSize.Y - TitleBar.Bounds.TotalSize.Y, Axes.X);

            // Reposition the content area.
            ContentArea.Bounds.RelativeTotalPosition = new Point(0, TitleBar.Bounds.TotalSize.Y);
        }
        #endregion

        #region Update Functions
        public override void Draw(IGuiCamera guiCamera) { if (isDirty) recalculateContentArea(); }

        public override void Update(GameTime gameTime) { if (isDirty) recalculateContentArea(); }
        #endregion
    }
}
