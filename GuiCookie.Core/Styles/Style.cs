using GuiCookie.Core.Data;
using GuiCookie.Core.Resources;
using GuiCookie.Core.Styles.Attributes;
using LiruGameHelper.Reflection;

namespace GuiCookie.Core.Styles
{
    /// <summary> Represents the parameters of an element's style. </summary>
    public class Style
    {
        #region Constants
        public const string BaseVariantName = "Base";

        public const string HoveredVariantName = "Hovered";

        public const string ClickedVariantName = "Clicked";

        public const string DisabledVariantName = "Disabled";
        #endregion

        #region Fields
        private readonly Dictionary<string, StyleVariant> styleVariantsByName = [];
        #endregion

        #region Properties
        /// <summary> The name of this <see cref="Style"/>. </summary>
        public string Name { get; }

        /// <summary> The name of the <see cref="Style"/> that this style inherits from. </summary>
        public string? BaseStyleName { get; }

        /// <summary> The base <see cref="StyleVariant"/> with no changes applied. </summary>
        public StyleVariant BaseVariant { get; }

        /// <summary> The read-only collection of style variants. </summary>
        public IReadOnlyDictionary<string, StyleVariant> StyleVariantsByName => styleVariantsByName;
        #endregion

        #region Constructors
        public Style(string name, string? baseStyleName, StyleVariant baseVariant)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            BaseStyleName = baseStyleName;
            BaseVariant = baseVariant;
            AddVariant(baseVariant);
        }
        #endregion

        #region Collection Functions
        public StyleVariant? GetStyleVariantFromName(string name) => styleVariantsByName.TryGetValue(name, out StyleVariant? styleVariant) ? styleVariant : null;

        /// <summary> Adds the given <paramref name="variant"/> to this style keyed by name. </summary>
        /// <param name="variant"> The variant to add. </param>
        public void AddVariant(StyleVariant variant)
        {
            // Add the variant to the dictionary using its name.
            if (!styleVariantsByName.TryAdd(variant.Name, variant))
                throw new Exception($"Style variant with name {variant.Name} has already been defined for style {Name}.");
        }
        #endregion

        #region Combination Functions
        public void CombineWithBase(Style baseStyle)
        {
            // Go over each style variant in the base style.
            foreach (StyleVariant baseVariant in baseStyle.styleVariantsByName.Values)
            {
                // Try get the variant from this style, if it exists, override the base with it.
                if (styleVariantsByName.TryGetValue(baseVariant.Name, out StyleVariant? derivedVariant))
                    derivedVariant.CombineOverBase(baseVariant);
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

        #region Load Functions
        public static Style Load(ResourceManager resourceManager, ConstructorCache<IStyleAttribute> attributeCache, IReadOnlySheetDataNode styleNode)
        {
            // Hold collections of the loaded variants and attributes.
            List<StyleVariant> variants = [];
            List<IStyleAttribute> styleAttributes = [];

            // Read each child node.
            foreach (IReadOnlySheetDataNode childNode in styleNode.ChildNodes)
            {
                // TODO: Be smarter with attribute names. ConstructorCache needs a way to find if a type with a given name exists. Try with just the node name, then append "StyleAttribute" if it doesn't exist".
                // If the child node has children, load it as a variant.
                if (childNode.ChildNodes.Count != 0)
                    variants.Add(StyleVariant.Load(childNode, resourceManager, attributeCache));
                // Otherwise; dynamically create the style attribute and add it to the list.
                else
                    styleAttributes.Add(attributeCache.CreateInstance(childNode.Name + "StyleAttribute", resourceManager, childNode.Attributes.CreateCopy()));
            }
            
            // Set the name of the base style if one was given.
            string? baseStyleName = styleNode.Attributes.GetAttributeOrDefault(BaseVariantName, (string?)null);

            // Create the base variant using the loaded attributes.
            StyleVariant baseVariant = new(BaseVariantName, styleAttributes);
            Style style = new(styleNode.Name, baseStyleName, baseVariant);

            // Create the derived variants using the base variant.
            foreach (StyleVariant variant in variants)
            {
                // Combine the variant with the base.
                variant.CombineOverBase(baseVariant);

                // Add the variant to the dictionary using its name.
                style.AddVariant(variant);
            }

            return style;
        }
        #endregion

        #region String Functions
        public override string ToString() => Name;
        #endregion
    }
}