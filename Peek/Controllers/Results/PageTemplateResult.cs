using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class PageTemplateResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Extension { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry TemplateType { get; set; }


        public static PageTemplateResult From(PageTemplateData item, ISessionAwareCoreService client, string currentUserId)
        {
            var result = new PageTemplateResult
            {
                Extension = TextEntry.From(item.FileExtension, Resources.LabelExtension),
                TemplateType = TextEntry.From(LookUpTemplateType(item.TemplateType, ItemType.PageTemplate, client), Resources.LabelTemplateType)
            };

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}