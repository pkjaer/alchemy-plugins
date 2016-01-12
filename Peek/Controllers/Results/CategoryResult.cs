using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class CategoryResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry XmlName { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry Publishable { get; set; }
        [JsonProperty(Order = 4)]
        public TextEntry UseForIdentification { get; set; }
        [JsonProperty(Order = 5)]
        public LinkEntry LinkedSchema { get; set; }


        public static CategoryResult From(CategoryData item, ISessionAwareCoreService client, string currentUserId)
        {
            var result = new CategoryResult 
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription), 
                XmlName = TextEntry.From(item.XmlName, Resources.LabelXmlName)
            };

            if (item.UseForNavigation == false)
            {
                result.Publishable = TextEntry.From(Resources.CannotBePublished, Resources.LabelPublishable);
            }

            if (item.UseForIdentification == true)
            {
                result.UseForIdentification = TextEntry.From(Resources.Yes, Resources.LabelUseForIdentification);
            }

            result.LinkedSchema = LinkEntry.From(item.KeywordMetadataSchema, Resources.LabelLinkedSchema, currentUserId);

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}