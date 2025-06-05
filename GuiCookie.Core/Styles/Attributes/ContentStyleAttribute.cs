using GuiCookie.Core.Data;
using GuiCookie.Core.Helpers;
using GuiCookie.Core.Rendering;
using GuiCookie.Core.Styles.DataStructures;
using System.Drawing;

namespace GuiCookie.Core.Styles.Attributes
{
    public class ContentStyleAttribute : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";
        #endregion        

        #region Properties
        public string Name { get; }

        /// <summary> The colour and tint applied to the frame. </summary>
        public TintedColour TintedColour { get; private set; }

        /// <summary> Accessor for <see cref="TintedColour.Colour"/>. </summary>
        public Color? Colour => TintedColour.Colour;

        /// <summary> Accessor for <see cref="TintedColour.Tint"/>. </summary>
        public Color? Tint => TintedColour.Tint;

        /// <summary> Accessor for <see cref="TintedColour.Mixed"/>. </summary>
        public Color MixedColour => TintedColour.Mixed;

        /// <summary> The drop shadow data used to drop a shadow behind the content. </summary>
        public DropShadow DropShadow { get; private set; }
        #endregion

        #region Constructors
        public ContentStyleAttribute(ResourceManager resourceManager, IReadOnlyAttributeCollection attributes)
        {
            // Parse the colour.
            TintedColour = new TintedColour(resourceManager, attributes);

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty)!;

            // Set the drop shadow.
            DropShadow = new DropShadow(resourceManager, attributes);
        }

        private ContentStyleAttribute(ContentStyleAttribute original)
        {
            ArgumentNullException.ThrowIfNull(original);

            Name = original.Name;

            TintedColour = original.TintedColour;

            DropShadow = original.DropShadow;
        }
        #endregion

        #region Copy Functions
        public IStyleAttribute CreateCopy() => new ContentStyleAttribute(this);
        #endregion

        #region Combination Functions
        public void OverrideBaseAttribute(IStyleAttribute baseAttribute)
        {
#if DEBUG
            // Validity checks.
            ArgumentNullException.ThrowIfNull(baseAttribute);
            if (baseAttribute.Name != Name) throw new Exception($"Content name mismatch; {baseAttribute.Name}, {Name}");
#endif

            // Ensure the attribute is a content.
            if (baseAttribute is not ContentStyleAttribute baseContent)
                throw new ArgumentException($"Cannot combine with attribute as it is not a content. {baseAttribute}");

            // Override the properties.
            TintedColour = TintedColour.CreateCombination(baseContent.TintedColour, TintedColour);
            DropShadow = DropShadow.CreateCombination(baseContent.DropShadow, DropShadow);
        }
        #endregion

        #region Draw Functions
        public void Draw(IGuiCamera camera, Rectangle destination, Rectangle? source, Image image, TintedColour? tintedColour = null, DropShadow? dropShadow = null)
        {
            source ??= image.Source;

            dropShadow ??= DropShadow;
            if (dropShadow.Value.HasData)
            {
                Rectangle shadowDestination = new(PointExtensions.Add(destination.Location, DropShadow.Offset!.Value.ToPoint()), destination.Size);
                camera.DrawStretched(image, shadowDestination, source.Value, DropShadow.Colour!.Value);
            }

            Color colour = TintedColour.CalculateOverriddenColour(tintedColour);
            camera.DrawStretched(image, destination, source, colour);
        }
        #endregion
    }
}