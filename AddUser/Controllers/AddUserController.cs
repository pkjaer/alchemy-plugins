using System.Web.Http;
using Alchemy4Tridion.Plugins;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.AddUser.Controllers
{
    public class UserInfoModel
    {
        public string Name;
        public string Description;
    }

    [AlchemyRoutePrefix("AddUserService")]
    public class AddUserController : AlchemyApiController
    {
        [HttpPost]
        [Route(@"NewUser")]
        public IHttpActionResult NewUser(UserInfoModel data)
        {
            if (string.IsNullOrWhiteSpace(data.Name))
            {
                return BadRequest("Name is required.");
            }

            var defaultReadOptions = new ReadOptions();
            var user = (UserData)Client.GetDefaultData(ItemType.User, null, defaultReadOptions);
            user.Title = data.Name;

            if (!string.IsNullOrWhiteSpace(data.Description))
            {
                user.Description = data.Description;
            }

            return Ok(Client.Create(user, defaultReadOptions).Id);
        }
    }
}
