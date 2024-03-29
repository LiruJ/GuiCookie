﻿using GuiCookie.DataStructures;
using GuiCookie.Rendering;
using GuiCookie.Styles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GuiCookie.Components
{
    public class ImageBlock : Component
    {
        #region Constants
        private const string imageAttributeName = "Image";
        private const string clippingModeAttributeName = "ClippingMode";
        private const string centredAttributeName = "Centred";
        #endregion

        #region Dependencies
        private readonly ResourceManager resourceManager;
        #endregion

        #region Fields
        private readonly StyleAttributeCache<Content> contentCache = new StyleAttributeCache<Content>();
        #endregion

        #region Properties
        public ClippingMode ClippingMode { get; set; }

        public bool Centred { get; set; }

        public Point Offset { get; private set; }

        public Vector2? Size { get; set; }

        public float Scale { get; private set; } = 1;

        public Texture2D Texture { get => Image.Texture; set => Image = new Image(value); }

        /// <summary> The current colour of the current content. </summary>
        public Color? Colour
        {
            get => contentCache.TryGetVariantAttribute(Style.BaseVariant, out Content content) ? content.Colour : null;
            set { if (contentCache.TryGetVariantAttribute(Style.BaseVariant, out Content content)) content.Colour = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public DropShadow DropShadow
        {
            get => contentCache.TryGetVariantAttribute(Style.BaseVariant, out Content content) ? content.DropShadow : new DropShadow((Vector2?)null, null);
            set { if (contentCache.TryGetVariantAttribute(Style.BaseVariant, out Content content)) content.DropShadow = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Tint
        {
            get => contentCache.TryGetVariantAttribute(Style.BaseVariant, out Content content) ? content.Tint : null;
            set { if (contentCache.TryGetVariantAttribute(Style.BaseVariant, out Content content)) content.Tint = value; }
        }

        /// <summary> The current image that is being displayed. </summary>
        public Image Image { get; set; }
        #endregion

        #region Constructors
        public ImageBlock(ResourceManager resourceManager)
        {
            // Set dependencies.
            this.resourceManager = resourceManager ?? throw new System.ArgumentNullException(nameof(resourceManager));
        }
        #endregion

        #region Initialisation Functions
        public override void OnCreated()
        {
            // Set the image.
            string imageName = Element.Attributes.GetAttributeOrDefault(imageAttributeName, string.Empty);
            if (!string.IsNullOrEmpty(imageName))
                SetImageFromName(imageName);

            // Set the colour and tint.
            if (contentCache.TryGetVariantAttribute(Style.BaseVariant, out Content content))
                content.TintedColour = TintedColour.CreateCombination(content.TintedColour, new TintedColour(Root.StyleManager.ResourceManager, Element.Attributes));

            // Set the drop shadow.
            DropShadow = DropShadow.CreateCombination(DropShadow, new DropShadow(Root.StyleManager.ResourceManager, Element.Attributes));

            // Set clipping mode.
            ClippingMode = Element.Attributes.GetEnumAttributeOrDefault(clippingModeAttributeName, ClippingMode.Squeeze);

            // Set centred.
            Centred = Element.Attributes.GetAttributeOrDefault(centredAttributeName, false);
        }
        #endregion

        #region Image Functions
        public void SetImageFromName(string name) 
            => Image = resourceManager.ImagesByName.TryGetValue(name, out Image image) ? image : throw new System.Exception($"Image with name {name} has not been loaded as an image resource.");

        public override void OnStyleChanged()
        {
            // Refresh the cache.
            contentCache.Refresh(Style);
        }
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            // If the current image is empty, don't draw.
            if (Texture == null) return;

            // Get the current content.
            contentCache.TryGetVariantAttribute(CurrentStyleVariant, out Content content);
            
            // Draw the image itself.
            drawImage(guiCamera, content);
        }

        private void drawImage(IGuiCamera guiCamera, Content content)
        {
            // Calculate the size.
            Vector2 size = Size ?? Element.Bounds.ContentSize.ToVector2();

            // Create rectangles for the target and source areas.
            Rectangle target, source = Image.Source;
            Point offset = Centred ? ((Element.Bounds.AbsoluteTotalArea.Size.ToVector2() / 2.0f) - (Image.Source.Size.ToVector2() / 2.0f)).ToPoint() : Point.Zero;

            // Set the offset.
            Offset = offset;

            // Draw the image based on the clipping mode.
            switch (ClippingMode)
            {
                case ClippingMode.None:
                    target = new Rectangle(Element.Bounds.AbsoluteContentPosition + offset, Image.Source.Size);
                    break;
                case ClippingMode.Clip:
                    // The source rectangle is the intersection between the desired source size and the actual content size.
                    source = Rectangle.Intersect(Image.Source, new Rectangle(Image.Source.Location - offset, size.ToPoint()));

                    // A new offset has to be calculated with the new source size.
                    Point clippedOffset = Centred ? ((size / 2.0f) - (source.Size.ToVector2() / 2.0f)).ToPoint() : Point.Zero;

                    // Set the offset.
                    Offset = clippedOffset;

                    target = new Rectangle(Element.Bounds.AbsoluteContentPosition + clippedOffset, source.Size);
                    break;
                case ClippingMode.Squeeze:
                    Point fittedSize = new Point(System.Math.Min(Image.Source.Width, (int)size.X), System.Math.Min(Image.Source.Height, (int)size.Y));

                    float ratioX = (float)fittedSize.X / Image.Source.Width;
                    float ratioY = (float)fittedSize.Y / Image.Source.Height;

                    Point squeezedSize = (ratioX < ratioY) ? new Point(fittedSize.X, (int)(Image.Source.Height * ratioX)) : new Point((int)(Image.Source.Width * ratioY), fittedSize.Y);

                    // Set the scale.
                    Scale = (ratioX < ratioY) ? ratioX : ratioY;

                    // A new offset has to be calculated with the squeezed size.
                    Point squeezedOffset = Centred ? ((size / 2.0f) - (squeezedSize.ToVector2() / 2.0f)).ToPoint() : Point.Zero;

                    // Set the offset.
                    Offset = squeezedOffset;

                    target = new Rectangle(Element.Bounds.AbsoluteContentPosition + squeezedOffset, squeezedSize);
                    break;
                case ClippingMode.Stretch:
                    target = new Rectangle(Element.Bounds.AbsoluteContentPosition, size.ToPoint());
                    Offset = Point.Zero;
                    break;
                default:
                    throw new System.Exception("Invalid clipping mode.");
            }

            // Draw the shadow first, if one exists.
            if (content != null && content.DropShadow.HasData)
                guiCamera.DrawTextureAt(Image.Texture, new Rectangle(target.Location + content.DropShadow.Offset.Value.ToPoint(), target.Size), source, content.DropShadow.Colour.Value);

            // Draw the image at the calculated target with the calculated source.
            guiCamera.DrawTextureAt(Image.Texture, target, source, content?.MixedColour ?? Color.White);
        }
        #endregion
    }
}
