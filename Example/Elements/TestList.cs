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
        public void Add(ListItemData testListItem)
        {
            elementManager.CreateElementFromTemplateName("TestListItem", null, this, testListItem);
        }
        #endregion
    }
}