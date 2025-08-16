using Example.MonoGame.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.MonoGame.Elements
{
    public enum LightInteractionMode
    {
        Toggle = 0,
        Timer,
    }

    public class IndicatorLight : Element
    {
        #region Constants
        public const string InteractionModeAttributeName = "InteractionMode";

        public const string TimerAttributeName = "Timer";
        #endregion

        #region Components
        public Light Light { get; private set; }
        #endregion

        #region Properties
        public LightInteractionMode InteractionMode { get; set; } = LightInteractionMode.Toggle;

        public float TimerSeconds { get; set; } = 0f;

        public float CurrentTimerSeconds { get; set; } = 0f;
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            base.OnCreated(attributes);

            Light = GetComponent<Light>();
            InteractionMode = attributes.GetAttributeOrDefault(InteractionModeAttributeName, LightInteractionMode.Toggle, Enum.TryParse);

            if (attributes.TryGetAttribute(TimerAttributeName, out float timerSeconds, float.TryParse))
            {
                InteractionMode = LightInteractionMode.Timer;
                TimerSeconds = timerSeconds;
                Light.IsOn = false;
            }
        }
        #endregion

        #region Interaction Functions
        public void Interact()
        {
            switch (InteractionMode)
            {
                case LightInteractionMode.Toggle:
                    Light.IsOn = !Light.IsOn;
                    break;
                case LightInteractionMode.Timer:
                    Light.IsOn = true;
                    CurrentTimerSeconds = TimerSeconds;
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Update Functions
        protected override void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            base.Update(elapsedTime, totalTime);

            if (InteractionMode == LightInteractionMode.Timer && CurrentTimerSeconds > 0)
            {
                CurrentTimerSeconds -= (float)elapsedTime.TotalSeconds;
                if (CurrentTimerSeconds <= 0)
                    Light.IsOn = false;
            }
        }
        #endregion
    }
}
