using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public abstract class PeekResult
    {
        protected static XNamespace schemaNamespace = @"http://www.w3.org/2001/XMLSchema";
        protected static XName schemaFields = schemaNamespace + "element";

        [JsonProperty(Order = 0, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public LinkEntry LockedBy { get; set; }

        // Fields specific to the item type will end up in the middle here

        [JsonProperty(Order = 10, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public LinkEntry MetadataSchema { get; set; }
        [JsonProperty(Order = 11, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public NumberEntry Versions { get; set; }
        [JsonProperty(Order = 12, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateEntry CreationDate { get; set; }
        [JsonProperty(Order = 13, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateEntry RevisionDate { get; set; }
        
        // PathOnWebsite does here

        [JsonProperty(Order = 15, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TextEntry WebDavUrl { get; set; }


        protected static void AddCommonProperties(IdentifiableObjectData item, PeekResult result)
        {
            if (item.VersionInfo == null) return;

            result.CreationDate = DateEntry.From(item.VersionInfo.CreationDate, Resources.LabelCreationDate);

            if (item.VersionInfo.RevisionDate.HasValue && item.VersionInfo.RevisionDate.Value != result.CreationDate.Value)
            {
                result.RevisionDate = DateEntry.From(item.VersionInfo.CreationDate, Resources.LabelRevisionDate);
            }
        }

        protected static void AddPropertiesForRepositoryLocalObject(RepositoryLocalObjectData rlo, PeekResult result, string currentUserId)
        {
            if (rlo == null) return;

            result.LockedBy = LinkEntry.From(rlo.LockInfo.LockUser, Resources.LabelLockedBy, currentUserId);
            result.MetadataSchema = LinkEntry.From(rlo.MetadataSchema, Resources.LabelMetadataSchema, currentUserId);

            if (rlo.LocationInfo != null)
            {
                result.WebDavUrl = TextEntry.From(rlo.LocationInfo.WebDavUrl, Resources.LabelWebDavUrl);
            }

            FullVersionInfo versionInfo = rlo.VersionInfo as FullVersionInfo;
            if (versionInfo != null)
            {
                if (result.CreationDate != null)
                {
                    result.CreationDate.User = GetUserTitle(versionInfo.Creator, currentUserId);
                }
                
                if (result.RevisionDate != null)
                {
                    result.RevisionDate.User = GetUserTitle(versionInfo.Revisor, currentUserId);
                }

                if (versionInfo.LastVersion != null && versionInfo.LastVersion.Value > 1)
                {
                    result.Versions = NumberEntry.From(versionInfo.LastVersion.Value, Resources.LabelVersions);
                }
            }
        }

        protected static string GetGroupMembershipSummary(IEnumerable<GroupMembershipData> groupMemberships)
        {
            IEnumerable<string> result = groupMemberships.Select(membership =>
                membership.Scope.Length > 0
                    ? string.Format(CultureInfo.InvariantCulture, Resources.GroupMembershipSummaryScoped, membership.Group.Title)
                    : membership.Group.Title
                );

            return string.Join(", ", result);
        }

        protected static TextEntry GetPublishPath(PublishLocationInfo publishInfo, string appendText)
        {
            if (publishInfo != null)
            {
                string path = publishInfo.PublishPath.Replace(@"\", "/");
                if (!string.IsNullOrWhiteSpace(appendText))
                {
                    path = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", path.TrimEnd('/'), appendText);
                }
                return TextEntry.From(path, Resources.LabelPathOnWebsite);
            }

            return null;
        }

        protected static string GetUserTitle(LinkToUserData user, string currentUserId)
        {
            return user.IdRef == currentUserId ? Resources.You.ToLowerInvariant() : user.Title;
        }

        protected static string LookUpTemplateType(string templateType, ItemType itemType, ISessionAwareCoreService client)
        {
            var templateTypes = client.GetListTemplateTypes(itemType);
            var found = templateTypes != null ? templateTypes.FirstOrDefault(t => t.Name == templateType) : null;
            return found != null ? found.Title : templateType;
        }
    }
}