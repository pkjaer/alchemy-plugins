using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.Peek.Config
{
    public class ButtonGroup : ResourceGroup
    {
        public ButtonGroup()
        {
            AddFile("Peek.js");
            AddFile("Dashboard.js");
            AddFile("FadeAnimation.js");
            AddFile("ResizeAnimation.js");
            AddFile("FrameManager.js");

            AddFile("ContextMenu.css");
            AddFile("Dashboard.css");
            AddFile("ToolbarButton.css");

            AddFile<Commands>();
            Dependencies.AddAlchemyCore();
        }
    }
}