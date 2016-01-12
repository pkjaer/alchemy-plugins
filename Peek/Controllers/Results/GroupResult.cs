using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class GroupResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry GroupMemberships { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry Scope { get; set; }


        public static GroupResult From(GroupData item)
        {
            var result = new GroupResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription)
            };
            

            if (item.GroupMemberships != null)
            {
                result.GroupMemberships = TextEntry.From(GetGroupMembershipSummary(item.GroupMemberships), Resources.LabelGroupMemberships);
            }

            string scope = item.Scope.Length > 0
                ? Resources.GroupScopeSpecificPublications
                : Resources.GroupScopeAllPublications;

            result.Scope = TextEntry.From(scope, Resources.LabelScope);

            AddCommonProperties(item, result);
            return result;
        }
    }
}