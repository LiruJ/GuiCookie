using GuiCookie.Core.Components;
using GuiCookie.Core.Data;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Resources;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.Attributes;
using GuiCookie.Core.Styles.DataStructures;
using System.Drawing;

namespace Example.MonoGame.Components
{
    public class Light(ResourceManager resourceManager, StyleManager styleManager) : StyledComponent
    {
        #region Backing Fields
        private Style frameStyle;
        #endregion

        #region Properties
        public Image? GlowImage { get; set; }

        public TintedColour? OffColour { get; set; }

        public TintedColour GlowColour { get; set; }

        public bool IsOn { get; set; }

        public Style? FrameStyle
        {
            get => frameStyle;
            set
            {
                frameStyle = value;
                FrameStyleAttribute = frameStyle?.BaseVariant?.GetFirstAttributeOfType<SliceFrameStyleAttribute>();
            }
        }

        public SliceFrameStyleAttribute? FrameStyleAttribute { get; private set; }

        public Image? FrameImage { get; set; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            base.OnCreated(attributes);

            string? glowImageName = attributes.GetAttributeOrDefault("LightImage", (string?)null);
            GlowImage = !string.IsNullOrWhiteSpace(glowImageName) && resourceManager.ImagesByName.TryGetValue(glowImageName, out Image? lightImage) ? lightImage : null;

            string? frameImageName = attributes.GetAttributeOrDefault("LightFrameImage", (string?)null);
            FrameImage = !string.IsNullOrWhiteSpace(frameImageName) && resourceManager.ImagesByName.TryGetValue(frameImageName, out Image? lightFrameImage) ? lightFrameImage : null;

            GlowColour = new(resourceManager, attributes, "LightColour", "LightTint");
            if (!GlowColour.HasData)
                GlowColour = new(Color.White, null);

            OffColour = new(resourceManager, attributes, "LightOffColour", "LightOffTint");
            if (!OffColour.HasValue || !OffColour.Value.HasData)
                OffColour = null;

            IsOn = attributes.GetAttributeOrDefault("IsLightOn", false);

            if (attributes.TryGetAttribute("LightFrameStyle", out string? lightFrameStyle))
                FrameStyle = styleManager.GetStyleFromName(lightFrameStyle);
        }
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            base.Draw(guiCamera);

            if (IsOn)
                SliceFrameStyleAttribute.Draw(guiCamera, Bounds.AbsoluteTotalArea, null, GlowImage, GlowColour.Mixed);
            else if (OffColour.HasValue)
                SliceFrameStyleAttribute.Draw(guiCamera, Bounds.AbsoluteTotalArea, null, GlowImage, OffColour.Value.Mixed);

            FrameStyleAttribute?.DrawWithShadow(guiCamera, Bounds.AbsoluteTotalArea, FrameImage);
        }
        #endregion
    }
}
