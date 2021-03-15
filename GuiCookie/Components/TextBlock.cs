﻿using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Components
{
    public class TextBlock : Component
    {
        #region Constants
        private const string textAttributeName = "Text";
        private const string anchorAttributeName = "TextAnchor";
        private const string pivotAttributeName = "TextPivot";
        private const string resizeAttributeName = "ResizeDirection";
        private const string shadowColourAttributeName = "ShadowColour";
        private const string shadowOffsetAttributeName = "ShadowOffset";
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

        public Space TextAnchor { get; set; }

        public Space TextPivot { get; set; }

        public Vector2? DropShadowOffset { get; set; }

        public Color DropShadowColour { get; set; } = Color.Black;

        /// <summary> The current colour of the current font. </summary>
        public Color? Colour 
        {
            get => fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font) ? font.Colour : null;
            set { if (fontCache.TryGetVariantAttribute(CurrentStyleVariant, out Font font)) font.Colour = value; }
        }

        /// <summary> The direction </summary>
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

            // Set the text anchor and pivot.
            TextAnchor = Element.Attributes.GetAttributeOrDefault(anchorAttributeName, new Space(0.5f, Axes.Both), Space.TryParse);
            TextPivot = Element.Attributes.GetAttributeOrDefault(pivotAttributeName, new Space(0.5f, Axes.Both), Space.TryParse);
            ResizeDirection = Element.Attributes.GetEnumAttributeOrDefault(resizeAttributeName, DirectionMask.None);

            // Set the drop shadow.
            DropShadowOffset = Element.Attributes.GetAttributeOrDefault(shadowOffsetAttributeName, null) is string input && ToVector2.TryParse(input, out Vector2 output) ? output : (Vector2?)null;
            DropShadowColour = Element.Attributes.GetAttributeOrDefault(shadowColourAttributeName, Color.Black);
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
                Vector2 position = Bounds.AbsoluteContentPosition.ToVector2() + (TextAnchor.GetScaledSpace(Bounds.ContentSize.ToVector2()) - TextPivot.GetScaledSpace(TextSize));
                position.X = (float)Math.Floor(position.X);
                position.Y = (float)Math.Floor(position.Y);

                // If a drop shadow is to be drawn, draw it first.
                if (DropShadowOffset.HasValue)
                    guiCamera.DrawString(fontVariant.SpriteFont, Text, position + DropShadowOffset.Value, DropShadowColour);

                // Draw the text itself.
                guiCamera.DrawString(fontVariant.SpriteFont, Text, position, fontVariant.Colour ?? Color.Black);
            }
        }
        #endregion
    }
}
