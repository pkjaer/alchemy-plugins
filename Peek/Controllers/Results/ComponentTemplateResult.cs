using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class ComponentTemplateResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry TemplateType { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry DynamicTemplateInfo { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry Priority { get; set; }


        public static ComponentTemplateResult From(ComponentTemplateData item, ISessionAwareCoreService client, string currentUserId)
        {
            var result = new ComponentTemplateResult
            {
                TemplateType = TextEntry.From(LookUpTemplateType(item.TemplateType, ItemType.ComponentTemplate, client), Resources.LabelTemplateType)
            };

            if (item.IsRepositoryPublishable == true)
            {
                string dynamicTemplate = Resources.DynamicTemplateNotAllowedOnPage;
                if (item.AllowOnPage == true)
                {
                    dynamicTemplate = Resources.DynamicTemplateAllowedOnPage;
                }
                result.DynamicTemplateInfo = TextEntry.From(dynamicTemplate, Resources.LabelDynamicTemplateInfo);
            }

            if (item.Priority != null)
            {
                string priority = Resources.PriorityNeverLink;
                switch (item.Priority.Value)
                {
                    case 300: priority = Resources.PriorityHigh; break;
                    case 200: priority = Resources.PriorityMedium; break;
                    case 100: priority = Resources.PriorityLow; break;
                }
                result.Priority = TextEntry.From(priority, Resources.LabelPriority);
            }

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}