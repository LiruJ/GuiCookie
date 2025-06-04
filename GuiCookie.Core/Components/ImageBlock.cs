using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Helpers;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Styles.Attributes;
using GuiCookie.Core.Styles.DataStructures;
using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Components
{
    public class ImageBlock(ResourceManager resourceManager) : StyledComponent
    {
        #region Constants
        private const string imageAttributeName = "Image";
        private const string clippingModeAttributeName = "ClippingMode";
        private const string centredAttributeName = "Centred";
        #endregion

        #region Fields
        private readonly StyleAttributeCache<ContentStyleAttribute> contentCache = new();
        #endregion

        #region Properties
        public ClippingMode ClippingMode { get; set; }

        public bool Centred { get; set; }

        public Point Offset { get; private set; }

        public Vector2? Size { get; set; }

        public float Scale { get; private set; } = 1;

        /// <summary> The current colour of the current content. </summary>
        public Color? Colour
        {
            get => contentCache.TryGetVariantAttribute(Style.BaseVariant, out ContentStyleAttribute? content) ? content!.Colour : null;
            set { if (contentCache.TryGetVariantAttribute(Style.BaseVariant, out ContentStyleAttribute? content)) content!.Colour = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public DropShadow DropShadow
        {
            get => contentCache.TryGetVariantAttribute(Style.BaseVariant, out ContentStyleAttribute? content) ? content!.DropShadow : new DropShadow((Vector2?)null, null);
            set { if (contentCache.TryGetVariantAttribute(Style.BaseVariant, out ContentStyleAttribute? content)) content!.DropShadow = value; }
        }

        /// <summary> A shortcut to the <see cref="Style.BaseVariant"/> property of the same name. </summary>
        public Color? Tint
        {
            get => contentCache.TryGetVariantAttribute(Style.BaseVariant, out ContentStyleAttribute? content) ? content!.Tint : null;
            set { if (contentCache.TryGetVariantAttribute(Style.BaseVariant, out ContentStyleAttribute? content)) content!.Tint = value; }
        }

        /// <summary> The current image that is being displayed. </summary>
        public Image? Image { get; set; }
        #endregion

        #region Initialisation Functions
        public override void OnCreated(IReadOnlyAttributeCollection attributes)
        {
            // Initialise the styled component first, so the style state machine can be set up.
            base.OnCreated(attributes);

            // Set the attributes.
            string imageName = attributes.GetAttributeOrDefault(imageAttributeName, string.Empty)!;
            if (!string.IsNullOrEmpty(imageName))
                SetImageFromName(imageName);
            if (contentCache.TryGetVariantAttribute(Style?.BaseVariant, out ContentStyleAttribute? content))
                content!.TintedColour = TintedColour.CreateCombination(content.TintedColour, new TintedColour(resourceManager, attributes));
            DropShadow = DropShadow.CreateCombination(DropShadow, new DropShadow(resourceManager, attributes));
            ClippingMode = attributes.GetEnumAttributeOrDefault(clippingModeAttributeName, ClippingMode.Squeeze);
            Centred = attributes.GetAttributeOrDefault(centredAttributeName, false);
        }
        #endregion

        #region Image Functions
        public void SetImageFromName(string name)
            => Image = resourceManager.ImagesByName.TryGetValue(name, out Image? image)
            ? image
            : throw new Exception($"Image with name {name} has not been loaded as an image resource.");

        public override void OnStyleChanged(Style? style) => contentCache.Refresh(Style);
        #endregion

        #region Draw Functions
        public override void Draw(IGuiCamera guiCamera)
        {
            // If the current image is empty, don't draw.
            if (Image == null)
                return;

            // Get the current content.
            if (!contentCache.TryGetVariantAttribute(CurrentStyleVariant, out ContentStyleAttribute? content))
                return;

            // Draw the image itself.
            drawImage(guiCamera, content!);
        }

        private void drawImage(IGuiCamera guiCamera, ContentStyleAttribute content)
        {
            if (Image == null)
                return;

            // Calculate the size.
            Size size = Size.HasValue ? Size.Value.ToSize() : (Size)Element.Bounds.ContentSize;

            // Create rectangles for the target and source areas.
            Rectangle target, source = Image.Source;
            Point offset = Centred ? (Element.Bounds.AbsoluteTotalArea.Size.ToVector2() / 2.0f - Image.Source.Size.ToVector2() / 2.0f).ToPoint() : new Point(0, 0);

            // Set the offset.
            Offset = offset;

            // Draw the image based on the clipping mode.
            switch (ClippingMode)
            {
                case ClippingMode.None:
                    target = new Rectangle(PointExtensions.Add(Element.Bounds.AbsoluteContentPosition, offset), Image.Source.Size);
                    break;
                case ClippingMode.Clip:
                    // The source rectangle is the intersection between the desired source size and the actual content size.
                    source = Rectangle.Intersect(Image.Source, new Rectangle(PointExtensions.Subtract(Image.Source.Location, offset), size));

                    // A new offset has to be calculated with the new source size.
                    Point clippedOffset = Centred ? ((size.ToVector2() / 2.0f) - (source.Size.ToVector2() / 2.0f)).ToPoint() : new Point(0, 0);

                    // Set the offset.
                    Offset = clippedOffset;

                    target = new Rectangle(PointExtensions.Add(Element.Bounds.AbsoluteContentPosition, clippedOffset), source.Size);
                    break;
                case ClippingMode.Squeeze:
                    Point fittedSize = new(Math.Min(Image.Source.Width, (int)size.Width), Math.Min(Image.Source.Height, (int)size.Height));

                    float ratioX = (float)fittedSize.X / Image.Source.Width;
                    float ratioY = (float)fittedSize.Y / Image.Source.Height;

                    Size squeezedSize = ratioX < ratioY ? new Size(fittedSize.X, (int)(Image.Source.Height * ratioX)) : new Size((int)(Image.Source.Width * ratioY), fittedSize.Y);

                    // Set the scale.
                    Scale = ratioX < ratioY ? ratioX : ratioY;

                    // A new offset has to be calculated with the squeezed size.
                    Point squeezedOffset = Centred ? ((size.ToVector2() / 2.0f) - (squeezedSize.ToVector2() / 2.0f)).ToPoint() : new Point(0, 0);

                    // Set the offset.
                    Offset = squeezedOffset;

                    target = new Rectangle(PointExtensions.Add(Element.Bounds.AbsoluteContentPosition, squeezedOffset), squeezedSize);
                    break;
                case ClippingMode.Stretch:
                    target = new Rectangle(Element.Bounds.AbsoluteContentPosition, size);
                    Offset = new Point(0, 0);
                    break;
                default:
                    throw new Exception("Invalid clipping mode.");
            }

            // Draw the shadow first, if one exists.
            if (content != null && content.DropShadow.HasData)
                guiCamera.DrawStretched(Image, new Rectangle(PointExtensions.Add(target.Location, content.DropShadow.Offset!.Value.ToPoint()), target.Size), source, content.DropShadow.Colour!.Value);

            // Draw the image at the calculated target with the calculated source.
            guiCamera.DrawStretched(Image, target, source, content?.MixedColour ?? Color.White);
        }
        #endregion
    }
}
