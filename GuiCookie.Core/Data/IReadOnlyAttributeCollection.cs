using GuiCookie.Core.DataStructures;
using System.Drawing;
using System.Numerics;

namespace GuiCookie.Core.Data
{
    public interface IReadOnlyAttributeCollection : IEnumerable<string>
    {
        #region Properties
        int Count { get; }
        #endregion

        #region Copy Functions
        AttributeCollection CreateCopy();
        #endregion

        #region Get Functions
        bool TryGetAttribute(string attributeName, out string? value);
        bool TryGetAttribute<T>(string attributeName, out T? value, AttributeCollection.TryParse<T> tryParser);
        bool TryGetAttribute<T>(string attributeName, out T? value, Func<string, T> parser);

        T GetAttributeOrDefault<T>(string attributeName, T defaultTo, AttributeCollection.TryParse<T> tryParser);
        string? GetAttributeOrDefault(string attributeName, string? defaultTo);
        T GetAttribute<T>(string attributeName, Func<string, T> parser);
        string GetAttribute(string attributeName);
        bool HasAttribute(string attributeName);

        int GetAttributeOrDefault(string attributeName, int defaultTo);
        float GetAttributeOrDefault(string attributeName, float defaultTo);
        T GetEnumAttributeOrDefault<T>(string attributeName, T defaultTo) where T : struct;
        bool GetAttributeOrDefault(string attributeName, bool defaultTo);
        Space GetAttributeOrDefault(string attributeName, Space defaultTo);
        Vector2 GetAttributeOrDefault(string attributeName, Vector2 defaultTo);
        Vector3 GetAttributeOrDefault(string attributeName, Vector3 defaultTo);
        Color GetAttributeOrDefault(string attributeName, Color defaultTo);
        Sides GetAttributeOrDefault(string attributeName, Sides defaultTo);
        T? GetAttributeOrDefault<T>(string attributeName, T? defaultTo, AttributeCollection.TryParse<T> tryParser) where T : struct;
        int? GetAttributeOrDefault(string attributeName, int? defaultTo);
        float? GetAttributeOrDefault(string attributeName, float? defaultTo);
        #endregion
    }
}