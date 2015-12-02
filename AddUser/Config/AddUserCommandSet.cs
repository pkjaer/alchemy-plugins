using Alchemy4Tridion.Plugins.GUI.Configuration;

namespace Alchemy.Plugins.AddUser.Config
{
    public class AddUserCommandSet : CommandSet
    {
        public AddUserCommandSet()
        {
            AddCommand("AddUserByName");
        }
    }
}
