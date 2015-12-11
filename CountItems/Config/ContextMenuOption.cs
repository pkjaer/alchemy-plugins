using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class ContextMenuOption : ContextMenuExtension
    {
        public ContextMenuOption()
        {
            AssignId = "";
            Name = "";
            InsertBefore = "cm_whereused";
            AddItem("alch_countitems", Resources.ContextMenuEntry, "CountItems");

            Dependencies.Add<ItemCountResourceGroup>();
            Apply.ToView(Constants.Views.DashboardView);
        }
    }
}