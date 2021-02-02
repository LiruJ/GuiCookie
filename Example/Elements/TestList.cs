using Example.DataStructures;
using GuiCookie.Elements;

namespace Example.Elements
{
    public class TestList : Element
    {
        #region Elements

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Initialisation Functions
        public override void OnPostFullSetup()
        {

        }

        public override void OnCreated()
        {

        }

        public override void OnFullSetup()
        {

        }
        #endregion

        #region List Functions
        public void Add(TestListItem testListItem)
        {

            Element listItem = elementManager.CreateElementFromTemplateName("TestListItem", null, this);
            listItem.GetChildByName<TextBox>("Key").Text = testListItem.Key;
            listItem.GetChildByName<TextBox>("Value").Text = testListItem.Value.ToString();
            listItem.GetChildByName<TextButton>("RemoveButton").ConnectLeftClick(listItem.Destroy);
        }
        #endregion
    }
}