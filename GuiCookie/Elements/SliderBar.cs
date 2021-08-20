using GuiCookie.Components;
using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using LiruGameHelper.Signals;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Elements
{
    public class SliderBar : ProgressBar, IClickable
    {
        #region Components
        private MouseHandler mouseHandler;
        #endregion

        #region Elements
        /// <summary> The handle for this slider bar. </summary>
        public Element Handle { get; private set; }
        #endregion

        #region Properties
        /// <summary> Gets or sets the value of the slider bar, not invoking <see cref="OnValueChanged"/>, but still updating the position of the slider handle. </summary>
        public float ValueNoSignal
        {
            get => value;
            set
            {
                // Set the internal field.
                this.value = value;

                // Update the slider handle, but do not invoke the event.
                updateValue(false);
            }
        }

        /// <summary> The total usable space along this slider's axis that represents the value area. The <see cref="Handle"/>'s size is taken into account. </summary>
        public float UsableSize => LayoutDirection == Direction.Horizontal ? Bounds.TotalSize.X - (Handle?.Bounds.TotalSize.X ?? 0) : Bounds.TotalSize.Y - (Handle?.Bounds.TotalSize.Y ?? 0);
        #endregion

        #region Signals
        /// <summary> Invoked whenever the <see cref="Value"/> is changed. </summary>
        public IConnectableSignal OnValueChanged => onValueChanged;
        private readonly Signal onValueChanged = new Signal();

        /// <summary> Access to <see cref="MouseHandler.LeftClicked"/>. </summary>
        public IConnectableSignal LeftClicked => mouseHandler.LeftClicked;

        /// <summary> Access to <see cref="MouseHandler.RightClicked"/>. </summary>
        public IConnectableSignal RightClicked => mouseHandler.RightClicked;
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup()
        {
            // Get the mouse handler.
            mouseHandler = GetComponent<MouseHandler>();

            // Get the handle element.
            Handle = GetChildByName("Handle");

            // If there is a handle and it has no mouse handler, set its mouse handler.
            if (Handle != null && Handle.StyleState.MouseHandler == null)
                Handle.StyleState.MouseHandler = mouseHandler;

            // Set up the base progress bar.
            base.OnFullSetup();
        }
        #endregion

        #region Bind Functions
        public void ConnectValueChanged(Action action) => OnValueChanged.Connect(action);
        #endregion

        #region Calculation Functions
        protected override void OnSizeChanged() => CalculateHandlePosition();

        public void CalculateHandlePosition()
        {
            if (Handle == null) return;

            Handle.Bounds.RelativeTotalPosition = LayoutDirection == Direction.Horizontal 
                ? new Point((Handle.Bounds.TotalSize.X / 2) + (int)MathF.Floor((UsableSize * NormalisedValue) - (Bounds.RelativeContentPosition.X - Bounds.RelativeTotalPosition.X)), Bounds.TotalSize.Y / 2)
                : new Point(Bounds.TotalSize.X / 2, (Handle.Bounds.TotalSize.Y / 2) + (int)MathF.Floor((UsableSize * NormalisedValue) - (Bounds.RelativeContentPosition.Y - Bounds.RelativeTotalPosition.Y)));
        }

        /// <summary> Resizes the <see cref="Handle"/> so that it covers the set range. For example; if the slider's minimum value is 0 and the maximum value is 2, and <paramref name="range"/> is 1, the handle will fill 50% of the slider bar. </summary>
        /// <param name="range"> The range for the handle to cover. </param>
        public void ResizeHandleRange(float range)
        {
            // If there is no handle, do nothing.
            if (Handle == null) return;

            // If the minimum is equal to the maximum, do nothing.
            if (MinimumValue == MaximumValue) return;

            // Resize and reposition the handle.
            Handle.Bounds.ScaledSize = LayoutDirection == Direction.Horizontal
                ? new Space(range / (MaximumValue - MinimumValue + 1), Handle.Bounds.ScaledSize.Y, Handle.Bounds.ScaledSize.IsYRelative ? Axes.Both : Axes.X)
                : new Space(Handle.Bounds.ScaledSize.X, range / (MaximumValue - MinimumValue + 1), Handle.Bounds.ScaledSize.IsXRelative ? Axes.Both : Axes.Y);
            CalculateHandlePosition();
        }

        protected override void onValueRecalculated(float oldValue) => updateValue(oldValue != Value);

        private void updateValue(bool invokeSignal)
        {
            // Recalculate the position of the slider regardless of if the value changed.
            CalculateHandlePosition();

            // If the signal should invoke, do so.
            if (invokeSignal) onValueChanged.Invoke();
        }
        #endregion

        #region Style Functions
        public override void OnStyleChanged() => fillCache.Refresh(Style);
        #endregion

        #region Update Functions
        protected override void Update(GameTime gameTime)
        {
            // If the bar is clicked, handle sliding the handle.
            if (mouseHandler.IsClickDragged)
            {
                // Calculate half of the handle's size. This is half of the size of the handle on the axis of the bar, or 0 if there is no handle.
                float halfHandleSize = Handle != null ? (LayoutDirection == Direction.Horizontal ? Handle.Bounds.TotalSize.X : Handle.Bounds.TotalSize.Y) / 2.0f : 0;

                // Calculate the position of the mouse within the usable area of the bar.
                float mousePos = LayoutDirection == Direction.Horizontal
                    ? MathHelper.Clamp(mouseHandler.RelativeMousePosition.X, halfHandleSize, Bounds.TotalSize.X - halfHandleSize) - halfHandleSize
                    : MathHelper.Clamp(mouseHandler.RelativeMousePosition.Y, halfHandleSize, Bounds.TotalSize.Y - halfHandleSize) - halfHandleSize;

                // Calculate the value based on what part of the bar is being clicked.
                NormalisedValue = (mousePos / UsableSize);
            }
        }
        #endregion

        #region Draw Functions
        protected override void drawFill(IGuiCamera guiCamera)
        {
            // Do nothing if there is no fill.
            if (!fillCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrame fill)) return;

            // If there is no handle, fall back on the progress bar's way of drawing the fill.
            if (Handle == null) base.drawFill(guiCamera);
            // Otherwise; draw the fill so that it is hidden behind the handle.
            else
            {
                // Calculate the absolute area for the fill to be drawn.
                Rectangle fillArea = FillPadding.ScaleRectangle(Bounds.AbsoluteTotalArea);

                // Adjust the fill area's width or height so that it is hidden behind the handle.
                if (LayoutDirection == Direction.Horizontal) fillArea.Width = Handle.Bounds.AbsoluteTotalArea.Center.X - fillArea.X;
                else fillArea.Height = Handle.Bounds.AbsoluteTotalArea.Center.Y - fillArea.Y;

                // Draw the fill.
                NineSliceDrawer.DrawFrameOnDemand(fill, fillArea, guiCamera, fill.MixedColour);
            }
        }
        #endregion
    }
}
