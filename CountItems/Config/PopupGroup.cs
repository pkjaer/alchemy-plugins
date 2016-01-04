using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.CountItems.Config
{
    public class PopupGroup : ResourceGroup
    {
        public PopupGroup()
        {
            AddFile("Popup.js");
            AddFile("Popup.css");
            AddWebApiProxy();
            AttachToView("CountItemsPopup.aspx");
            Dependencies.Add("Tridion.Web.UI.Editors.CME");
        }
    }
}
