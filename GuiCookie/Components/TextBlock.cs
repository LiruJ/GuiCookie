using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

        private DirectionMask resizeElement = DirectionMask.None;
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

        public Space TextAnchor
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.TextAnchor : new Space(0.5f, Axes.Both);
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.TextAnchor = value; }
        }

        public Space TextPivot
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.TextPivot : new Space(0.5f, Axes.Both);
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.TextPivot = value; }
        }

        public Vector2? DropShadowOffset
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.DropShadowOffset : null;
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.DropShadowOffset = value; }
        }

        public Color DropShadowColour
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.DropShadowColour : Color.Black;
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.DropShadowColour = value; }
        }

        /// <summary> The current colour of the current font. </summary>
        public Color? Colour 
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.Colour : null;
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.Colour = value; }
        }

        public Color? Tint
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.Tint : null;
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.Tint = value; }
        }

        public Vector2 Offset
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.Offset : Vector2.Zero;
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.Offset = value; }
        }

        public SpriteFont SpriteFont
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.SpriteFont : null;
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.SpriteFont = value; }
        }

        /// <summary> The direction in which this text's element resizes. </summary>
        public DirectionMask ResizeDirection
        {
            get => resizeElement;
            set
            {
                resizeElement = value;

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
            if (Element.Attributes.HasAttribute(Font.ShadowOffsetAttributeName)) DropShadowOffset = Element.Attributes.GetAttribute(Font.ShadowOffsetAttributeName, ToVector2.Parse);
            if (Element.Attributes.HasAttribute(Font.ShadowColourAttributeName)) DropShadowColour = Root.StyleManager.ResourceManager.GetColourOrDefault(Element.Attributes, Font.ShadowColourAttributeName).Value;

            // Set the offet.
            if (Element.Attributes.HasAttribute(Font.OffsetAttributeName)) Offset = Element.Attributes.GetAttribute(Font.OffsetAttributeName, ToVector2.Parse);

            // Set the colour and tint.
            if (Element.Attributes.HasAttribute(ResourceManager.ColourAttributeName)) Colour = Root.StyleManager.ResourceManager.GetColourOrDefault(Element.Attributes, ResourceManager.ColourAttributeName);
            if (Element.Attributes.HasAttribute(Font.TintAttributeName)) Tint = Root.StyleManager.ResourceManager.GetColourOrDefault(Element.Attributes, Font.TintAttributeName);

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
        public override void Draw(IGuiCamera guiCamera)
        {
            // Ensure there is text and a font to draw.
            if (!string.IsNullOrWhiteSpace(Text) && fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font fontVariant))
            {
                // Calculate the position based on the anchor and pivot. Round this down to avoid blurry text.
                Vector2 position = (Bounds.AbsoluteContentPosition.ToVector2() + (fontVariant.TextAnchor.GetScaledSpace(Bounds.ContentSize.ToVector2()) - fontVariant.TextPivot.GetScaledSpace(TextSize))) + fontVariant.Offset;
                position.X = (float)Math.Floor(position.X);
                position.Y = (float)Math.Floor(position.Y);

                // If a drop shadow is to be drawn, draw it first.
                if (DropShadowOffset.HasValue)
                    guiCamera.DrawString(fontVariant.SpriteFont, Text, position + fontVariant.DropShadowOffset.Value, fontVariant.DropShadowColour);

                // Draw the text itself.
                guiCamera.DrawString(fontVariant.SpriteFont, Text, position, fontVariant.FinalColour);
            }
        }
        #endregion
    }
}
