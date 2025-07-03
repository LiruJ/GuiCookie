namespace GuiCookie.Core.Data
{
    public class DerivedAttributeCollection : AttributeCollection
    {
        #region Indexers
        public override string? this[string name]
        {
            get => rawAttributesByName.TryGetValue(name, out string? value) ? value : null;
            set => rawAttributesByName.Add(name, value!);
        }
        #endregion

        #region Properties
        public IReadOnlyAttributeCollection BaseAttributes { get; }

        public override int Count => BaseAttributes.Union(this).Count();
        #endregion

        #region Constructors
        public DerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes) : base() 
            => BaseAttributes = baseAttributes;
        
        public DerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes, int capacity) : base(capacity) 
            => BaseAttributes = baseAttributes;

        public DerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes, Dictionary<string, string> rawAttributesByName) : base(rawAttributesByName) 
            => BaseAttributes = baseAttributes;

        public DerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes, IEnumerable<(string key, string value)> rawAttributesByName) : base(rawAttributesByName) 
            => BaseAttributes = baseAttributes;

        public DerivedAttributeCollection(IReadOnlyAttributeCollection baseAttributes, IReadOnlyAttributeCollection derivedAttributes) : base()
        {
            BaseAttributes = baseAttributes;
            foreach (string attributeName in derivedAttributes)
                Add(attributeName, derivedAttributes.GetAttribute(attributeName));
        }
        #endregion

        #region Get Functions
        public override IEnumerator<string> GetEnumerator() => BaseAttributes.Union(this).GetEnumerator();
        
        public override T GetAttributeOrDefault<T>(string attributeName, T defaultTo, TryParse<T> tryParser)
        {
            // Ensure validity.
            ArgumentNullException.ThrowIfNull(tryParser);
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            return rawAttributesByName.TryGetValue(attributeName, out string? attributeString) && tryParser(attributeString, out T output) 
                ? output 
                : BaseAttributes.GetAttributeOrDefault(attributeName, defaultTo, tryParser);
        }

        public override T? GetAttributeOrDefault<T>(string attributeName, T? defaultTo, TryParse<T> tryParser)
        {
            // Ensure validity.
            ArgumentNullException.ThrowIfNull(tryParser);
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            return rawAttributesByName.TryGetValue(attributeName, out string? attributeString) && tryParser(attributeString, out T output) 
                ? output 
                : BaseAttributes.GetAttributeOrDefault(attributeName, defaultTo, tryParser);
        }

        public override string? GetAttributeOrDefault(string attributeName, string? defaultTo)
            => rawAttributesByName.TryGetValue(attributeName, out string? attributeString) ? attributeString : BaseAttributes.GetAttributeOrDefault(attributeName, defaultTo);

        public override T GetAttribute<T>(string attributeName, Func<string, T> parser)
        {
            // Ensure validity.
            ArgumentNullException.ThrowIfNull(parser);
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            // Get the string from the raw dictionary, if it does not exist, try with the base.
            if (!rawAttributesByName.TryGetValue(attributeName, out string? attributeString))
                return BaseAttributes.GetAttribute<T>(attributeName, parser);

            // Parse and return the value.
            return parser(attributeString);
        }

        public override string GetAttribute(string attributeName)
            => rawAttributesByName.TryGetValue(attributeName, out string? value) ? value : BaseAttributes.GetAttribute(attributeName);

        public override bool HasAttribute(string attributeName) => base.HasAttribute(attributeName) || BaseAttributes.HasAttribute(attributeName);
        #endregion
    }
}
