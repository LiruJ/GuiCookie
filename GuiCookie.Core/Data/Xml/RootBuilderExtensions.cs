using GuiCookie.Core.Screens;

namespace GuiCookie.Core.Data.Xml
{
    public static class RootBuilderExtensions
    {
        #region Layout Sheet Functions
        public static GuiScreenBuilder<T> WithXmlLayoutSheet<T>(this GuiScreenBuilder<T> rootBuilder, string filePath) where T : GuiScreen 
            => rootBuilder.WithLayoutSheet(() => XmlSheetDataSource.Load(filePath));
        #endregion

        #region Style Sheet Functions
        public static GuiScreenBuilder<T> WithXmlStyleSheet<T>(this GuiScreenBuilder<T> rootBuilder, string filePath) where T : GuiScreen 
            => rootBuilder.WithStyleSheet(() => XmlSheetDataSource.Load(filePath));
        #endregion

        #region Template Functions
        public static GuiScreenBuilder<T> WithXmlTemplateSheet<T>(this GuiScreenBuilder<T> rootBuilder, string filePath) where T : GuiScreen 
            => rootBuilder.WithTemplateSheet(() => XmlSheetDataSource.Load(filePath));
        #endregion
    }
}
