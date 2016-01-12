using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class VirtualFolderResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }


        public static VirtualFolderResult From(VirtualFolderData item, string currentUserId)
        {
            var result = new VirtualFolderResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription)
            };

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}