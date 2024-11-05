namespace GuiCookie.Core.Input
{
    [Flags]
    public enum MouseButton
    {
        None            = 0b0000_0000,
        LeftButton      = 0b0000_0001,
        RightButton     = 0b0000_0010,
        MiddleButton    = 0b0000_0100,
        XButton1        = 0b0000_1000,
        XButton2        = 0b0001_0000,
    }
}
