using Example.DataStructures;
using GuiCookie.Elements;

namespace Example.Elements
{
    public class TestListItem : Element
    {
        private readonly ListItemData item;

        public TestListItem(ListItemData item)
        {
            this.item = item;
        }

        public override void OnFullSetup()
        {
            GetChildByName<TextBox>("Key").Text = item.Key;
            GetChildByName<TextBox>("Value").Text = item.Value.ToString();
            GetChildByName<TextButton>("RemoveButton").ConnectLeftClick(Destroy);
        }
    }
}
