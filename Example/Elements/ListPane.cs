using Example.DataStructures;
using GuiCookie.Components;
using GuiCookie.Elements;
using System;

namespace Example.Elements
{
    public class ListPane : Element
    {
        #region Dependencies
        private readonly Random random;
        #endregion

        #region Elements
        private DirectionalLayout list = null;

        private LabelledSlider spaceSlider = null;

        private Button addItemButton = null;
        #endregion

        #region Fields
        private int totalItems = 0;
        #endregion

        #region Constructors
        public ListPane(Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }
        #endregion

        #region Initialisation Functions
        public override void OnFullSetup()
        {
            list = GetChildByName("List").GetComponent<DirectionalLayout>();
            spaceSlider = GetChildByName<LabelledSlider>("SpaceSlider");
            addItemButton = GetChildByName<Button>("AddItemButton");
        }

        public override void OnPostFullSetup()
        {
            // Add a bunch of random items to the list.
            for (int i = 0; i < 10; i++)
                AddRandom();

            // Connect the slider to change the spacing.
            spaceSlider?.Slider?.ConnectValueChanged(() => list.Spacing = (int)spaceSlider.Slider.Value);

            // Connect the add item button to add a random item.
            addItemButton.ConnectLeftClick(AddRandom);
        }
        #endregion

        #region List Functions
        /// <summary> Adds a random item to the list. </summary>
        public void AddRandom() => Add(new ListItemData(random.Next(0, 9999), $"Item {totalItems}"));

        /// <summary> Adds the given <paramref name="testListItem"/> to the list. </summary>
        /// <param name="testListItem"> The item to add. </param>
        public void Add(ListItemData testListItem)
        {
            elementManager.CreateElementFromTemplateName("TestListItem", null, list.Element, testListItem);
            
            // Increment the number of total items added to the list.
            totalItems++;
        }
        #endregion
    }
}