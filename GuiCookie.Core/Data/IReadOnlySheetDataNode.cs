namespace GuiCookie.Core.Data
{
    public interface IReadOnlySheetDataNode
    {
        #region Properties
        IReadOnlyAttributeCollection Attributes { get; }
        IReadOnlyList<IReadOnlySheetDataNode> ChildNodes { get; }
        string Name { get; }
        IReadOnlySheetDataNode? Parent { get; }
        #endregion

        IReadOnlySheetDataNode? GetChildWithName(string name);
    }
}