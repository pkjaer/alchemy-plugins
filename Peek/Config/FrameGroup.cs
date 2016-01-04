using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.Peek.Config
{
    public class FrameGroup : ResourceGroup
    {
        public FrameGroup()
        {
            AddFile("Frame.js");
            AddFile("Frame.css");
            AddWebApiProxy();
            AttachToView("Frame.aspx");
            Dependencies.Add("Tridion.Web.UI.Editors.CME");
        }
    }
}
