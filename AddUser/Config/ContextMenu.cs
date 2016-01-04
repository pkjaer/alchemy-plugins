using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class ContextMenu : ContextMenuExtension
    {
        public ContextMenu()
        {
            AssignId = "";
            Name = "";
            InsertBefore = "cm_new_user";

            AddItem("alch_adduserbyname", Resources.ContextMenuEntry, "AddUserByName");

            Dependencies.Add<ButtonGroup>();
            Apply.ToView(Constants.Views.DashboardView);
        }
    }
}