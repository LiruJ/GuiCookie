using GuiCookie.Components;
using GuiCookie.Elements;

namespace GuiCookie.Styles
{
    public class StyleStateMachine
    {
        #region Constants
        protected const string baseVariantName = "Base";

        protected const string hoveredVariantName = "Hovered";

        protected const string clickedVariantName = "Clicked";

        protected const string disabledVariantName = "Disabled";
        #endregion

        #region Fields
        protected readonly Element element;

        private Style style;
        #endregion

        #region Cached Variants
        /// <summary> The base variant of the current <see cref="Style"/>. </summary>
        public StyleVariant BaseVariant { get; protected set; }

        /// <summary> The variant of the current <see cref="Style"/> that is used when the element is moused over, defaulting to <see cref="BaseVariant"/> if none was defined. </summary>
        /// <remarks> The element needs a <see cref="MouseHandler"/> in order for this variant to be used. </remarks>
        public StyleVariant HoveredVariant { get; protected set; }

        /// <summary> The variant of the current <see cref="Style"/> that is used when the element is clicked, defaulting to <see cref="BaseVariant"/> if none was defined. </summary>
        /// <remarks> The element needs a <see cref="MouseHandler"/> in order for this variant to be used. </remarks>
        public StyleVariant ClickedVariant { get; protected set; }

        /// <summary> The variant of the current <see cref="Style"/> that is used when the element is disabled, defaulting to <see cref="BaseVariant"/> if none was defined. </summary>
        public StyleVariant DisabledVariant { get; protected set; }
        #endregion

        #region Components
        /// <summary> The mouse handler of the parent element. </summary>
        protected readonly MouseHandler mouseHandler;
        #endregion

        #region Properties
        /// <summary> The current style variant according to the state of the element. </summary>
        public StyleVariant CurrentStyleVariant { get; protected set; }

        /// <summary> The style to work with. </summary>
        public Style Style
        {
            get => style;
            set
            {
                // Set the style.
                style = value;

                // Refresh the current variant.
                refreshStyleVariants();
            }
        }
        #endregion

        #region Constructors
        public StyleStateMachine(Element element)
        {
            // Set the element.
            this.element = element;

            // Set the mouse handler. This can be null, in which case it will be ignored.
            mouseHandler = element.GetComponent<MouseHandler>();
        }
        #endregion

        #region Change Functions
        private void refreshStyleVariants()
        {
            // If a new style exists, cache the variants.
            BaseVariant = Style?.BaseVariant;
            HoveredVariant = Style?.GetStyleVariantFromName(hoveredVariantName) ?? BaseVariant;
            ClickedVariant = Style?.GetStyleVariantFromName(clickedVariantName) ?? BaseVariant;
            DisabledVariant = Style?.GetStyleVariantFromName(disabledVariantName) ?? BaseVariant;

            // Refresh the current variant.
            UpdateCurrentStyle();

            // Invoke the virtual method.
            onStyleChanged();

            // Tell the element that its style was changed.
            element.onStyleChanged();
        }

        protected virtual void onStyleChanged() { }
        #endregion

        #region Update Functions
        public virtual void UpdateCurrentStyle()
        {
            // If the style is null, then the current style variant is also null.
            if (Style == null) CurrentStyleVariant = null;

            // First, if the element is disabled, change the style to disabled.
            if (!element.Enabled) CurrentStyleVariant = DisabledVariant;
            // Otherwise; if the mouse has clicked the element, change the style of the state machine.
            else
            {
                // If the element has a mouse handler, use it for the state.
                if (mouseHandler != null)
                {
                    if (mouseHandler.IsLeftClicked || mouseHandler.IsRightClicked) CurrentStyleVariant = ClickedVariant;
                    else if (mouseHandler.IsMainMousedOver) CurrentStyleVariant = HoveredVariant;
                    else CurrentStyleVariant = BaseVariant;
                }
                // Otherwise, use the default style.
                else CurrentStyleVariant = BaseVariant;
            }
        }
        #endregion
    }
}
