using GuiCookie.Core.DataStructures;
using LiruGameHelper.Parsers;
using System.Collections;
using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Data
{
    public class ReadOnlyDerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes, IReadOnlyAttributeCollection derivedAttributes) : IReadOnlyAttributeCollection
    {
        #region Indexers
        public string? this[string name] 
            => DerivedAttributes.TryGetAttribute(name, out string? value)
                ? value
                : BaseAttributes.TryGetAttribute(name, out value)
                    ? value
                    : null;
        #endregion

        #region Properties
        public IReadOnlyAttributeCollection BaseAttributes { get; } = baseAttributes;

        public IReadOnlyAttributeCollection DerivedAttributes { get; } = derivedAttributes;

        public int Count => BaseAttributes.Union(DerivedAttributes).Count();
        #endregion

        #region Constructors
        public ReadOnlyDerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes, Dictionary<string, string> rawAttributesByName)
            : this(baseAttributes, new AttributeCollection(rawAttributesByName)) { }

        public ReadOnlyDerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes, IEnumerable<(string key, string value)> rawAttributesByName) 
            : this(baseAttributes, new AttributeCollection(rawAttributesByName)) { }
        #endregion

        #region Copy Functions
        public AttributeCollection CreateCopy()
        {
            AttributeCollection attributes = [];
            foreach (var attributeName in this)
                attributes.Add(attributeName, this[attributeName]!);
            return attributes;
        }

        public ReadOnlyDerivedAttributeCollection CreateDerivedCopy()
            => new(BaseAttributes, DerivedAttributes);
        #endregion

        #region Get Functions
        public IEnumerator<string> GetEnumerator() => BaseAttributes.Union(DerivedAttributes).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool TryGetAttribute(string attributeName, out string? value)
            => DerivedAttributes.TryGetAttribute(attributeName, out value) || BaseAttributes.TryGetAttribute(attributeName, out value);

        public bool TryGetAttribute<T>(string attributeName, out T? value, AttributeCollection.TryParse<T> tryParser)
            => DerivedAttributes.TryGetAttribute(attributeName, out value, tryParser) || BaseAttributes.TryGetAttribute(attributeName, out value, tryParser);

        public bool TryGetAttribute<T>(string attributeName, out T? value, Func<string, T> parser)
            => DerivedAttributes.TryGetAttribute(attributeName, out value, parser) || BaseAttributes.TryGetAttribute(attributeName, out value, parser);

        public T GetAttributeOrDefault<T>(string attributeName, T defaultTo, AttributeCollection.TryParse<T> tryParser)
            => DerivedAttributes.TryGetAttribute(attributeName, out T? value, tryParser) 
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, tryParser) 
                    ? value! 
                    : defaultTo;

        public string? GetAttributeOrDefault(string attributeName, string? defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out string? value)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value)
                    ? value!
                    : defaultTo;

        public T GetAttribute<T>(string attributeName, Func<string, T> parser)
            => DerivedAttributes.TryGetAttribute(attributeName, out T? value, parser)
                ? value!
                : BaseAttributes.GetAttribute(attributeName, parser);

        public string GetAttribute(string attributeName)
            => DerivedAttributes.TryGetAttribute(attributeName, out string? value)
                ? value!
                : BaseAttributes.GetAttribute(attributeName);

        public bool HasAttribute(string attributeName)
            => DerivedAttributes.HasAttribute(attributeName) || BaseAttributes.HasAttribute(attributeName);

        public int GetAttributeOrDefault(string attributeName, int defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out int value, int.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, int.TryParse)
                    ? value!
                    : defaultTo;

        public float GetAttributeOrDefault(string attributeName, float defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out float value, float.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, float.TryParse)
                    ? value!
                    : defaultTo;

        public T GetEnumAttributeOrDefault<T>(string attributeName, T defaultTo) where T : struct
            => DerivedAttributes.TryGetAttribute(attributeName, out T value, Enum.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, Enum.TryParse)
                    ? value!
                    : defaultTo;

        public bool GetAttributeOrDefault(string attributeName, bool defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out bool value, bool.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, bool.TryParse)
                    ? value!
                    : defaultTo;

        public Space GetAttributeOrDefault(string attributeName, Space defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out Space value, Space.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, Space.TryParse)
                    ? value!
                    : defaultTo;

        public Vector2 GetAttributeOrDefault(string attributeName, Vector2 defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out Vector2 value, ToVector.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, ToVector.TryParse)
                    ? value!
                    : defaultTo;

        public Vector3 GetAttributeOrDefault(string attributeName, Vector3 defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out Vector3 value, ToVector.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, ToVector.TryParse)
                    ? value!
                    : defaultTo;

        public Color GetAttributeOrDefault(string attributeName, Color defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out Color value, Colour.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, Colour.TryParse)
                    ? value!
                    : defaultTo;

        public Sides GetAttributeOrDefault(string attributeName, Sides defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out Sides value, Sides.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, Sides.TryParse)
                    ? value!
                    : defaultTo;

        public T? GetAttributeOrDefault<T>(string attributeName, T? defaultTo, AttributeCollection.TryParse<T> tryParser) where T : struct
            => DerivedAttributes.TryGetAttribute(attributeName, out T value, tryParser)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, tryParser)
                    ? value!
                    : defaultTo;

        public int? GetAttributeOrDefault(string attributeName, int? defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out int value, int.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, int.TryParse)
                    ? value!
                    : defaultTo;

        public float? GetAttributeOrDefault(string attributeName, float? defaultTo)
            => DerivedAttributes.TryGetAttribute(attributeName, out float value, float.TryParse)
                ? value!
                : BaseAttributes.TryGetAttribute(attributeName, out value, float.TryParse)
                    ? value!
                    : defaultTo;
        #endregion
    }
}
