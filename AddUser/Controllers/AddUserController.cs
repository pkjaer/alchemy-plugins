using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Alchemy4Tridion.Plugins;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.AddUser.Controllers
{
    public class UserInfoModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
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

            try
            {
                var defaultReadOptions = new ReadOptions();
                var user = (UserData)Client.GetDefaultData(ItemType.User, null, defaultReadOptions);
                user.Title = data.Name;
                user.Description = data.Name;

                if (!string.IsNullOrWhiteSpace(data.Description))
                {
                    user.Description = data.Description;
                }

                return Ok(Client.Create(user, defaultReadOptions).Id);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }
    }
}
