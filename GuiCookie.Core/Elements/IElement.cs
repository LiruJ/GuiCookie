using GuiCookie.Core.DataStructures;

namespace GuiCookie.Core.Elements
{
    public interface IElement
    {
        #region Properties
        /// <summary> The bounds of the element. </summary>
        public Bounds Bounds { get; }
        InitialisationState InitialisationState { get; }
        string? Name { get; set; }
        bool HasName { get; }
        string? Tag { get; }
        bool HasTag { get; }
        Element? Parent { get; set; }
        bool BlocksMouse { get; set; }
        bool EnabledAndVisible { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        bool HasChildren { get; }
        int ChildCount { get; }
        #endregion
    }
}
