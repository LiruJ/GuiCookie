using System.Collections.Generic;

namespace GuiCookie.Styles
{
    public class StyleAttributeCache<T> where T : class, IStyleAttribute
    {
        #region Fields
        private readonly Dictionary<StyleVariant, T> attributesByVariant = new Dictionary<StyleVariant, T>();
        #endregion

        #region Cache Functions
        public bool TryGetVariantAttribute(StyleVariant styleVariant, out T output) => attributesByVariant.TryGetValue(styleVariant, out output);

        public void Refresh(Style style)
        {
            // Clear the old cache.
            attributesByVariant.Clear();

            // If the new style is not null, cache the variants.
            if (style != null)
                foreach (StyleVariant variant in style.StyleVariantsByName.Values)
                    attributesByVariant.Add(variant, variant.GetFirstAttributeOfType<T>());
        }
        #endregion
    }
}
