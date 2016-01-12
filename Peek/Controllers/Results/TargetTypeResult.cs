using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class TargetTypeResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }


        public static TargetTypeResult From(TargetTypeData item)
        {
            var result = new TargetTypeResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription)
            };

            AddCommonProperties(item, result);
            return result;
        }
    }
}