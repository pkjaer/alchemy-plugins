using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class TemplateBuildingBlockResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public LinkEntry ParametersSchema { get; set; }
        [JsonProperty(Order = 11)]
        public TextEntry TemplateType { get; set; }


        public static TemplateBuildingBlockResult From(TemplateBuildingBlockData item, ISessionAwareCoreService client, string currentUserId)
        {
            var result = new TemplateBuildingBlockResult
            {
                ParametersSchema = LinkEntry.From(item.ParameterSchema, Resources.LabelParametersSchema, currentUserId),
                MetadataSchema = LinkEntry.From(item.MetadataSchema, Resources.LabelMetadataSchema, currentUserId),
                TemplateType = TextEntry.From(LookUpTemplateType(item.TemplateType, ItemType.TemplateBuildingBlock, client), Resources.LabelTemplateType)
            };

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}