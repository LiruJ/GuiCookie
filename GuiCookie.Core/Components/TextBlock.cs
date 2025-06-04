using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Helpers;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.Attributes;
using GuiCookie.Core.Styles.DataStructures;
using LiruGameHelper.Parsers;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace GuiCookie.Core.Components
{
    public class TextBlock(ResourceManager resourceManager) : StyledComponent
    {
        #region Constants
        private const string textAttributeName = "Text";
        private const string resizeAttributeName = "ResizeDirection";
        #endregion

        #region Fields
        private readonly StyleAttributeCache<FontStyleAttribute> fontCache = new();
        #endregion

        #region Backing Fields
        private string text = string.Empty;

        private DirectionMask resizeDirection = DirectionMask.None;
        #endregion

        #region Properties
        /// <summary> The text to display. </summary>
        public string Text
        {
            get => text;
            set
            {
                text = value;
                recalculateTextSizeProperties();
            }
        }

        /// <summary> The size in pixels that the text wants to use. </summary>
        public Vector2 TextSize { get; private set; }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Space? TextAnchor
        {
            get => fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font) ? font!.TextAnchor : null;
            //set { if (fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font)) font!.TextAnchor = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Space? TextPivot
        {
            get => fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font) ? font!.TextPivot : null;
            //set { if (fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font)) font!.TextPivot = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public DropShadow DropShadow
        {
            get => fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font) ? font!.DropShadow : new DropShadow((Vector2?)null, null);
            //set { if (fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font)) font!.DropShadow = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Colour
        {
            get => fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font) ? font!.Colour : null;
            //set { if (fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font)) font!.Colour = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Tint
        {
            get => fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font) ? font!.Tint : null;
            //set { if (fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font)) font!.Tint = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Vector2? Offset
        {
            get => fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font) ? font!.Offset : null;
            //set { if (fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? font)) font!.Offset = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Font? Font
        {
            get => fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? fontStyle) ? fontStyle!.Font : null;
            //set { if (fontCache.TryGetVariantAttribute(Style?.BaseVariant, out FontStyleAttribute? fontStyle)) fontStyle!.Font = value; }
        }

        /// <summary> The direction in which this text's element resizes. </summary>
        public DirectionMask ResizeDirection
        {
            get => resizeDirection;
            set
            {
                resizeDirection = value;

                recalculateTextSizeProperties();
            }
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Set the attributes.
            text = attributes.GetAttributeOrDefault(textAttributeName, string.Empty)!;
            ResizeDirection = attributes.GetEnumAttributeOrDefault(resizeAttributeName, DirectionMask.None);
            //if (attributes.TryGetAttribute(FontStyleAttribute.AnchorAttributeName, out Space anchor, Space.TryParse))
            //    TextAnchor = anchor;
            //if (attributes.TryGetAttribute(FontStyleAttribute.PivotAttributeName, out Space pivot, Space.TryParse))
            //    TextPivot = pivot;
            //DropShadow = DropShadow.CreateCombination(DropShadow, new DropShadow(resourceManager, attributes));
            //if (attributes.TryGetAttribute(FontStyleAttribute.OffsetAttributeName, out Vector2 offset, ToVector.TryParse))
            //    Offset = offset;
            //if (fontCache.TryGetVariantAttribute(StyleStateMachine?.Style?.BaseVariant, out FontStyleAttribute? font))
            //    font!.TintedColour = TintedColour.CreateCombination(font.TintedColour, new TintedColour(resourceManager, attributes));

            // Try load the font, if one was defined.
            //if (attributes.TryGetAttribute(FontStyleAttribute.FontAttributeName, out string? fontName))
            //    Font = !string.IsNullOrWhiteSpace(fontName) ?
            //        resourceManager.FontsByName.TryGetValue(fontName, out Font? spriteFont) ? spriteFont : throw new Exception($"Font resource named \"{fontName}\" does not exist.")
            //        : null;
        }

        public override void OnSetup(IReadOnlyAttributeCollection attributes)
        {
            recalculateTextSizeProperties();
        }
        #endregion

        #region Style Functions
        public override void OnStyleChanged(Style? style) => fontCache.Refresh(style);

        private void recalculateTextSizeProperties()
        {
            // Calculate and save the text size.
            TextSize = CalculateSize(Text);

            // If the element's bounds have not been set up yet, do nothing.
            if ((Element.InitialisationState & InitialisationState.Setup) != InitialisationState.Setup)
                return;

            // If the text size should resize the containing element, do so.
            if (ResizeDirection != DirectionMask.None && TextSize.X > 0 && TextSize.Y > 0)
                Bounds.ContentSize = new Point(
                    (ResizeDirection & DirectionMask.Horizontal) == DirectionMask.Horizontal ? (int)Math.Ceiling(TextSize.X) : Bounds.ContentSize.X,
                    (ResizeDirection & DirectionMask.Vertical) == DirectionMask.Vertical ? (int)Math.Ceiling(TextSize.Y) : Bounds.ContentSize.Y);
        }
        #endregion

        #region Calculation Functions
        public Vector2 CalculateSize(string text) 
            => !string.IsNullOrWhiteSpace(text) && fontCache.TryGetVariantAttribute(CurrentStyleVariant, out FontStyleAttribute? font)
                ? font!.Font.MeasureString(text)
                : Vector2.Zero;
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera) => DrawText(guiCamera, Text);

        public Vector2 CalculateTextPosition(FontStyleAttribute fontVariant, Vector2 textSize)
        {
            // Get the anchor and pivot, defaulting to the centre.
            Space textAnchor = fontVariant.TextAnchor ?? new Space(0.5f, Axes.Both);
            Space textPivot = fontVariant.TextPivot ?? new Space(0.5f, Axes.Both);

            // Calculate the position based on the anchor and pivot. Round this down to avoid blurry text.
            Vector2 position = Bounds.AbsoluteContentPosition.ToVector2() + (textAnchor.GetScaledSpace(Bounds.ContentSize.ToVector2()) - textPivot.GetScaledSpace(textSize))
                               + (fontVariant.Offset ?? Vector2.Zero);
            position.X = (float)Math.Floor(position.X);
            position.Y = (float)Math.Floor(position.Y);
            return position;
        }

        public void DrawText(IGuiCamera guiCamera, string text)
        {
            // Ensure there is text and a font to draw.
            if (string.IsNullOrWhiteSpace(text) || !fontCache.TryGetVariantAttribute(CurrentStyleVariant, out FontStyleAttribute? fontVariant)) 
                return;

            // Calculate the position of the text.
            Vector2 position = CalculateTextPosition(fontVariant!, TextSize);

            // If a drop shadow is to be drawn, draw it first.
            if (fontVariant!.DropShadow.HasData)
                guiCamera.DrawString(fontVariant.Font, text, position + fontVariant.DropShadow.Offset!.Value, fontVariant.DropShadow.Colour!.Value);

            // Draw the text itself.
            guiCamera.DrawString(fontVariant.Font, text, position, fontVariant.MixedColour);
        }

        public void DrawText(IGuiCamera guiCamera, StringBuilder text)
        {
            // Ensure there is text and a font to draw.
            if (text.Length == 0 || !fontCache.TryGetVariantAttribute(CurrentStyleVariant, out FontStyleAttribute? fontVariant))
                return;

            // Calculate the position of the text.
            Vector2 position = CalculateTextPosition(fontVariant!, fontVariant!.Font.MeasureString(text));

            // If a drop shadow is to be drawn, draw it first.
            if (fontVariant.DropShadow.HasData)
                guiCamera.DrawString(fontVariant.Font, text, position + fontVariant.DropShadow.Offset.Value, fontVariant.DropShadow.Colour.Value);

            // Draw the text itself.
            guiCamera.DrawString(fontVariant.Font, text, position, fontVariant.MixedColour);
        }
        #endregion
    }
}
