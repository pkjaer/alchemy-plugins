using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class KeywordResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry Key { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry IsAbstract { get; set; }


        public static KeywordResult From(KeywordData item, string currentUserId)
        {
            var result = new KeywordResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription),
                Key = TextEntry.From(item.Key, Resources.LabelKey)
            };

            if (item.IsAbstract == true)
            {
                result.IsAbstract = TextEntry.From(Resources.Yes, Resources.LabelIsAbstract);
            }


            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}