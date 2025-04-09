using GuiCookie.Core.DataStructures;
using LiruGameHelper.Parsers;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Numerics;

namespace GuiCookie.Core.Data
{
    public class AttributeCollection : IReadOnlyAttributeCollection, IEnumerable<string>
    {
        #region Delegates
        /// <summary> A delegate describing a function that attempts to parse the given <paramref name="input"/> into the <paramref name="output"/> with the given <typeparamref name="T"/> type. </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public delegate bool TryParse<T>(string input, out T output);
        #endregion

        #region Indexers
        public string this[string name]
        {
            get => rawAttributesByName.TryGetValue(name, out string value) ? value : null;
            set => rawAttributesByName.Add(name, value);
        }
        #endregion

        #region Fields
        /// <summary> The raw string values keyed by attribute. </summary>
        private readonly Dictionary<string, string> rawAttributesByName = [];
        #endregion

        #region Properties
        public ICollection<string> Keys => rawAttributesByName.Keys;

        public int Count => rawAttributesByName.Count;
        #endregion

        #region Constructors
        public AttributeCollection() { }

        public AttributeCollection(int capacity)
        {
            // Initialise the raw attributes.
            rawAttributesByName = new Dictionary<string, string>(capacity);
        }

        public AttributeCollection(Dictionary<string, string> rawAttributesByName)
        {
            this.rawAttributesByName = new Dictionary<string, string>(rawAttributesByName ?? throw new ArgumentNullException(nameof(rawAttributesByName)));
        }

        public AttributeCollection(IEnumerable<(string key, string value)> rawAttributesByName)
        {
            this.rawAttributesByName = rawAttributesByName.ToDictionary(x => x.key, x => x.value);
        }
        #endregion

        #region Copy Functions
        public AttributeCollection CreateCopy() => new(new Dictionary<string, string>(rawAttributesByName));
        #endregion

        #region Get Functions
        public IEnumerator<string> GetEnumerator() => Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Keys).GetEnumerator();

        /// <summary> Attempts to parse the attribute with the given <paramref name="attributeName"/> using the given <paramref name="tryParser"/> function, returns <paramref name="defaultTo"/> if the key was not found or the function returned false. </summary>
        /// <typeparam name="T"> The type of object to parse. </typeparam>
        /// <param name="attributeName"> The name of the attribute to parse. </param>
        /// <param name="defaultTo"> The value to return if the parser fails or the attribute was not found. </param>
        /// <param name="tryParser"> The function to run in order to try and parse the attribute. e.g. <see cref="int.TryParse(string, out int)"/>. This function should not throw exceptions. </param>
        /// <returns> The parsed attribute, or <paramref name="defaultTo"/> if the attribute was not found or could not parse. </returns>
        /// <exception cref="ArgumentNullException"> The <paramref name="tryParser"/> was null. </exception>
        /// <exception cref="ArgumentException"> The <paramref name="attributeName"/> was null or whitespace. </exception>
        public T GetAttributeOrDefault<T>(string attributeName, T defaultTo, TryParse<T> tryParser)
        {
            // Ensure validity.
            ArgumentNullException.ThrowIfNull(tryParser);
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            return HasAttribute(attributeName) && tryParser(rawAttributesByName[attributeName], out T output) ? output : defaultTo;
        }

        public T? GetAttributeOrDefault<T>(string attributeName, T? defaultTo, TryParse<T> tryParser) where T : struct
        {
            // Ensure validity.
            ArgumentNullException.ThrowIfNull(tryParser);
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            return HasAttribute(attributeName) && tryParser(rawAttributesByName[attributeName], out T output) ? output : defaultTo;
        }

        public string? GetAttributeOrDefault(string attributeName, string? defaultTo)
        {
            // Ensure validity.
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            return rawAttributesByName.TryGetValue(attributeName, out string? value) ? value : defaultTo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributeName"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> The <paramref name="parser"/> was null. </exception>
        /// <exception cref="ArgumentException"> The <paramref name="attributeName"/> was null or whitespace. </exception>
        public T GetAttribute<T>(string attributeName, Func<string, T> parser)
        {
            // Ensure validity.
            ArgumentNullException.ThrowIfNull(parser);
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            // Get the string from the raw dictionary, if it does not exist, throw an exception.
            if (!rawAttributesByName.TryGetValue(attributeName, out string? attributeString))
                throw new KeyNotFoundException($"No attribute with the key {attributeName} exists.");

            // Parse and return the value.
            return parser(attributeString);
        }

        public string GetAttribute(string attributeName)
        {
            // Ensure validity.
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            // Get the string from the raw dictionary, if it does not exist, throw an exception.
            if (!rawAttributesByName.TryGetValue(attributeName, out string? attributeString)) 
                throw new KeyNotFoundException($"No attribute with the key {attributeName} exists.");

            // Return the attribute string.
            return attributeString;
        }

        public bool HasAttribute(string attributeName) => rawAttributesByName.ContainsKey(attributeName);
        #endregion

        #region Shortcut Functions
        public T GetEnumAttributeOrDefault<T>(string attributeName, T defaultTo) where T : struct => GetAttributeOrDefault(attributeName, defaultTo, Enum.TryParse);

        public int GetAttributeOrDefault(string attributeName, int defaultTo) 
            => GetAttributeOrDefault(attributeName, defaultTo, (string input, out int output) => int.TryParse(input, NumberStyles.Integer, ParserSettings.FormatProvider, out output));

        public int? GetAttributeOrDefault(string attributeName, int? defaultTo) 
            => GetAttributeOrDefault(attributeName, defaultTo, (string input, out int output) => int.TryParse(input, NumberStyles.Integer, ParserSettings.FormatProvider, out output));
        
        public float GetAttributeOrDefault(string attributeName, float defaultTo) 
            => GetAttributeOrDefault(attributeName, defaultTo, (string input, out float output) => float.TryParse(input, NumberStyles.Float, ParserSettings.FormatProvider, out output));

        public float? GetAttributeOrDefault(string attributeName, float? defaultTo)
            => GetAttributeOrDefault(attributeName, defaultTo, (string input, out float output) => float.TryParse(input, NumberStyles.Float, ParserSettings.FormatProvider, out output));

        public bool GetAttributeOrDefault(string attributeName, bool defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, bool.TryParse);

        public Space GetAttributeOrDefault(string attributeName, Space defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, Space.TryParse);

        public Sides GetAttributeOrDefault(string attributeName, Sides defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, Sides.TryParse);

        public Vector2 GetAttributeOrDefault(string attributeName, Vector2 defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, ToVector.TryParse);

        public Vector3 GetAttributeOrDefault(string attributeName, Vector3 defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, ToVector.TryParse);

        public Color GetAttributeOrDefault(string attributeName, Color defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, Colour.TryParse);
        #endregion

        #region Addition Functions
        public void Add(string key, string value) 
        {
            if (!rawAttributesByName.TryAdd(key, value)) 
                throw new Exception($"Attribute with key {key} already exists."); 
        }

        public void Add(string key, object value) => Add(key, value.ToString()!);

        public void Replace(string key, string value) 
        {
            if (!rawAttributesByName.ContainsKey(key)) 
                rawAttributesByName.Remove(key); 
            rawAttributesByName.Add(key, value);  
        }
        #endregion

        #region Removal Functions
        public bool Remove(string key) => rawAttributesByName.Remove(key);
        #endregion

        #region String Functions
        public override string ToString() => $"Attribute collection with {Count} attributes.";
        #endregion
    }
}