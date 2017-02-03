using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.Recent.Config
{
    public class TreeResourceGroup : ResourceGroup
    {
        public TreeResourceGroup()
        {
            AddFile("Dashboard.js");
            AddFile("Tree.css");

            AddFile("SaveRecent.js");
            AddFile<SaveRecentCommand>();
            AddWebApiProxy();
            Dependencies.AddAlchemyCore();
        }
    }
}