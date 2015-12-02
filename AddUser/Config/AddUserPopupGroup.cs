using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class AddUserPopupGroup : ResourceGroup
    {
        public AddUserPopupGroup()
        {
            AddFile("AddUserPopup.js");
            AddFile("AddUserPopup.css");
            AttachToView("AddUserPopup.aspx");
            Dependencies.Add("Tridion.Web.UI.Editors.CME");
        }
    }
}
