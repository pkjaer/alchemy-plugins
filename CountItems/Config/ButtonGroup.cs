using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class ButtonGroup : ResourceGroup
    {
        public ButtonGroup()
        {
            AddFile("CountItems.js");
            AddFile("ToolbarButton.css");
            AddFile("ContextMenu.css");
            AddFile<Commands>();
            Dependencies.AddAlchemyCore();
        }
    }
}