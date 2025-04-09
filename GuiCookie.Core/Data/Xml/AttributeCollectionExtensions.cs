using System.Xml;

namespace GuiCookie.Core.Data.Xml
{
    public static class AttributeCollectionExtensions
    {
        public static AttributeCollection CreateFromXmlNode(XmlNode elementNode)
        {
            if (elementNode.Attributes == null)
                return [];

            return CreateFromXmlAttributes(elementNode.Attributes);
        }

        public static AttributeCollection CreateFromXmlAttributes(XmlAttributeCollection xmlAttributeCollection)
        {
            AttributeCollection attributes = [];
            foreach (XmlAttribute attribute in xmlAttributeCollection)
                attributes.Add(attribute.Name, attribute.InnerText);
            return attributes;
        }
    }
}
