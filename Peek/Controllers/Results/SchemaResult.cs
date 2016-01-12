using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class SchemaResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry Namespace { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry RootElementName { get; set; }
        [JsonProperty(Order = 4)]
        public TextEntry FieldsSummary { get; set; }
        [JsonProperty(Order = 5)]
        public TextEntry MetadataFieldsSummary { get; set; }

        public static SchemaResult From(SchemaData item, string currentUserId)
        {
            var result = new SchemaResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription),
                Namespace = TextEntry.From(item.NamespaceUri, Resources.LabelNamespace),
                RootElementName = TextEntry.From(item.RootElementName, Resources.LabelRootElementName)
            };

            AddFieldsSummary(item, result);
            AddCommonProperties(item, result);
            AddPropertiesForRepositoryLocalObject(item, result, currentUserId);
            return result;
        }

        private static void AddFieldsSummary(SchemaData schema, SchemaResult result)
        {
            try
            {
                XDocument doc = XDocument.Parse(schema.Xsd);
                const string nameAttr = "name";

                if (!string.IsNullOrWhiteSpace(schema.RootElementName))
                {
                    var rootElement = doc.Descendants(schemaFields).FirstOrDefault(e => e.Attribute(nameAttr) != null && e.Attribute(nameAttr).Value == schema.RootElementName);
                    result.FieldsSummary = TextEntry.From(GetFieldSummary(rootElement, Resources.None), Resources.LabelFieldsSummary);
                }

                bool expectFields = (schema.Purpose == SchemaPurpose.Metadata ||
                                     schema.Purpose == SchemaPurpose.Multimedia ||
                                     schema.Purpose == SchemaPurpose.Bundle);

                var metadataRoot = doc.Descendants(schemaFields).FirstOrDefault(e => e.Attribute(nameAttr) != null && e.Attribute(nameAttr).Value == "Metadata");
                result.MetadataFieldsSummary = TextEntry.From(GetFieldSummary(metadataRoot, expectFields ? Resources.None : null), Resources.LabelMetadataFieldsSummary);
            }
            catch (System.Xml.XmlException)
            {
            }
        }

        private static string GetFieldSummary(XContainer rootElement, string defaultResult = null)
        {
            if (rootElement != null)
            {
                XName minOccurs = "minOccurs";

                var fields = rootElement.Descendants(schemaFields).ToArray();
                int count = fields.Count();
                int countOptional = fields.Count(e => e.Attribute(minOccurs) == null || e.Attribute(minOccurs).Value == "0");

                if (count < 1)
                {
                    return Resources.None;
                }

                return countOptional == count
                    ? string.Format(CultureInfo.InvariantCulture, Resources.SchemaFieldsSummaryAllOptional, count)
                    : countOptional < 1
                        ? string.Format(CultureInfo.InvariantCulture, Resources.SchemaFieldsSummaryAllMandatory, count)
                        : string.Format(CultureInfo.InvariantCulture, Resources.SchemaFieldsSummary, count, countOptional);
            }

            return defaultResult;
        }
    }
}