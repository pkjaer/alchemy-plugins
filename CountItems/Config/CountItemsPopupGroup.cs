using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class CountItemsPopupGroup : ResourceGroup
    {
        public CountItemsPopupGroup()
        {
            AddFile("CountItemsPopup.js");
            AddFile("CountItemsPopup.css");
            AddWebApiProxy();
            AttachToView("CountItemsPopup.aspx");
            Dependencies.Add("Tridion.Web.UI.Editors.CME");
        }
    }
}
