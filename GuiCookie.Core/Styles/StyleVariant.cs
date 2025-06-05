using GuiCookie.Core.Data;
using GuiCookie.Core.Styles.Attributes;
using LiruGameHelper.Reflection;

namespace GuiCookie.Core.Styles
{
    public class StyleVariant
    {
        #region Fields
        /// <summary> The collection of style attributes keyed by a combination of the type and its name. </summary>
        private readonly Dictionary<string, IStyleAttribute> styleAttributesByTypeAndName = [];

        private readonly Dictionary<Type, IStyleAttribute> unnamedStyleAttributesByType = [];
        #endregion

        #region Properties
        /// <summary> The name of this variant, e.g. "Hovered". </summary>
        public string Name { get; }
        #endregion

        #region Constructors
        public StyleVariant(string name, IReadOnlyList<IStyleAttribute> styleAttributes)
        {
            // Set the name.
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Variant name cannot be empty.", nameof(name));

            // Ensure the attributes exist.
            ArgumentNullException.ThrowIfNull(styleAttributes);

            // Add each style attribute.
            foreach (IStyleAttribute styleAttribute in styleAttributes)
                AddAttribute(styleAttribute);
        }

        private StyleVariant(StyleVariant original)
        {
            ArgumentNullException.ThrowIfNull(original);

            Name = original.Name;

            // Copy each attribute.
            foreach (IStyleAttribute attribute in original.styleAttributesByTypeAndName.Values)
                AddAttribute(attribute.CreateCopy());
        }
        #endregion

        #region Collection Functions
        public T? GetUnnamedAttributeOfType<T>() where T : class, IStyleAttribute => unnamedStyleAttributesByType.TryGetValue(typeof(T), out IStyleAttribute? styleAttribute) ? (T)styleAttribute : null;

        public T? GetNamedAttributeOfType<T>(string name) where T : class, IStyleAttribute
            => styleAttributesByTypeAndName.TryGetValue(calculateKey(typeof(T), name), out IStyleAttribute? styleAttribute) ? styleAttribute as T : null;

        /// <summary> Gets the first attribute of the type <typeparamref name="T"/>, prioritising unnamed attributes. </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetFirstAttributeOfType<T>() where T : class, IStyleAttribute
        {
            // Search the unnamed collection first.
            if (GetUnnamedAttributeOfType<T>() is T attribute) return attribute;

            // If the unnamed collection does not contain the type, return the first instance of the type within the main collection.
            foreach (IStyleAttribute namedAttribute in styleAttributesByTypeAndName.Values)
                if (namedAttribute is T typedAttribute) 
                    return typedAttribute;

            // Otherwise; return null.
            return null;
        }
        #endregion

        #region Add Functions
        public void AddAttribute(IStyleAttribute styleAttribute)
        {
            // Calculate the key.
            Type type = styleAttribute.GetType();
            string key = calculateKey(type, styleAttribute);

            // Try to add the attribute to the collection.
            if (!styleAttributesByTypeAndName.TryAdd(key, styleAttribute)) 
                throw new Exception($"Style attribute with key {key} has already been added to style variant {Name}.");

            // If the attribute has no name, add it to the typed collection. No two unnamed attributes of the same type can exist at this point, so no contains check is required.
            if (string.IsNullOrWhiteSpace(styleAttribute.Name))
                unnamedStyleAttributesByType.Add(type, styleAttribute);
        }

        private static string calculateKey(Type type, IStyleAttribute styleAttribute) => calculateKey(type, styleAttribute?.Name);

        private static string calculateKey(Type type, string? name) => $"{type.Name}:{name}";
        #endregion

        #region Copy Functions
        public StyleVariant CreateCopy() => new(this);
        #endregion

        #region Combination Functions
        public void CombineOverBase(StyleVariant baseVariant)
        {
            // Go over each attribute in the base.
            foreach (IStyleAttribute baseAttribute in baseVariant.styleAttributesByTypeAndName.Values)
            {
                // If this variant does not have the attribute, take a copy of it as it is.
                string key = calculateKey(baseAttribute.GetType(), baseAttribute);
                if (!styleAttributesByTypeAndName.TryGetValue(key, out IStyleAttribute? derivedAttribute)) 
                    AddAttribute(baseAttribute.CreateCopy());
                // Otherwise; combine the derived attribute over the base attribute.
                else derivedAttribute.OverrideBaseAttribute(baseAttribute);
            }
        }
        #endregion

        #region Load Functions
        public static StyleVariant Load(IReadOnlySheetDataNode variantNode, ResourceManager resourceManager, ConstructorCache<IStyleAttribute> attributeCache)
        {
            List<IStyleAttribute> attributes = [];
            foreach (IReadOnlySheetDataNode childNode in variantNode.ChildNodes)
            {
                // Create the attribute.
                IStyleAttribute styleAttribute = attributeCache.CreateInstance(childNode.Name + "StyleAttribute", resourceManager, childNode.Attributes.CreateCopy());

                // Try to add the attributes to the collection.
                attributes.Add(styleAttribute);
            }

            return new StyleVariant(variantNode.Name, attributes);
        }
        #endregion

        #region String Functions
        public override string ToString() => Name;
        #endregion
    }
}