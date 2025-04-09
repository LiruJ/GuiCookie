namespace GuiCookie.Core.Data
{
    public class SheetDataNode(SheetDataNode? parent, string name, AttributeCollection attributes) : IReadOnlySheetDataNode
    {
        #region Backing Fields
        protected readonly List<SheetDataNode> childNodes = [];
        #endregion

        #region Properties
        public SheetDataNode? Parent { get; } = parent;

        public IReadOnlyList<SheetDataNode> ChildNodes => childNodes;

        public string Name { get; set; } = name;

        public AttributeCollection Attributes { get; } = attributes;

        IReadOnlyAttributeCollection IReadOnlySheetDataNode.Attributes => Attributes;

        IReadOnlyList<IReadOnlySheetDataNode> IReadOnlySheetDataNode.ChildNodes => ChildNodes;

        IReadOnlySheetDataNode? IReadOnlySheetDataNode.Parent => Parent;
        #endregion

        #region Get Functions
        public SheetDataNode? GetChildWithName(string name) => childNodes.FirstOrDefault(x => x.Name == name);

        IReadOnlySheetDataNode? IReadOnlySheetDataNode.GetChildWithName(string name) => GetChildWithName(name);
        #endregion
    }
}
