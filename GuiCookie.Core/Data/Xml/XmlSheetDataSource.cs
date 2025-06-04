using System.Xml;

namespace GuiCookie.Core.Data.Xml
{
    public class XmlSheetDataSource(string? filePath, SheetDataNode rootNode) : SheetDataSource(filePath, rootNode)
    {
        #region Load Functions
        public static XmlSheetDataSource Load(string filePath)
        {
            filePath = Path.HasExtension(filePath) ? filePath : Path.ChangeExtension(filePath, "xml");

            using FileStream stream = File.OpenRead(filePath);
            return Load(stream, filePath);
        }

        public static XmlSheetDataSource Load(Stream stream, string? filePath = null)
        {
            XmlDocument sheetDocument = new();
            sheetDocument.Load(stream);

            XmlNode? rootNode = sheetDocument.LastChild;
            if (rootNode == null || rootNode.NodeType != XmlNodeType.Element)
                throw new ArgumentException("Given xml sheet has no valid root node!");

            SheetDataNode rootDataNode = XmlSheetDataNode.LoadFromXMLNode(null, rootNode);

            return new XmlSheetDataSource(filePath, rootDataNode);
        }
        #endregion
    }
}
