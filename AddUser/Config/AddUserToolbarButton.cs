using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class AddUserToolbarButton : RibbonToolbarExtension
    {
        public AddUserToolbarButton()
        {
            AssignId = @"AddUserByName";
            Command = @"AddUserByName";

            Name = Resources.ToolbarButtonName;
            Title = Resources.ToolbarButtonTooltip;
            PageId = Constants.PageIds.AdministrationPage;
            GroupId = @"AccessManagementGroup";
            InsertBefore = @"UsersDropdown";

            Dependencies.Add<AddUserButtonGroup>();
            Apply.ToView(Constants.Views.DashboardView, "DashboardToolbar");
        }
    }
}
