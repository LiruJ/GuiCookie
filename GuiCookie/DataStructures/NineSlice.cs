using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

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
        public byte CalculatePieceMask(Piece piece)
        {
            switch (piece)
            {
                // If it's an edge or centre piece, return the mask constant.
                case Piece.Left:
                    return 0b10000;
                case Piece.Bottom:
                    return 0b01000;
                case Piece.Right:
                    return 0b00100;
                case Piece.Top:
                    return 0b00010;
                case Piece.Centre:
                    return 0b00001;

                // Corners can never be stretched, so return an invalid mask.
                case Piece.BottomLeft:
                case Piece.BottomRight:
                case Piece.TopRight:
                case Piece.TopLeft:
                default:
                    return 0;
            }
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

        public Rectangle CalculateSource(Rectangle mainSource, Piece piece)
        {
            switch (piece)
            {
                case Piece.BottomLeft:
                    return new Rectangle(new Point(mainSource.X, mainSource.Y + (int)Math.Ceiling(mainSource.Height * MaxY)),
                        new Point((int)Math.Ceiling(mainSource.Width * MinX), (int)Math.Ceiling(mainSource.Height * (1.0f - MaxY))));
                case Piece.Bottom:
                    return new Rectangle(new Point(mainSource.X + (int)Math.Ceiling(mainSource.Width * MinX), mainSource.Y + (int)Math.Ceiling(mainSource.Height * MaxY)),
                        new Point((int)Math.Ceiling(mainSource.Width * (MaxX - MinX)), (int)Math.Ceiling(mainSource.Height * (1.0f - MaxY))));
                case Piece.BottomRight:
                    return new Rectangle(new Point(mainSource.X + (int)Math.Ceiling(mainSource.Width * MaxX), mainSource.Y + (int)Math.Ceiling(mainSource.Height * MaxY)),
                        new Point((int)Math.Ceiling(mainSource.Width * (1.0f - MaxX)), (int)Math.Ceiling(mainSource.Height * (1.0f - MaxY))));
                case Piece.Right:
                    return new Rectangle(new Point(mainSource.X + (int)Math.Ceiling(mainSource.Width * MaxX), mainSource.Y + (int)Math.Ceiling(mainSource.Height * MinY)),
                        new Point((int)Math.Ceiling(mainSource.Width * (1.0f - MaxX)), (int)Math.Ceiling(mainSource.Height * (MaxY - MinY))));
                case Piece.TopRight:
                    return new Rectangle(new Point(mainSource.X + (int)Math.Ceiling(mainSource.Width * MaxX), mainSource.Y),
                        new Point((int)Math.Ceiling(mainSource.Width * (1.0f - MaxX)), (int)Math.Ceiling(mainSource.Height * MinY)));
                case Piece.Top:
                    return new Rectangle(new Point(mainSource.X + (int)Math.Ceiling(mainSource.Width * MinX), mainSource.Y),
                        new Point((int)Math.Ceiling(mainSource.Width * (MaxX - MinX)), (int)Math.Ceiling(mainSource.Height * MinY)));
                case Piece.TopLeft:
                    return new Rectangle(new Point(mainSource.X, mainSource.Y),
                        new Point((int)Math.Ceiling(mainSource.Width * MinX), (int)Math.Ceiling(mainSource.Height * MinY)));
                case Piece.Left:
                    return new Rectangle(new Point(mainSource.X, mainSource.Y + (int)Math.Ceiling(mainSource.Height * MinY)),
                        new Point((int)Math.Ceiling(mainSource.Width * MinX), (int)Math.Ceiling(mainSource.Height * (MaxY - MinY))));
                case Piece.Centre:
                    return new Rectangle(new Point(mainSource.X + (int)Math.Ceiling(mainSource.Width * MinX), mainSource.Y + (int)Math.Ceiling(mainSource.Height * MinY)),
                        new Point((int)Math.Ceiling(mainSource.Width * (MaxX - MinX)), (int)Math.Ceiling(mainSource.Height * (MaxY - MinY))));
                default:
                    throw new Exception($"Invalid piece. {piece}");
            }
        }
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
                    return values.Length > 1 ? tryParseStretchMask(values[1], out nineSlice.stretchMask, throwException) : true;
                case vSliceKeyword:
                    nineSlice = VSlice;
                    return values.Length > 1 ? tryParseStretchMask(values[1], out nineSlice.stretchMask, throwException) : true;
                case hSliceKeyword:
                    nineSlice = HSlice;
                    return values.Length > 1 ? tryParseStretchMask(values[1], out nineSlice.stretchMask, throwException) : true;

                // Otherwise; parse the string.
                default:
                    // If there are not exactly 4 or 5 values, throw an exception.
                    if (values.Length != 4 || values.Length != 5)
                        return throwException ? throw new ArgumentException($"NineSlice must have 4 or 5 values; minX, maxX, minY, and maxY, with an optional stretchMask, separated with the {separator} character. Given string: {input} had {values.Length} values.") : false;

                    // Parse each value.
                    if (!float.TryParse(values[0], out float minX) || (minX < 0 || minX > 1)) return throwException ? throw new FormatException($"MinX of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[1], out float maxX) || (maxX < 0 || maxX > 1)) return throwException ? throw new FormatException($"MaxX of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[2], out float minY) || (minY < 0 || minY > 1)) return throwException ? throw new FormatException($"MinY of nineslice was invalid, float value between 0 and 1 expected.") : false;
                    if (!float.TryParse(values[3], out float maxY) || (maxY < 0 || maxY > 1)) return throwException ? throw new FormatException($"MaxY of nineslice was invalid, float value between 0 and 1 expected.") : false;

                    // Parse the stretchmask.
                    tryParseStretchMask(values[4], out byte stretchMask, throwException);

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
