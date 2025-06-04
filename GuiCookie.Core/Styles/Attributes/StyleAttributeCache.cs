namespace GuiCookie.Core.Styles.Attributes
{
    public class StyleAttributeCache<T> where T : class, IStyleAttribute
    {
        #region Fields
        private readonly Dictionary<StyleVariant, T> attributesByVariant = [];
        #endregion

        #region Properties
        /// <summary> The name of the style attribute to use. If this is <c>null</c> then unnamed attributes are used instead. </summary>
        public string? Name { get; set; }
        #endregion

        #region Constructors
        public StyleAttributeCache() { }

        public StyleAttributeCache(string name) => Name = name;
        #endregion

        #region Cache Functions
        public bool TryGetVariantAttribute(StyleVariant? styleVariant, out T? output)
        {
            output = null;
            if (styleVariant == null) 
                return false;
            return attributesByVariant.TryGetValue(styleVariant, out output) && output != null;
        }

        public void Refresh(Style? style)
        {
            // Clear the old cache.
            attributesByVariant.Clear();

            if (style == null)
                return;

            // If the new style is not null, cache the variants.
            foreach (StyleVariant variant in style.StyleVariantsByName.Values)
                attributesByVariant.Add(variant, string.IsNullOrWhiteSpace(Name) ? variant.GetFirstAttributeOfType<T>() : variant.GetNamedAttributeOfType<T>(Name));
        }
        #endregion
    }
}
