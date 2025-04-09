using System.Xml;

namespace GuiCookie.Core.Data.Xml
{
    public class XmlSheetDataNode(SheetDataNode? parent, string name, AttributeCollection attributes) : SheetDataNode(parent, name, attributes)
    {
        #region Load Functions
        public static SheetDataNode LoadFromXMLNode(SheetDataNode? parent, XmlNode node)
        {
            AttributeCollection attributes = AttributeCollectionExtensions.CreateFromXmlNode(node);

            XmlSheetDataNode dataNode = new(parent, node.Name, attributes);
            dataNode.childNodes.Capacity = node.ChildNodes.Count;
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element)
                    continue;

                SheetDataNode childDataNode = LoadFromXMLNode(dataNode, childNode);
                dataNode.childNodes.Add(childDataNode);
            }

            return dataNode;
        }
        #endregion
    }
}
