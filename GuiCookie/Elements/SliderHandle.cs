using GuiCookie.DataStructures;

namespace GuiCookie.Elements
{
    public class SliderHandle : Element
    {
        #region Fields
        private SliderBar sliderBar;
        #endregion

        #region Properties
        public float SizeScalar => sliderBar.LayoutDirection == Direction.Horizontal ?
            (Bounds.ScaledSize.IsXRelative ? Bounds.ScaledSize.X : (float)Bounds.TotalSize.X / sliderBar.Bounds.TotalSize.X) :
            (Bounds.ScaledSize.IsYRelative ? Bounds.ScaledSize.Y : (float)Bounds.TotalSize.Y / sliderBar.Bounds.TotalSize.Y);
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Get the parent as a slider bar. If the parent is not a slider bar, throw an exception.
            sliderBar = (Parent is SliderBar slider) ? slider : throw new System.Exception("Slider knob cannot be a child of a non-slider bar element.");
        }
        #endregion

        #region Slider Functions
        public void CalculateSliderPosition()
        {
            // Calculate the relative position of the handle within the bar. Respect the handle's size in relation to the normalised value of the bar.
            float positionScalar = ((1 - SizeScalar) * sliderBar.NormalisedValue) + (SizeScalar / 2);

            // Use the scalar as a relative position on the axis, repositioning the handle along the bar.
            Bounds.ScaledPosition = sliderBar.LayoutDirection == Direction.Horizontal ? new Space(positionScalar, 0.5f, Axes.Both) : new Space(0.5f, positionScalar, Axes.Both);
        }
        #endregion
    }
}
