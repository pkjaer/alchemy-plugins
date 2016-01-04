using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class ToolbarButton : RibbonToolbarExtension
    {
        public ToolbarButton()
        {
            AssignId = @"CountItems";
            Command = @"CountItems";

            Name = Resources.ToolbarButtonName;
            Title = Resources.ToolbarButtonTooltip;
            PageId = Constants.PageIds.HomePage;
            GroupId = Constants.GroupIds.HomePage.ManageGroup;
            InsertBefore = @"WhereUsedBtn";

            Dependencies.Add<ButtonGroup>();
            Apply.ToView(Constants.Views.DashboardView, "DashboardToolbar");
        }
    }
}
