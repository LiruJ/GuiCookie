using GuiCookie.Attributes;
using LiruGameHelper.Reflection;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GuiCookie.Styles
{
    /// <summary> Represents the parameters of an <see cref="Elements"/>'s style. </summary>
    public class Style
    {
        #region Constants
        public const string BaseVariantName = "Base";

        public const string HoveredVariantName = "Hovered";

        public const string ClickedVariantName = "Clicked";

        public const string DisabledVariantName = "Disabled";
        #endregion

        #region Fields
        private readonly Dictionary<string, StyleVariant> styleVariantsByName = new Dictionary<string, StyleVariant>();
        #endregion

        #region Properties
        /// <summary> The name of this <see cref="Style"/>. </summary>
        public string Name { get; }

        /// <summary> The name of the <see cref="Style"/> that this style inherits from. </summary>
        public string BaseStyleName { get; }

        /// <summary> The base <see cref="StyleVariant"/> with no changes applied. </summary>
        public StyleVariant BaseVariant { get; }

        /// <summary> The read-only collection of style variants. </summary>
        public IReadOnlyDictionary<string, StyleVariant> StyleVariantsByName => styleVariantsByName;
        #endregion

        #region Constructors
        /// <summary> Creates a style from the given <paramref name="styleNode"/>. </summary>
        /// <param name="styleNode"> The <see cref="XmlNode"/> that contains the element style. </param>
        public Style(ResourceManager resourceManager, ConstructorCache<IStyleAttribute> attributeCache, XmlNode styleNode)
        {
            // Set the name of this style based on the name of the node.
            Name = styleNode.Name;

            // Create attributes from this style node.
            IReadOnlyAttributes attributes = new AttributeCollection(styleNode);

            // Set the name of the base style if one was given.
            BaseStyleName = attributes.GetAttributeOrDefault(BaseVariantName, string.Empty);

            // Hold collections of the loaded variants and attributes.
            List<StyleVariant> variants = new List<StyleVariant>();
            List<IStyleAttribute> styleAttributes = new List<IStyleAttribute>();

            // Read each child node.
            foreach (XmlNode childNode in styleNode)
            {
                // If the child node has children, load it as a variant.
                if (childNode.HasChildNodes) variants.Add(new StyleVariant(childNode, resourceManager, attributeCache));
                // Otherwise; dynamically create the style attribute and add it to the list.
                else styleAttributes.Add(attributeCache.CreateInstance(childNode.Name, resourceManager, new AttributeCollection(childNode)));
            }

            // Create the base variant using the loaded attributes.
            BaseVariant = new StyleVariant(BaseVariantName, styleAttributes);
            AddVariant(BaseVariant);

            // Create the derived variants using the base variant.
            foreach (StyleVariant variant in variants)
            {
                // Combine the variant with the base.
                variant.CombineOverBase(BaseVariant);

                // Add the variant to the dictionary using its name.
                AddVariant(variant);
            }
        }

        private Style(Style original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));

            Name = original.Name;
            BaseStyleName = original.BaseStyleName;

            // Copy each variant over.
            foreach (StyleVariant variant in original.styleVariantsByName.Values)
                AddVariant(variant.CreateCopy());

            // Set the base variant to the base variant from the newly populated dictionary, ensuring it's the new copy and not the original.
            BaseVariant = styleVariantsByName[BaseVariantName];
        }
        #endregion

        #region Collection Functions
        public StyleVariant GetStyleVariantFromName(string name) => styleVariantsByName.TryGetValue(name, out StyleVariant styleVariant) ? styleVariant : null;

        /// <summary> Adds the given <paramref name="variant"/> to this style keyed by name. </summary>
        /// <param name="variant"> The variant to add. </param>
        public void AddVariant(StyleVariant variant)
        {
            // Add the variant to the dictionary using its name.
            if (!styleVariantsByName.ContainsKey(variant.Name)) styleVariantsByName.Add(variant.Name, variant);
            else throw new Exception($"Style variant with name {variant.Name} has already been defined for style {Name}.");
        }
        #endregion

        #region Copy Functions
        public Style CreateCopy() => new Style(this);
        #endregion

        #region Combination Functions
        public void CombineWithBase(Style baseStyle)
        {
            // Go over each style variant in the base style.
            foreach (StyleVariant baseVariant in baseStyle.styleVariantsByName.Values)
            {
                // Try get the variant from this style, if it exists, override the base with it.
                if (styleVariantsByName.TryGetValue(baseVariant.Name, out StyleVariant derivedVariant)) derivedVariant.CombineOverBase(baseVariant);
                // Otherwise; copy it from the base to the derived style as-is.
                else
                {
                    // Copy the variant from the base style.
                    StyleVariant baseVariantCopy = baseVariant.CreateCopy();

                    // Combine the copy with the base variant of this style, filling in any gaps.
                    baseVariantCopy.CombineOverBase(BaseVariant);

                    // Add the new copy.
                    AddVariant(baseVariantCopy);
                }
            }
        }
        #endregion

        #region String Functions
        public override string ToString() => Name;
        #endregion
    }
}
