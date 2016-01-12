using System.Globalization;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class ComponentResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry FileName { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry FileSize { get; set; }
        [JsonProperty(Order = 3)]
        public LinkEntry Schema { get; set; }


        public static ComponentResult From(ComponentData item, string currentUserId)
        {
            var result = new ComponentResult { Schema = LinkEntry.From(item.Schema, Resources.LabelSchema, currentUserId) };

            if (item.ComponentType == ComponentType.Multimedia)
            {
                if (item.BinaryContent.FileSize != null)
                {
                    result.FileSize = TextEntry.From(FormatFileSize((long)item.BinaryContent.FileSize), Resources.LabelFileSize);
                }
                result.FileName = TextEntry.From(item.BinaryContent.Filename, Resources.LabelFileName);
            }

            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }

        private static string FormatFileSize(long size)
        {
            long absoluteValue = (size < 0 ? -size : size);
            string resource;
            double readable;

            if (absoluteValue >= 0x40000000)
            {
                resource = Resources.FileSizeInGigaBytes;
                readable = (size >> 20);
            }
            else if (absoluteValue >= 0x100000)
            {
                resource = Resources.FileSizeInMegaBytes;
                readable = (size >> 10);
            }
            else if (absoluteValue >= 0x400)
            {
                resource = Resources.FileSizeInKiloBytes;
                readable = size;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, Resources.FileSizeInBytes, size);
            }

            readable = (readable / 1024);
            return string.Format(CultureInfo.InvariantCulture, resource, readable.ToString("0.##"));
        }


    }
}