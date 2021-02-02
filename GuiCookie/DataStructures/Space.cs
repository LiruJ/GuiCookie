using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.DataStructures
{
    /// <summary> Represents a two-dimensional position or size where either axis can be relative or absolute. </summary>
    public struct Space
    {
        #region Properties
        public float X { get; }

        public float Y { get; }

        public bool IsXRelative => (Axes & Axes.X) == Axes.X;

        public bool IsYRelative => (Axes & Axes.Y) == Axes.Y;

        public Axes Axes { get; }
        #endregion

        #region Preset Properties
        public static Space Empty { get => new Space(0, Axes.None); }
        #endregion

        #region Equality Function
        public override bool Equals(object obj) => obj is Space space &&
                   Axes == space.Axes &&
                   X == space.X &&
                   Y == space.Y;
        

        public static bool operator ==(Space left, Space right) =>left.Equals(right);

        public static bool operator !=(Space left, Space right) =>!(left == right);
        #endregion

        #region Constructors
        public Space(Point point)
        {
            X = point.X;
            Y = point.Y;
            Axes = Axes.None;
        }

        public Space(float both, Axes axes)
        {
            X = both;
            Y = both;
            this.Axes = axes;
        }

        public Space(float x, float y, Axes axes)
        {
            X = x;
            Y = y;
            this.Axes = axes;
        }
        #endregion

        #region Get Functions
        public float GetScaledX(float parentAxis) => IsXRelative ? parentAxis * X : X;

        public float GetScaledY(float parentAxis) => IsYRelative ? parentAxis * Y : Y;

        public Vector2 GetScaledSpace(Vector2 parentSpace) => new Vector2(GetScaledX(parentSpace.X), GetScaledY(parentSpace.Y));

        public Point GetScaledSpace(Point parentSpace) => new Point((int)Math.Floor(GetScaledX(parentSpace.X)), (int)Math.Floor(GetScaledY(parentSpace.Y)));
        #endregion

        #region Parsers
        public static Space Parse(string input)
        {
            // Try to parse the string, if it fails, throw an error; otherwise, return the parsed space.
            if (!TryParse(input, out Space space)) throw new ArgumentException("Given string was not a valid space.");
            else return space;
        }

        public static bool TryParse(string input, out Space space)
        {
            // If the given string is empty, return with an empty space.
            if (input == string.Empty) { space = new Space(0, Axes.None); return false; }

            // Split the input into axes.
            string[] spaceAxes = input.Split(',');

            // Parse the first value.
            if (!Relative.TryParse(spaceAxes[0], out float x, out bool xRelative)) { space = Empty; return false; }

            // If only one value was given, use this for both sides.
            if (spaceAxes.Length == 1) { space = new Space(x, xRelative ? Axes.Both : Axes.None); return true; }

            // Parse the second value.
            if (!Relative.TryParse(spaceAxes[1], out float y, out bool yRelative)) { space = Empty; return false; }

            // Keep track of what axes should be relative.
            Axes relativeAxes = Axes.None;
            if (xRelative) relativeAxes |= Axes.X;
            if (yRelative) relativeAxes |= Axes.Y;

            // Create a space with the parsed values.
            space = new Space(x, y, relativeAxes);
            return true;
        }
        #endregion

        #region Override Functions
        public override string ToString()
        {
            // Create a string builder.
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            // Add the x and y.
            stringBuilder.Append(((IsXRelative) ? $"{X:P0}" : X.ToString()) + ",");
            stringBuilder.Append((IsYRelative) ? $"{Y:P0}" : Y.ToString());

            return stringBuilder.ToString();
        }
        public override int GetHashCode()
        {
            var hashCode = 1616992779;
            hashCode = hashCode * -1521134295 + Axes.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
        #endregion
    }
}
