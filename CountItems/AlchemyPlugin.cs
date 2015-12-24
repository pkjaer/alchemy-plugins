using Alchemy4Tridion.Plugins;
using Alchemy.Plugins.CountItems.Config;

namespace Alchemy.Plugins.CountItems
{
    public class AlchemyPlugin : AlchemyPluginBase
    {
        public override void Configure(IPluginServiceLocator services)
        {
            services.SettingsDeserialization.ClientSettingsType = typeof(ClientSettings);
        }
    }
}
