namespace GuiCookie.DataStructures
{
    public enum ClippingMode
    {
        /// <summary> No cipping mode, content will overflow. </summary>
        None = 0,

        /// <summary> Cuts the content to fit. </summary>
        Clip,

        /// <summary> Squeezes the content to fit. </summary>
        Squeeze,

        /// <summary> Stretches the content to fill the whole area. </summary>
        Stretch
    }
}
