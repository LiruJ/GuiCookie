using System;

namespace GuiCookie.DataStructures
{
    [Flags]
    public enum InitialisationState : byte
    {
        None        = 0b0000,
        Created     = 0b1000,
        Setup       = 0b0100,
        PostSetup   = 0b0010,
        FullySetup  = 0b1110
    }
}
