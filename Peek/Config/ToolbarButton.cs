using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.Peek.Config
{
    public class ToolbarButton : RibbonToolbarExtension
    {
        public ToolbarButton()
        {
            AssignId = @"Peek";
            Command = @"Peek";

            Name = Resources.ToolbarButtonName;
            Title = Resources.ToolbarButtonTooltip;
            PageId = Constants.PageIds.HomePage;
            GroupId = Constants.GroupIds.HomePage.EditGroup;
            InsertBefore = @"PreviewBtn";

            Dependencies.Add<ButtonGroup>();
            Apply.ToView(Constants.Views.DashboardView, "DashboardToolbar");
        }
    }
}
