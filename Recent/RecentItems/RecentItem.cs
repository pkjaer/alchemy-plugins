using System;
using Alchemy.Plugins.Recent.Controllers;

namespace Alchemy.Plugins.Recent
{
    [Serializable()]
    public class RecentItem
    {
        public string Id;
        public string Title;
        public string Icon;

        public static RecentItem From(SaveRecentItemParameters parameters)
        {
            return new RecentItem { Id = parameters.Id, Title = parameters.Title, Icon = parameters.Icon };
        }
    }
}