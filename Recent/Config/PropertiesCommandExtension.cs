using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.Recent.Config
{
    public class PropertiesCommandExtension : CommandExtension
    {
        public PropertiesCommandExtension()
        {
            Name = "Properties";
            EditorExtensionTarget = "CME";
            ExtendingCommand = "SaveRecent";
        }
    }
}