using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class MultimediaTypeResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry MimeType { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry FileExtensions { get; set; }


        public static MultimediaTypeResult From(MultimediaTypeData item)
        {
            var result = new MultimediaTypeResult
            {
                MimeType = TextEntry.From(item.MimeType, Resources.LabelMimeType),
                FileExtensions = TextEntry.From(string.Join(", ", item.FileExtensions), Resources.LabelFileExtensions)
            };

            AddCommonProperties(item, result);
            return result;
        }
    }
}