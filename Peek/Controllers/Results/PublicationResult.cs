using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class PublicationResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Key { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry PublicationPath { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry PublicationUrl { get; set; }
        [JsonProperty(Order = 4)]
        public TextEntry MultimediaPath { get; set; }
        [JsonProperty(Order = 5)]
        public TextEntry MultimediaUrl { get; set; }


        public static PublicationResult From(PublicationData item, string currentUserId)
        {
            var result = new PublicationResult
            {
                Key = TextEntry.From(item.Key, Resources.LabelKey),
                PublicationPath = TextEntry.From(item.PublicationPath, Resources.LabelPublicationPath),
                PublicationUrl = TextEntry.From(item.PublicationUrl, Resources.LabelPublicationUrl),
                MultimediaPath = TextEntry.From(item.MultimediaPath, Resources.LabelMultimediaPath),
                MultimediaUrl = TextEntry.From(item.MultimediaUrl, Resources.LabelMultimediaUrl),
                WebDavUrl = TextEntry.From(item.LocationInfo.WebDavUrl, Resources.LabelWebDavUrl)
            };

            AddCommonProperties(item, result);
            return result;
        }
    }
}