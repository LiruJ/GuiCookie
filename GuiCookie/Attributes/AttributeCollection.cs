using GuiCookie.DataStructures;
using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Xml;

namespace GuiCookie.Attributes
{
    public class AttributeCollection : IReadOnlyAttributes
    {
        #region Delegates
        public delegate bool TryParse<T>(string input, out T output);
        #endregion

        #region Fields
        /// <summary> The raw string values keyed by attribute. </summary>
        private readonly Dictionary<string, string> rawAttributesByName = new Dictionary<string, string>();
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

        internal AttributeCollection(XmlNode elementNode)
        {
            foreach (XmlAttribute attribute in elementNode.Attributes) Add(attribute.Name, attribute.InnerText);
        }

        private AttributeCollection(Dictionary<string, string> rawAttributesByName)
        {
            this.rawAttributesByName = rawAttributesByName ?? throw new ArgumentNullException(nameof(rawAttributesByName));
        }
        #endregion

        #region Copy Functions
        public AttributeCollection CreateCopy() => new AttributeCollection(new Dictionary<string, string>(rawAttributesByName));
        #endregion

        #region Get Functions
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
            if (tryParser == null) throw new ArgumentNullException(nameof(tryParser));
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            return HasAttribute(attributeName) && tryParser(rawAttributesByName[attributeName], out T output) ? output : defaultTo;
        }

        public string GetAttributeOrDefault(string attributeName, string defaultTo)
        {
            // Ensure validity.
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            return rawAttributesByName.TryGetValue(attributeName, out string value) ? value : defaultTo;
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
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            // Get the string from the raw dictionary, if it does not exist, throw an exception.
            if (!rawAttributesByName.TryGetValue(attributeName, out string attributeString)) throw new KeyNotFoundException($"No attribute with the key {attributeName} exists.");

            // Parse and return the value.
            return parser(attributeString);
        }

        public string GetAttribute(string attributeName)
        {
            // Ensure validity.
            if (string.IsNullOrWhiteSpace(attributeName))
                throw new ArgumentException($"'{nameof(attributeName)}' cannot be null or whitespace", nameof(attributeName));

            // Get the string from the raw dictionary, if it does not exist, throw an exception.
            if (!rawAttributesByName.TryGetValue(attributeName, out string attributeString)) throw new KeyNotFoundException($"No attribute with the key {attributeName} exists.");

            // Return the attribute string.
            return attributeString;
        }

        public bool HasAttribute(string attributeName) => rawAttributesByName.ContainsKey(attributeName);
        #endregion

        #region Shortcut Functions
        public T GetEnumAttributeOrDefault<T>(string attributeName, T defaultTo) where T : struct => GetAttributeOrDefault(attributeName, defaultTo, Enum.TryParse);

        public int GetAttributeOrDefault(string attributeName, int defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, int.TryParse);

        public float GetAttributeOrDefault(string attributeName, float defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, float.TryParse);

        public bool GetAttributeOrDefault(string attributeName, bool defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, bool.TryParse);

        public Space GetAttributeOrDefault(string attributeName, Space defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, Space.TryParse);

        public Sides GetAttributeOrDefault(string attributeName, Sides defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, Sides.TryParse);

        public Vector2 GetAttributeOrDefault(string attributeName, Vector2 defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, ToVector2.TryParse);

        public Vector3 GetAttributeOrDefault(string attributeName, Vector3 defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, ToVector3.TryParse);

        public Color GetAttributeOrDefault(string attributeName, Color defaultTo) => GetAttributeOrDefault(attributeName, defaultTo, Colour.TryParse);
        #endregion

        #region Addition Functions
        public void Add(string key, string value) { if (!rawAttributesByName.ContainsKey(key)) rawAttributesByName.Add(key, value); else throw new Exception($"Attribute with key {key} already exists."); }

        public void Add(string key, object value) => Add(key, value.ToString());

        public void Replace(string key, string value) { if (!rawAttributesByName.ContainsKey(key)) rawAttributesByName.Remove(key); rawAttributesByName.Add(key, value);  }
        #endregion

        #region Removal Functions
        public bool Remove(string key) => rawAttributesByName.Remove(key);
        #endregion

        #region String Functions
        public override string ToString() => $"Attribute collection with {Count} attributes.";
        #endregion
    }
}
