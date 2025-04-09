using GuiCookie.Core.Data;
using GuiCookie.Core.DataStructures;
using GuiCookie.Core.Roots;
using GuiCookie.Core.Styles;
using GuiCookie.Core.Templates;

namespace GuiCookie.Core.Elements
{
    public interface IElement
    {
        #region Properties
        /// <summary> The bounds of the element. </summary>
        public Bounds Bounds { get; }
        Style Style { get; set; }
        StyleVariant CurrentStyleVariant { get; }
        InitialisationState InitialisationState { get; }
        Template Template { get; }
        string Name { get; set; }
        bool HasName { get; }
        string Tag { get; }
        bool HasTag { get; }
        Root Root { get; }
        IReadOnlyAttributeCollection Attributes { get; }
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
