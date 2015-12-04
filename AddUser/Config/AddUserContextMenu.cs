using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class AddUserContextMenu : ContextMenuExtension
    {
        public AddUserContextMenu()
        {
            AssignId = "";
            Name = "";
            InsertBefore = "cm_new_user";

            AddItem("alch_adduserbyname", Resources.ContextMenuEntry, "AddUserByName");

            Dependencies.Add<AddUserButtonGroup>();
            Apply.ToView(Constants.Views.DashboardView);
        }
    }
}