namespace GuiCookie.Core.Data
{
    public abstract class SheetDataSource(string filePath, SheetDataNode rootNode) : IReadOnlySheetDataSource
    {
        #region Properties
        public string FilePath { get; } = filePath;

        public SheetDataNode RootNode { get; } = rootNode;

        IReadOnlySheetDataNode IReadOnlySheetDataSource.RootNode => RootNode;
        #endregion

        #region Get Functions
        public SheetDataNode? GetChildWithName(string name) => RootNode.ChildNodes.FirstOrDefault(x => x.Name == name);

        IReadOnlySheetDataNode? IReadOnlySheetDataSource.GetChildWithName(string name) => GetChildWithName(name);
        #endregion
    }
}
