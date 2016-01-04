using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class ToolbarButton : RibbonToolbarExtension
    {
        public ToolbarButton()
        {
            AssignId = @"AddUserByName";
            Command = @"AddUserByName";

            Name = Resources.ToolbarButtonName;
            Title = Resources.ToolbarButtonTooltip;
            PageId = Constants.PageIds.AdministrationPage;
            GroupId = @"AccessManagementGroup";
            InsertBefore = @"UsersDropdown";

            Dependencies.Add<ButtonGroup>();
            Apply.ToView(Constants.Views.DashboardView, "DashboardToolbar");
        }
    }
}
