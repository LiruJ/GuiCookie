using System;

namespace GuiCookie.DataStructures
{
    /// <summary> Represents four different directions. </summary>
    [Flags]
    public enum SideMask : byte
    {
        None = 0b0000_0000,                 // No direction.
        All = Left | Top | Right | Bottom,  // All directions.
        Top = 0b0000_0001,                  // Top of an element.
        Right = 0b0000_0010,                // Right of an element.
        Bottom = 0b0000_0100,               // Bottom of an element.
        Left = 0b0000_1000,                 // Left of an element.
        Horizontal = Left | Right,          // Both left and right.
        Vertical = Top | Bottom             // Both top and bottom.
    }
}
