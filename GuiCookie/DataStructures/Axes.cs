using System;

namespace GuiCookie.DataStructures
{
    [Flags]
    public enum Axes : byte
    {
        X = 0b0000_0001,
        Y = 0b0000_0010,
        Both = X | Y,
        None = 0
    }
}
