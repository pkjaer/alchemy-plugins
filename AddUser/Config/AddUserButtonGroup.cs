using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class AddUserButtonGroup : ResourceGroup
    {
        public AddUserButtonGroup()
        {
            AddFile("AddUserByNameCommand.js");
            AddFile("AddUserButton.css");
            AddFile<AddUserCommandSet>();
            AddWebApiProxy();
            Dependencies.AddAlchemyCore();
        }
    }
}
