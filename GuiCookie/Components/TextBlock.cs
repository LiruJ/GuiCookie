using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace GuiCookie.Components
{
    public class TextBlock : Component
    {
        #region Constants
        private const string textAttributeName = "Text";
        private const string resizeAttributeName = "ResizeDirection";
        #endregion

        #region Fields
        private readonly StyleAttributeCache<Font> fontCache = new StyleAttributeCache<Font>();
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
                // Set the text.
                text = value;

                // Recalculate the text properties.
                recalculateTextSizeProperties();
            }
        }

        /// <summary> The size in pixels that the text wants to use. </summary>
        public Vector2 TextSize { get; private set; }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Space? TextAnchor
        {
            get => fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font) ? font.TextAnchor : null;
            set { if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font)) font.TextAnchor = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Space? TextPivot
        {
            get => fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font) ? font.TextPivot : null;
            set { if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font)) font.TextPivot = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public DropShadow DropShadow
        {
            get => fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font) ? font.DropShadow : new DropShadow((Vector2?)null, null);
            set { if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font)) font.DropShadow = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Colour
        {
            get => fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font) ? font.Colour : null;
            set { if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font)) font.Colour = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Tint
        {
            get => fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font) ? font.Tint : null;
            set { if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font)) font.Tint = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Vector2? Offset
        {
            get => fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font) ? font.Offset : null;
            set { if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font)) font.Offset = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public SpriteFont SpriteFont
        {
            get => fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font) ? font.SpriteFont : null;
            set { if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font)) font.SpriteFont = value; }
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
        public override void OnCreated()
        {
            // Set the text.
            text = Element.Attributes.GetAttributeOrDefault(textAttributeName, string.Empty);

            // Set the resize direction.
            ResizeDirection = Element.Attributes.GetEnumAttributeOrDefault(resizeAttributeName, DirectionMask.None);

            // Set the text anchor and pivot.
            if (Element.Attributes.HasAttribute(Font.AnchorAttributeName)) TextAnchor = Element.Attributes.GetAttribute(Font.AnchorAttributeName, Space.Parse);
            if (Element.Attributes.HasAttribute(Font.PivotAttributeName)) TextPivot = Element.Attributes.GetAttribute(Font.PivotAttributeName, Space.Parse);

            // Set the drop shadow.
            DropShadow = DropShadow.CreateCombination(DropShadow, new DropShadow(Root.StyleManager.ResourceManager, Element.Attributes));

            // Set the offet.
            if (Element.Attributes.HasAttribute(Font.OffsetAttributeName)) Offset = Element.Attributes.GetAttribute(Font.OffsetAttributeName, ToVector2.Parse);

            // Set the colour and tint.
            if (fontCache.TryGetVariantAttribute(Style.BaseVariant, out Font font))
                font.TintedColour = TintedColour.CreateCombination(font.TintedColour, new TintedColour(Root.StyleManager.ResourceManager, Element.Attributes));

            if (Element.Attributes.HasAttribute(Font.FontAttributeName))
            {
                string fontName = Element.Attributes.GetAttributeOrDefault(Font.FontAttributeName, string.Empty);
                SpriteFont = !string.IsNullOrWhiteSpace(fontName) ?
                    Root.StyleManager.ResourceManager.FontsByName.TryGetValue(fontName, out SpriteFont spriteFont) ? spriteFont : throw new Exception($"Font resource named \"{fontName}\" does not exist.")
                    : null;
            }
        }

        public override void OnSetup()
        {
            recalculateTextSizeProperties();
        }
        #endregion

        #region Style Functions
        public override void OnStyleChanged()
        {
            // Refresh the font cache.
            fontCache.Refresh(Style);
        }

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
        public Vector2 CalculateSize(string text) => !string.IsNullOrWhiteSpace(text) && fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) 
            ? font.SpriteFont.MeasureString(text) 
            : Vector2.Zero;
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera) => DrawText(guiCamera, Text);

        public Vector2 CalculateTextPosition(Font fontVariant, Vector2 textSize)
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
            if (string.IsNullOrWhiteSpace(text) || !fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font fontVariant)) return;
            
            // Calculate the position of the text.
            Vector2 position = CalculateTextPosition(fontVariant, TextSize);

            // If a drop shadow is to be drawn, draw it first.
            if (fontVariant.DropShadow.HasData)
                guiCamera.DrawString(fontVariant.SpriteFont, text, position + fontVariant.DropShadow.Offset.Value, fontVariant.DropShadow.Colour.Value);

            // Draw the text itself.
            guiCamera.DrawString(fontVariant.SpriteFont, text, position, fontVariant.MixedColour);
        }

        public void DrawText(IGuiCamera guiCamera, StringBuilder text)
        {
            // Ensure there is text and a font to draw.
            if (text.Length == 0 || !fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font fontVariant)) return;
            
            // Calculate the position of the text.
            Vector2 position = CalculateTextPosition(fontVariant, fontVariant.SpriteFont.MeasureString(text));

            // If a drop shadow is to be drawn, draw it first.
            if (fontVariant.DropShadow.HasData)
                guiCamera.DrawString(fontVariant.SpriteFont, text, position + fontVariant.DropShadow.Offset.Value, fontVariant.DropShadow.Colour.Value);

            // Draw the text itself.
            guiCamera.DrawString(fontVariant.SpriteFont, text, position, fontVariant.MixedColour);
        }
        #endregion
    }
}
