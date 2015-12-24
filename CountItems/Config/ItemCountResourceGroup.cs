using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class ItemCountResourceGroup : ResourceGroup
    {
        public ItemCountResourceGroup()
        {
            AddFile("CountItems.js");
            AddFile("CountItemsButton.css");
            AddFile("CountItemsContextMenu.css");
            AddFile<ItemCountCommandSet>();
            Dependencies.AddAlchemyCore();
        }
    }
}