using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiCookie.DataStructures
{
    public enum Direction { None = 0, Horizontal, Vertical }

    [Flags]
    public enum DirectionMask
    {
        None = 0,
        Horizontal = 0b1,
        Vertical = 0b10,
        Both = 0b11
    }
}
