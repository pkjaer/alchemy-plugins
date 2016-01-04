using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class ContextMenu : ContextMenuExtension
    {
        public ContextMenu()
        {
            AssignId = "";
            Name = "";
            InsertBefore = "cm_whereused";
            AddItem("alch_countitems", Resources.ContextMenuEntry, "CountItems");

            Dependencies.Add<ButtonGroup>();
            Apply.ToView(Constants.Views.DashboardView);
        }
    }
}