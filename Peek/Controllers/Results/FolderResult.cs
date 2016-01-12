using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class FolderResult : PeekResult
    {
        [JsonProperty(Order=1)]
        public LinkEntry LinkedSchema { get; set; }


        public static FolderResult From(FolderData item, string currentUserId)
        {
            var result = new FolderResult { LinkedSchema = LinkEntry.From(item.LinkedSchema, Resources.LabelLinkedSchema, currentUserId) };

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}