using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class AddUserToolbarButton : RibbonToolbarExtension
    {
        public AddUserToolbarButton()
        {
            AssignId = @"CountItems";
            Command = @"CountItems";

            Name = Resources.ToolbarButtonName;
            Title = Resources.ToolbarButtonTooltip;
            PageId = Constants.PageIds.HomePage;
            GroupId = Constants.GroupIds.HomePage.ManageGroup;
            InsertBefore = @"WhereUsedBtn";

            Dependencies.Add<ItemCountResourceGroup>();
            Apply.ToView(Constants.Views.DashboardView, "DashboardToolbar");
        }
    }
}
