using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alchemy.Plugins.Recent.Controllers
{
    public class GetRecentItemsResult
    {
        public RecentItem[] Items;

        public static GetRecentItemsResult From(IEnumerable<RecentItem> entries)
        {
            return new GetRecentItemsResult {Items = entries?.ToArray()};
        }
    }
}