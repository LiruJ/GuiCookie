using GuiCookie.Attributes;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.Styles
{
    public class Content : IStyleAttribute
    {
        #region Constants
        private const string nameAttributeName = "Name";
        #endregion        

        #region Properties
        public string Name { get; }

        /// <summary> The tint applied to content (image blocks, etc.). </summary>
        public Color? Colour { get; set; }
        #endregion

        #region Constructors
        public Content(ResourceManager resourceManager, IReadOnlyAttributes attributes)
        {
            // Set the colour.
            Colour = resourceManager.GetColourOrDefault(attributes, ResourceManager.ColourAttributeName);

            // Set the name.
            Name = attributes.GetAttributeOrDefault(nameAttributeName, string.Empty);
        }

        private Content(Content original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            Colour = original.Colour;
        }
        #endregion

        #region Copy Functions
        public IStyleAttribute CreateCopy() => new Content(this);
        #endregion

        #region Combination Functions
        public void OverrideBaseAttribute(IStyleAttribute baseAttribute)
        {
#if DEBUG
            // Validity checks.
            if (baseAttribute == null) throw new ArgumentNullException(nameof(baseAttribute));
            if (baseAttribute.Name != Name) throw new Exception($"Content name mismatch; {baseAttribute.Name}, {Name}");
#endif

            // Ensure the attribute is a content.
            if (!(baseAttribute is Content baseContent)) throw new ArgumentException($"Cannot combine with attribute as it is not a content. {baseAttribute}");

            // Override the colour.
            if (Colour == null) Colour = baseContent.Colour;
        }
        #endregion
    }
}
