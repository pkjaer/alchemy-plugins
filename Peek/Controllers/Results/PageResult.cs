using System.Globalization;
using System.Linq;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class PageResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry FileName { get; set; }
        [JsonProperty(Order = 2)]
        public LinkEntry Template { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry ComponentPresentations { get; set; }
        [JsonProperty(Order = 14)]
        public TextEntry PathOnWebsite { get; set; }


        public static PageResult From(PageData item, ISessionAwareCoreService client, string currentUserId)
        {
            var template = (PageTemplateData)client.Read(item.PageTemplate.IdRef, new ReadOptions());
            string extension = template.FileExtension;

            var result = new PageResult
            {
                Template = LinkEntry.From(item.PageTemplate, Resources.LabelTemplate, currentUserId),
                FileName = TextEntry.From(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", item.FileName, extension), Resources.LabelFileName),
            };

            if (result.FileName != null)
            {
                result.PathOnWebsite = GetPublishPath(item.LocationInfo as PublishLocationInfo, result.FileName.Value);
            }

            string componentPresentations = Resources.None;
            if (item.ComponentPresentations.Any())
            {
                int count = item.ComponentPresentations.Count();
                int templateCount = item.ComponentPresentations.DistinctBy(cp => cp.ComponentTemplate.IdRef).Count();
                componentPresentations = (templateCount == 1)
                    ? string.Format(CultureInfo.InvariantCulture, Resources.ComponentPresentationSummarySameTemplate, count)
                    : string.Format(CultureInfo.InvariantCulture, Resources.ComponentPresentationSummary, count, templateCount);
            }

            result.ComponentPresentations = TextEntry.From(componentPresentations, Resources.LabelComponentPresentations);

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }
    }
}