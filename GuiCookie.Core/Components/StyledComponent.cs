using GuiCookie.Core.Data;
using GuiCookie.Core.Styles;
using LiruGameHelper.Signals;

namespace GuiCookie.Core.Components
{
    public abstract class StyledComponent : Component
    {
        #region Backing Fields
        private StyleStateMachine? styleStateMachine;

        private SignalConnection? styleChangedConnection = null;
        #endregion

        #region Properties
        public StyleStateMachine? StyleStateMachine
        {
            get => styleStateMachine;
            set
            {
                if (styleStateMachine == value)
                    return;

                styleStateMachine = value;
                styleChangedConnection?.Disconnect();
                styleChangedConnection = styleStateMachine?.OnStyleChanged?.Connect(OnStyleChanged);
            }
        }

        public Style? Style
        {
            get => StyleStateMachine?.Style;
            set
            {
                if (StyleStateMachine != null)
                    StyleStateMachine.Style = value;
            }
        }

        public StyleVariant? CurrentStyleVariant => StyleStateMachine?.CurrentStyleVariant;
        #endregion

        #region Style Functions
        public virtual void OnStyleChanged(Style? style) { }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            StyleStateMachine = Element.GetComponent<StyleStateMachine>();
            base.OnCreated(attributes);
        }
        #endregion
    }
}
