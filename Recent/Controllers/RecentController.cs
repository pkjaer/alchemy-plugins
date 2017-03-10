using System.Web.Http;
using Alchemy4Tridion.Plugins;

namespace Alchemy.Plugins.Recent.Controllers
{
    [AlchemyRoutePrefix("RecentService")]
    public class RecentController : AlchemyApiController
    {
        private RecentItems _recentItems;

        public void Initialize()
        {
            if (_recentItems == null)
            {
                _recentItems = new RecentItems(Client);
            }
        }

        [HttpPost]
        [Route("SaveRecentItem")]
        public void SaveRecentItem(SaveRecentItemParameters parameters)
        {
            Initialize();
            _recentItems.Add(RecentItem.From(parameters));
        }

        [HttpGet]
        [Route("GetRecentItems")]
        public GetRecentItemsResult GetRecentItems()
        {
            Initialize();
            return GetRecentItemsResult.From(_recentItems.GetItems());
        }
    }
}