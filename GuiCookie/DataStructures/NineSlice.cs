﻿using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace GuiCookie.DataStructures
{
    public struct NineSlice : IEquatable<NineSlice>
    {
        #region Constants
        private const char separator = ',';

        private const string thirdsKeyword = "Thirds";

        private const string hSliceKeyword = "HSlice";

        private const string vSliceKeyword = "VSlice";
        #endregion

        #region Fields
        private byte stretchMask;
        #endregion

        #region Properties
        public float MaxY { get; }

        public float MinY { get; }

        public float MaxX { get; }

        public float MinX { get; }
        #endregion

        #region Presets
        public static NineSlice Empty => new NineSlice(0, 0, 0, 0);

        public static NineSlice Thirds => new NineSlice(1.0f / 3, 2.0f / 3, 1.0f / 3, 2.0f / 3);

        public static NineSlice HSlice => new NineSlice(1.0f / 3, 2.0f / 3, 0, 1, 0b10100);

        public static NineSlice VSlice => new NineSlice(0, 1, 1.0f / 3, 2.0f / 3, 0b01010);
        #endregion

        #region Constructors
        public NineSlice(float minX, float maxX, float minY, float maxY, byte mask)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;

            // Set the mask.
            stretchMask = mask;
        }

        public NineSlice(float minX, float maxX, float minY, float maxY) : this(minX, maxX, minY, maxY, 0) { }
        #endregion

        #region Calculation Functions
        public static byte CalculatePieceMask(Piece piece)
        {
            return piece switch
            {
                // If it's an edge or centre piece, return the mask constant.
                Piece.Left => 0b10000,
                Piece.Bottom => 0b01000,
                Piece.Right => 0b00100,
                Piece.Top => 0b00010,
                Piece.Centre => 0b00001,
                // Corners can never be stretched, so return an invalid mask.
                _ => 0,
            };
        }

        public void SetPieceStretchMask(Piece piece, bool stretched)
        {
            // Get the mask for the piece.
            byte pieceMask = CalculatePieceMask(piece);

            // If the mask is 0, do nothing.
            if (pieceMask == 0) return;

            // If the given boolean is false, then invert the piece mask.
            if (stretched) stretchMask |= pieceMask;
            else stretchMask ^= pieceMask;
        }

        public bool IsPieceStretched(Piece piece)
        {
            // Get the mask for the piece.
            byte pieceMask = CalculatePieceMask(piece);

            // If the mask is 0, return false.
            if (pieceMask == 0 || stretchMask == 0) return false;

            // Return the bit of the mask.
            return (stretchMask & pieceMask) == pieceMask;
        }

        /// <summary> Calculates the sliced rectangle for a given <paramref name="piece"/> from the given <paramref name="mainSource"/>. </summary>
        /// <param name="mainSource"> The main rectangle to slice. </param>
        /// <param name="piece"> The piece of the rectangle to calculate. </param>
        /// <returns> The sliced bounds. </returns>
        public Rectangle CalculateSource(Rectangle mainSource, Piece piece)
        {
            return piece switch
            {
                Piece.BottomLeft => new Rectangle(new Point(mainSource.X, mainSource.Y + calculateMaxEdge(mainSource.Height, MaxY)),
                        new Point(calculateMinEdge(mainSource.Width, MinX), mainSource.Height - calculateMaxEdge(mainSource.Height, MaxY))),
                Piece.Bottom => new Rectangle(new Point(mainSource.X + calculateMinEdge(mainSource.Width, MinX), mainSource.Y + calculateMaxEdge(mainSource.Height, MaxY)),
                        new Point(calculateCentreSize(mainSource.Width, MinX, MaxX), mainSource.Height - calculateMaxEdge(mainSource.Height, MaxY))),
                Piece.BottomRight => new Rectangle(new Point(mainSource.X + calculateMaxEdge(mainSource.Width, MaxX), mainSource.Y + calculateMaxEdge(mainSource.Height, MaxY)),
                        new Point(mainSource.Width - calculateMaxEdge(mainSource.Width, MaxX), mainSource.Height - calculateMaxEdge(mainSource.Height, MaxY))),
                Piece.Right => new Rectangle(new Point(mainSource.X + calculateMaxEdge(mainSource.Width, MaxX), mainSource.Y + calculateMinEdge(mainSource.Height, MinY)),
                        new Point(mainSource.Width - calculateMaxEdge(mainSource.Width, MaxX), calculateCentreSize(mainSource.Height, MinY, MaxY))),
                Piece.TopRight => new Rectangle(new Point(mainSource.X + calculateMaxEdge(mainSource.Width, MaxX), mainSource.Y),
                        new Point(mainSource.Width - calculateMaxEdge(mainSource.Width, MaxX), calculateMinEdge(mainSource.Height, MinY))),
                Piece.Top => new Rectangle(new Point(mainSource.X + calculateMinEdge(mainSource.Width, MinX), mainSource.Y),
                        new Point(calculateCentreSize(mainSource.Width, MinX, MaxX), calculateMinEdge(mainSource.Height, MinY))),
                Piece.TopLeft => new Rectangle(new Point(mainSource.X, mainSource.Y),
                        new Point(calculateMinEdge(mainSource.Width, MinX), calculateMinEdge(mainSource.Height, MinY))),
                Piece.Left => new Rectangle(new Point(mainSource.X, mainSource.Y + calculateMinEdge(mainSource.Height, MinY)),
                        new Point(calculateMinEdge(mainSource.Width, MinX), calculateCentreSize(mainSource.Height, MinY, MaxY))),
                Piece.Centre => new Rectangle(new Point(mainSource.X + calculateMinEdge(mainSource.Width, MinX), mainSource.Y + calculateMinEdge(mainSource.Height, MinY)),
                        new Point(calculateCentreSize(mainSource.Width, MinX, MaxX), calculateCentreSize(mainSource.Height, MinY, MaxY))),
                _ => throw new Exception($"Invalid piece. {piece}"),
            };
        }

        private int calculateMinEdge(int axisSize, float axisSlice) => axisSize % 2 == 0 ? (int)MathF.Floor(axisSize * axisSlice) : (int)MathF.Ceiling(axisSize * axisSlice);

        private int calculateMaxEdge(int axisSize, float axisSlice) => (int)MathF.Ceiling(axisSize * axisSlice);

        private int calculateCentreSize(int axisSize, float axisSliceMin, float axisSliceMax) => calculateMaxEdge(axisSize, axisSliceMax) - calculateMinEdge(axisSize, axisSliceMin);
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
            if (string.IsNullOrWhiteSpace(input)) return throwException ? throw new ArgumentNullException(nameof(input), "Given string cannot be null, empty, or whitespace.") : false;

            // Split the input into separate values.
            string[] values = input.Split(separator);

            switch (values[0])
            {
                // Handle keywords.
                case thirdsKeyword:
                    nineSlice = Thirds;
                    return values.Length <= 1 || tryParseStretchMask(values[1], out nineSlice.stretchMask, throwException);
                case vSliceKeyword:
                    nineSlice = VSlice;
                    return values.Length <= 1 || tryParseStretchMask(values[1], out nineSlice.stretchMask, throwException);
                case hSliceKeyword:
                    nineSlice = HSlice;
                    return values.Length <= 1 || tryParseStretchMask(values[1], out nineSlice.stretchMask, throwException);

                // Otherwise; parse the string.
                default:
                    // If there are not exactly 4 or 5 values, throw an exception.
                    if (values.Length != 4 && values.Length != 5)
                        return throwException ? throw new ArgumentException($"NineSlice must have 4 or 5 values; minX, maxX, minY, and maxY, with an optional StretchMask, separated with the {separator} character. Given string: {input} had {values.Length} values.") : false;

                    // Parse each value.
                    if (!float.TryParse(values[0], NumberStyles.Float, ParserSettings.FormatProvider, out float minX) || (minX < 0 || minX > 1)) return throwException ? throw new FormatException($"MinX of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[1], NumberStyles.Float, ParserSettings.FormatProvider, out float maxX) || (maxX < 0 || maxX > 1)) return throwException ? throw new FormatException($"MaxX of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[2], NumberStyles.Float, ParserSettings.FormatProvider, out float minY) || (minY < 0 || minY > 1)) return throwException ? throw new FormatException($"MinY of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[3], NumberStyles.Float, ParserSettings.FormatProvider, out float maxY) || (maxY < 0 || maxY > 1)) return throwException ? throw new FormatException($"MaxY of nineslice was invalid, float value between 0 and 1 expected.") : false;

                    // Parse the stretchmask.
                    byte stretchMask = 0;
                    if (values.Length == 5)
                        tryParseStretchMask(values[4], out stretchMask, throwException);

                    // Return the parsed nineslice.
                    nineSlice = new NineSlice(minX, maxX, minY, maxY, stretchMask);
                    return true;
            }
        }

        private static bool tryParseStretchMask(string input, out byte stretchMask, bool throwException = false)
        {
            // Try to convert the stretch mask from binary.
            try
            {
                stretchMask = Convert.ToByte(input.Trim(), 2);
                return true;
            }
            catch (Exception)
            {
                stretchMask = 0;
                return throwException ? throw new FormatException($"StretchMask of nineslice was invalid, byte mask value expected.") : false;
            }
        }
        #endregion

        #region String Functions
        public override string ToString() => $"X: {MinX}-{MaxX} Y: {MinY}{MaxY} Stretch: {stretchMask}";
        #endregion

        #region Equality Functions
        public override bool Equals(object obj) => obj is NineSlice slice && Equals(slice);

        public bool Equals(NineSlice other)
            => stretchMask == other.stretchMask &&
                   MaxY == other.MaxY &&
                   MinY == other.MinY &&
                   MaxX == other.MaxX &&
                   MinX == other.MinX;

        public override int GetHashCode()
        {
            int hashCode = -1714440444;
            hashCode = hashCode * -1521134295 + stretchMask.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxY.GetHashCode();
            hashCode = hashCode * -1521134295 + MinY.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxX.GetHashCode();
            hashCode = hashCode * -1521134295 + MinX.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(NineSlice left, NineSlice right) => left.Equals(right);

        public static bool operator !=(NineSlice left, NineSlice right) => !(left == right);
        #endregion
    }
}
