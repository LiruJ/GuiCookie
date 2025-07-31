namespace GuiCookie.Core.Data
{
    public readonly record struct SheetNodePathPair(IReadOnlySheetDataNode Node, string FilePath)
    {
        public IReadOnlySheetDataNode Node { get; } = Node;
        public string FilePath { get; } = FilePath;
    }
}
