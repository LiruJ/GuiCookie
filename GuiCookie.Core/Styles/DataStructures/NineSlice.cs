using LiruGameHelper.Parsers;
using System.Globalization;
using System.Numerics;

namespace GuiCookie.Core.Styles.DataStructures
{
    public struct NineSlice(float minX, float maxX, float minY, float maxY) : IEquatable<NineSlice>
    {
        #region Constants
        private const char separator = ',';

        private const string thirdsKeyword = "Thirds";

        private const string hSliceKeyword = "HSlice";

        private const string vSliceKeyword = "VSlice";
        #endregion

        #region Properties
        public readonly Vector4 Values { get; } = new(minX, maxX, minY, maxY);

        public readonly float MaxY => Values.W;

        public readonly float MinY => Values.Z;

        public readonly float MaxX => Values.Y;

        public readonly float MinX => Values.X;
        #endregion

        #region Presets
        public static NineSlice Empty => new(0, 0, 0, 0);

        public static NineSlice Thirds => new(1.0f / 3, 2.0f / 3, 1.0f / 3, 2.0f / 3);

        public static NineSlice HSlice => new(1.0f / 3, 2.0f / 3, 0, 1);

        public static NineSlice VSlice => new(0, 1, 1.0f / 3, 2.0f / 3);
        #endregion

        #region Parse Functions
        public static NineSlice Parse(string input)
        {
            // Try to parse with exceptions being thrown.
            tryParse(input, out NineSlice nineSlice, true);

            // Return the parsed nineslice.
            return nineSlice;
        }

        public static bool TryParse(string input, out NineSlice nineSlice) => tryParse(input, out nineSlice);

        private static bool tryParse(string input, out NineSlice nineSlice, bool throwException = false)
        {
            // Start with an empty value.
            nineSlice = Empty;

            // If the input is invalid, handle it.
            if (string.IsNullOrWhiteSpace(input))
                return throwException ? throw new ArgumentNullException(nameof(input), "Given string cannot be null, empty, or whitespace.") : false;

            // Split the input into separate values.
            string[] values = input.Split(separator);

            switch (values[0])
            {
                // Handle keywords.
                case thirdsKeyword:
                    nineSlice = Thirds;
                    return true;
                case vSliceKeyword:
                    nineSlice = VSlice;
                    return true;
                case hSliceKeyword:
                    nineSlice = HSlice;
                    return true;

                // Otherwise; parse the string.
                default:
                    // If there are not exactly 4 or 5 values, throw an exception.
                    if (values.Length != 4 && values.Length != 5)
                        return throwException ? throw new ArgumentException($"NineSlice must have 4 or 5 values; minX, maxX, minY, and maxY, with an optional StretchMask, separated with the {separator} character. Given string: {input} had {values.Length} values.") : false;

                    // Parse each value.
                    if (!float.TryParse(values[0], NumberStyles.Float, ParserSettings.FormatProvider, out float minX) || minX < 0 || minX > 1) return throwException ? throw new FormatException($"MinX of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[1], NumberStyles.Float, ParserSettings.FormatProvider, out float maxX) || maxX < 0 || maxX > 1) return throwException ? throw new FormatException($"MaxX of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[2], NumberStyles.Float, ParserSettings.FormatProvider, out float minY) || minY < 0 || minY > 1) return throwException ? throw new FormatException($"MinY of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[3], NumberStyles.Float, ParserSettings.FormatProvider, out float maxY) || maxY < 0 || maxY > 1) return throwException ? throw new FormatException($"MaxY of nineslice was invalid, float value between 0 and 1 expected.") : false;

                    // Return the parsed nineslice.
                    nineSlice = new NineSlice(minX, maxX, minY, maxY);
                    return true;
            }
        }
        #endregion

        #region String Functions
        public override readonly string ToString() => $"X: {MinX}-{MaxX} Y: {MinY}-{MaxY}";
        #endregion

        #region Equality Functions
        public override readonly bool Equals(object? obj) => obj is NineSlice slice && Equals(slice);

        public readonly bool Equals(NineSlice other)
            => MaxY == other.MaxY &&
                MinY == other.MinY &&
                MaxX == other.MaxX &&
                MinX == other.MinX;

        public override readonly int GetHashCode()
            => HashCode.Combine(MaxY, MinY, MaxX, MinX);

        public static bool operator ==(NineSlice left, NineSlice right) => left.Equals(right);

        public static bool operator !=(NineSlice left, NineSlice right) => !(left == right);
        #endregion
    }
}