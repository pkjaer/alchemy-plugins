using System.Globalization;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;
using System.Linq;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class TargetGroupResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }
        [JsonProperty(Order = 11)]
        public TextEntry Conditions { get; set; }


        public static TargetGroupResult From(TargetGroupData item, string currentUserId)
        {
            int count = item.Conditions.Count();
            var result = new TargetGroupResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription),
                MetadataSchema = LinkEntry.From(item.MetadataSchema, Resources.LabelMetadataSchema, currentUserId),
                Conditions = TextEntry.From(count > 0 ? count.ToString(CultureInfo.InvariantCulture) : Resources.None, Resources.LabelConditions)
            };

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}