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
        public SliderHandle Handle { get; private set; }
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

            // Get the handle element, throw an exception if it does not exist.
            Handle = GetChild<SliderHandle>() ?? throw new Exception("Slider bar is missing slider handle child.");

            // Set up the base progress bar.
            base.OnFullSetup();
        }
        #endregion

        #region Bind Functions
        public void ConnectValueChanged(Action action) => OnValueChanged.Connect(action);
        #endregion

        #region Calculation Functions
        protected override void OnSizeChanged() => Handle?.CalculateSliderPosition();

        protected override void onValueRecalculated(float oldValue) => updateValue(oldValue != Value);

        private void updateValue(bool invokeSignal)
        {
            // Recalculate the position of the slider regardless of if the value changed..
            Handle?.CalculateSliderPosition();

            // If the signal should invoke, do so.
            if (invokeSignal) onValueChanged.Invoke();
        }
        #endregion

        #region Style Functions
        public override void OnStyleChanged() => fillFrameCache.Refresh(Style);
        #endregion

        #region Update Functions
        protected override void Update(GameTime gameTime)
        {
            // If the bar is clicked, handle sliding the handle.
            if (mouseHandler.IsClickDragged)
            {
                // Calculate the useable size of the bar.
                float size = LayoutDirection == Direction.Horizontal ? Bounds.TotalSize.X - Handle.Bounds.TotalSize.X : Bounds.TotalSize.Y - Handle.Bounds.TotalSize.Y;

                // Calculate the position of the mouse within the usable area of the bar.
                float mousePos = LayoutDirection == Direction.Horizontal ?
                    MathHelper.Clamp(mouseHandler.RelativeMousePosition.X, Handle.Bounds.TotalSize.X / 2.0f, Bounds.TotalSize.X - (Handle.Bounds.TotalSize.X / 2.0f)) - (Handle.Bounds.TotalSize.X / 2.0f) :
                    MathHelper.Clamp(mouseHandler.RelativeMousePosition.Y, Handle.Bounds.TotalSize.Y / 2.0f, Bounds.TotalSize.Y - (Handle.Bounds.TotalSize.Y / 2.0f)) - (Handle.Bounds.TotalSize.Y / 2.0f);

                // Calculate the value based on what part of the bar is being clicked.
                NormalisedValue = (mousePos / size);
            }
        }
        #endregion

        #region Draw Functions
        protected override void drawFill(IGuiCamera guiCamera)
        {
            // Do nothing if there is no fill.
            if (!fillFrameCache.TryGetVariantAttribute(CurrentStyleVariant, out SliceFrame fill)) return;

            // Calculate the size of the fill.
            Point fillSize = LayoutDirection == Direction.Horizontal
                ? new Point((int)MathF.Floor(Handle.Bounds.ScaledPosition.GetScaledX(Bounds.ContentSize.X)), Bounds.ContentSize.Y)
                : new Point(Bounds.ContentSize.X, (int)MathF.Floor(Handle.Bounds.ScaledPosition.GetScaledY(Bounds.ContentSize.Y)));

            // Calculate the destination of the fill.
            Rectangle fillDestination = new Rectangle(Bounds.AbsoluteContentPosition, fillSize);

            // Draw the fill.
            NineSliceDrawer.DrawFrameOnDemand(fill, fillDestination, guiCamera, fill.FinalColour);
        }
        #endregion
    }
}
