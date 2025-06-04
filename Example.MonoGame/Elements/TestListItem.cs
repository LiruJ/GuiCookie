using Example.MonoGame.DataStructures;
using GuiCookie.Core.Data;
using GuiCookie.Core.Elements;

namespace Example.MonoGame.Elements
{
    public class TestListItem(ListItemData item) : Element
    {
        public override void OnFullSetup(IReadOnlyAttributeCollection attributes)
        {
            GetChildByName<TextBox>("Key").Text = item.Key;
            GetChildByName<TextBox>("Value").Text = item.Value.ToString();
            //GetChildByName<TextButton>("RemoveButton").ConnectLeftClick(Destroy);
        }
    }
}
