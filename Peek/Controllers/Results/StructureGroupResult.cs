using System.Globalization;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class StructureGroupResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Directory { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry Publishable { get; set; }
        [JsonProperty(Order = 3)]
        public LinkEntry Template { get; set; }
        [JsonProperty(Order = 14)]
        public TextEntry PathOnWebsite { get; set; }


        public static StructureGroupResult From(StructureGroupData item, string currentUserId)
        {
            var result = new StructureGroupResult
            {
                Directory = TextEntry.From(string.IsNullOrWhiteSpace(item.Directory) ? Resources.EmptyLabel : item.Directory, Resources.LabelDirectory),
                Template = LinkEntry.From(item.DefaultPageTemplate, Resources.LabelTemplate, currentUserId),
                PathOnWebsite = GetPublishPath(item.LocationInfo as PublishLocationInfo, item.Directory)
            };
            
            if (item.IsActiveResolvedValue == false)
            {
                string publishable = Resources.CannotBePublished;
                if (item.IsActive == true)
                {
                    publishable = Resources.CannotBePublishedDueToAncestor;
                }
                result.Publishable = TextEntry.From(publishable, Resources.LabelPublishable);
            }

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}