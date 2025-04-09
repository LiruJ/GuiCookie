
namespace GuiCookie.Core.Data
{
    public interface IReadOnlySheetDataSource
    {
        string FilePath { get; }
        IReadOnlySheetDataNode RootNode { get; }

        IReadOnlySheetDataNode? GetChildWithName(string name);
    }
}