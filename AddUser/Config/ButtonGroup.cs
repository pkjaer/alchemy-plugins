using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class ButtonGroup : ResourceGroup
    {
        public ButtonGroup()
        {
            AddFile("AddUserByNameCommand.js");
            AddFile("ToolbarButton.css");
            AddFile("ContextMenu.css");
            AddFile<Commands>();
            AddWebApiProxy();
            Dependencies.AddAlchemyCore();
        }
    }
}
