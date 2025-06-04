using GuiCookie.Core.Data;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Styles;
using LiruGameHelper.Signals;

namespace GuiCookie.Core.Components
{
    public class StyleStateMachine(StyleManager? styleManager) : Component
    {
        #region Constants
        public const string StyleAttributeName = "Style";
        #endregion

        #region Fields
        private Style? style;

        private string? styleName;
        #endregion

        #region Cached Variants
        /// <summary> The base variant of the current <see cref="Style"/>. </summary>
        public StyleVariant? BaseVariant { get; protected set; }

        /// <summary> The variant of the current <see cref="Style"/> that is used when the element is moused over, defaulting to <see cref="BaseVariant"/> if none was defined. </summary>
        /// <remarks> The element needs a <see cref="Components.MouseHandler"/> in order for this variant to be used. </remarks>
        public StyleVariant? HoveredVariant { get; protected set; }

        /// <summary> The variant of the current <see cref="Style"/> that is used when the element is clicked, defaulting to <see cref="BaseVariant"/> if none was defined. </summary>
        /// <remarks> The element needs a <see cref="Components.MouseHandler"/> in order for this variant to be used. </remarks>
        public StyleVariant? ClickedVariant { get; protected set; }

        /// <summary> The variant of the current <see cref="Style"/> that is used when the element is disabled, defaulting to <see cref="BaseVariant"/> if none was defined. </summary>
        public StyleVariant? DisabledVariant { get; protected set; }
        #endregion

        #region Properties
        /// <summary> The current style variant according to the state of the element. </summary>
        public StyleVariant? CurrentStyleVariant { get; protected set; }

        /// <summary> The style to work with. </summary>
        public Style? Style
        {
            get => style;
            set
            {
                // Set the style.
                style = value;
                styleName = style?.Name;

                // Refresh the current variant.
                RefreshStyleVariants();
            }
        }

        public string? StyleName
        {
            get => styleName;
            set
            {
                if (styleManager == null || value == styleName)
                    return;

                if (!string.IsNullOrWhiteSpace(value) && styleManager.StylesByName.TryGetValue(value, out Style? style))
                {
                    styleName = value;
                    this.style = style;
                }
                else
                {
                    styleName = null;
                    this.style = null;
                }

                // Refresh the current variant.
                RefreshStyleVariants();
            }
        }

        /// <summary> The mouse handler that determines what variant to use. This defaults to the element's mouse handler, but can be set to any element's mouse handler. </summary>
        public MouseHandler? MouseHandler { get; set; }

        public bool IsCurrentVariantBase => CurrentStyleVariant != null && CurrentStyleVariant == BaseVariant;
        #endregion

        #region Signals
        public IConnectableSignal<Style?> OnStyleChanged => onStyleChanged;
        private readonly Signal<Style?> onStyleChanged = new();
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            MouseHandler = Element.GetComponent<MouseHandler>();

            StyleName = attributes.GetAttributeOrDefault(StyleAttributeName, (string?)null);

            base.OnCreated(attributes);
        }
        #endregion

        #region Change Functions
        /// <summary> Searches through the current <see cref="Style"/> and caches any variants. </summary>
        public void RefreshStyleVariants()
        {
            // If a new style exists, cache the variants.
            BaseVariant = Style?.BaseVariant;
            HoveredVariant = Style?.GetStyleVariantFromName(Style.HoveredVariantName) ?? BaseVariant;
            ClickedVariant = Style?.GetStyleVariantFromName(Style.ClickedVariantName) ?? BaseVariant;
            DisabledVariant = Style?.GetStyleVariantFromName(Style.DisabledVariantName) ?? BaseVariant;

            // Refresh the current variant.
            UpdateCurrentStyle();

            // Tell the element that its style was changed.
            //Element.onStyleChanged();
            onStyleChanged.Invoke(Style);
        }
        #endregion

        #region Update Functions
        public override void Update(TimeSpan elapsedTime, TimeSpan totalTime) => UpdateCurrentStyle();
        
        public virtual void UpdateCurrentStyle()
        {
            // If the style is null, then the current style variant is also null.
            if (Style == null)
                CurrentStyleVariant = null;

            // First, if the element is disabled, change the style to disabled.
            if (!Element.Enabled) 
                CurrentStyleVariant = DisabledVariant;
            // Otherwise; if the mouse has clicked the element, change the style of the state machine.
            else
            {
                // If the element has a mouse handler, use it for the state.
                if (MouseHandler != null)
                {
                    if (MouseHandler.IsLeftClicked || MouseHandler.IsRightClicked) 
                        CurrentStyleVariant = ClickedVariant;
                    else if (MouseHandler.IsMainMousedOver) 
                        CurrentStyleVariant = HoveredVariant;
                    else
                        CurrentStyleVariant = BaseVariant;
                }
                // Otherwise, use the default style.
                else 
                    CurrentStyleVariant = BaseVariant;
            }
        }
        #endregion
    }
}
