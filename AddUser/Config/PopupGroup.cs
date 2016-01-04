using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class PopupGroup : ResourceGroup
    {
        public PopupGroup()
        {
            AddFile("Popup.js");
            AddFile("Popup.css");
            AttachToView("AddUserPopup.aspx");
            Dependencies.Add("Tridion.Web.UI.Editors.CME");
        }
    }
}
