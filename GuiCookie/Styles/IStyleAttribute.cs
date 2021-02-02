namespace GuiCookie.Styles
{
    public interface IStyleAttribute
    {
        /// <summary> The name of this style attribute. </summary>
        string Name { get; }

        /// <summary> Fully copies all attribute values to a new object. </summary>
        /// <returns></returns>
        IStyleAttribute CreateCopy();

        /// <summary> This is called when this style attribute must be combined with another in order to create a hybrid style. This usually happens for things like a style's hover attributes overriding the base. </summary>
        /// <param name="baseAttribute"></param>
        void OverrideBaseAttribute(IStyleAttribute baseAttribute);
    }
}