using LiruGameHelperMonoGame.Parsers;
using Microsoft.Xna.Framework;
using System;

namespace GuiCookie.DataStructures
{
    public struct Sides
    {
        #region Fields
        private readonly SideMask relativeSides;
        #endregion

        #region Relative Properties
        public bool IsTopRelative => (relativeSides & SideMask.Top) == SideMask.Top;

        public bool IsRightRelative => (relativeSides & SideMask.Right) == SideMask.Right;

        public bool IsBottomRelative => (relativeSides & SideMask.Bottom) == SideMask.Bottom;

        public bool IsLeftRelative => (relativeSides & SideMask.Left) == SideMask.Left;
        #endregion

        #region Side Properties
        public float Top { get; }

        public float Right { get; }

        public float Bottom { get; }

        public float Left { get; }
        #endregion

        #region Constructors
        public Sides(float allSides, SideMask relativeSides) : this(allSides, allSides, relativeSides) { }

        public Sides(float leftRight, float topBottom, SideMask relativeSides) : this(topBottom, leftRight, topBottom, leftRight, relativeSides) { }

        public Sides(float top, float leftRight, float bottom, SideMask relativeSides) : this(top, leftRight, bottom, leftRight, relativeSides) { }

        public Sides(float top, float right, float bottom, float left, SideMask relativeSides)
        {
            // Set relative sides.
            this.relativeSides = relativeSides;

            // Set sides.
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
        #endregion

        #region Scale Functions
        /// <summary> Scales the given rectangle based on the values of this padding. </summary>
        /// <param name="baseRectangle"></param>
        /// <returns></returns>
        public Rectangle ScaleRectangle(Rectangle baseRectangle)
        {
            Rectangle scaledRectangle = new Rectangle()
            {
                X = baseRectangle.X + (int)Math.Floor(IsLeftRelative ? baseRectangle.Width * Left : Left),
                Y = baseRectangle.Y + (int)Math.Floor(IsTopRelative ? baseRectangle.Height * Top : Top),
                Width = baseRectangle.Width - (int)Math.Ceiling(IsRightRelative ? baseRectangle.Width * Right : Right),
                Height = baseRectangle.Height - (int)Math.Ceiling(IsBottomRelative ? baseRectangle.Height * Bottom : Bottom)
            };

            // Adjust the width and height as the X and Y has changed it.
            scaledRectangle.Width -= (int)Math.Floor(IsLeftRelative ? baseRectangle.Width * Left : Left);
            scaledRectangle.Height -= (int)Math.Floor(IsTopRelative ? baseRectangle.Height * Top : Top);
            
            return scaledRectangle;
        }

        public Rectangle InverseScaleRectangle(Rectangle baseRectangle)
        {
            Rectangle inverseScaledRectangle = new Rectangle()
            {
                X = baseRectangle.X - (int)Math.Floor(IsLeftRelative ? baseRectangle.Width / (1.0f - Left) : Left),
                Y = baseRectangle.Y - (int)Math.Floor(IsTopRelative ? baseRectangle.Height / (1.0f - Top) : Top),
                Width = baseRectangle.Width + (int)Math.Ceiling(IsRightRelative ? baseRectangle.Width / (1.0f - Right) : Right),
                Height = baseRectangle.Height + (int)Math.Ceiling(IsBottomRelative ? baseRectangle.Height / (1.0f - Bottom) : Bottom)
            };

            inverseScaledRectangle.Width += (int)Math.Floor(IsLeftRelative ? baseRectangle.Width / (1.0f - Left) : Left);
            inverseScaledRectangle.Height += (int)Math.Floor(IsTopRelative ? baseRectangle.Height / (1.0f - Top) : Top);

            return inverseScaledRectangle;
        }
        #endregion

        #region Static Parse Functions
        public static Sides Parse(string input)
        {
            // Try to parse the string, if it fails, throw an error; otherwise, return the parsed padding.
            if (!TryParse(input, out Sides padding)) throw new ArgumentException("Given string was not a valid padding.");
            else return padding;
        }

        public static bool TryParse(string input, out Sides sides)
        {
            // If the given string is empty, return with an empty padding.
            if (string.IsNullOrWhiteSpace(input)) { sides = new Sides(0, SideMask.None); return false; }

            // Split the input into sides.
            string[] paddingSides = input.Split(',');

            // Keep track of what sides should be relative.
            SideMask relativeSides = SideMask.None;

            // Parse the first value.
            if (!Relative.TryParse(paddingSides[0], out float top, out bool topRelative)) { sides = new Sides(0, SideMask.None); return false; }

            // If the first value is the only value, return a padding with all sides set to the value.
            if (paddingSides.Length == 1) { sides = new Sides(top, topRelative ? SideMask.All : SideMask.None); return true; }

            // Parse the right side.
            if (!Relative.TryParse(paddingSides[1], out float right, out bool rightRelative)) { sides = new Sides(0, SideMask.None); return false; }

            // If there are 2 given sides, use them for the vertical and horizontal sides.
            if (paddingSides.Length == 2)
            {
                // Set the bitflag based on which sides are relative.
                if (rightRelative) relativeSides |= SideMask.Horizontal;
                if (topRelative) relativeSides |= SideMask.Vertical;

                // Return a relative padding with the parsed values and sides.
                sides = new Sides(right, top, relativeSides);
                return true;
            }

            // Parse the bottom side.
            if (!Relative.TryParse(paddingSides[2], out float bottom, out bool bottomRelative)) { sides = new Sides(0, SideMask.None); return false; }

            // If there are 3 given sides, use them for the top, bottom, and horizontal sides.
            if (paddingSides.Length == 3)
            {
                // Set the bitflag based on which sides are relative.
                if (rightRelative) relativeSides |= SideMask.Horizontal;
                if (topRelative) relativeSides |= SideMask.Top;
                if (bottomRelative) relativeSides |= SideMask.Bottom;

                // Return a new relative padding with the parsed values and sides.
                sides = new Sides(top, right, bottom, relativeSides);
                return true;
            }

            // Parse the left side.
            if (!Relative.TryParse(paddingSides[3], out float left, out bool leftRelative)) { sides = new Sides(0, SideMask.None); return false; }

            // Set the bitflag based on which sides are relative.
            if (rightRelative) relativeSides |= SideMask.Right;
            if (leftRelative) relativeSides |= SideMask.Left;
            if (topRelative) relativeSides |= SideMask.Top;
            if (bottomRelative) relativeSides |= SideMask.Bottom;

            // Return a relative padding with all sides set.
            sides = new Sides(top, right, bottom, left, relativeSides);
            return true;
        }
        #endregion
    }
}
