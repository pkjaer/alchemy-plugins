using System.Linq;
using System.Text;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class PublicationTargetResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry TargetLanguage { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry DefaultCodePage { get; set; }
        [JsonProperty(Order = 4)]
        public TextEntry Priority { get; set; }
        [JsonProperty(Order = 5)]
        public TextEntry Destinations { get; set; }


        public static PublicationTargetResult From(PublicationTargetData item)
        {
            var result = new PublicationTargetResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription),
                TargetLanguage = TextEntry.From(item.TargetLanguage, Resources.LabelTargetLanguage)
            };

            if (item.DefaultCodePage != null)
            {
                string codePage = "System Default";
                if (item.DefaultCodePage > 0)
                {
                    var encoding = Encoding.GetEncodings().FirstOrDefault(e => e.CodePage == item.DefaultCodePage);
                    if (encoding != null)
                    {
                        codePage = encoding.DisplayName;
                    }
                }
                result.DefaultCodePage = TextEntry.From(codePage, Resources.LabelDefaultCodePage);
            }

            if (item.Priority != null)
            {
                string priority = Resources.PriorityMedium;
                
                switch (item.Priority.Value)
                {
                    case PublishPriority.High:
                        priority = Resources.PriorityHigh;
                        break;
                    case PublishPriority.Low:
                        priority = Resources.PriorityLow;
                        break;
                }

                result.Priority = TextEntry.From(priority, Resources.LabelPriority);
            }

            var destinations = item.Destinations.Select(
                    destination => string.Format(Resources.PublicationTargetDestination, destination.Title, destination.ProtocolSchema.Title)
                ).ToList();

            result.Destinations = TextEntry.From(string.Join(", ", destinations), Resources.LabelDestinations);

            AddCommonProperties(item, result);
            return result;
        }
    }
}