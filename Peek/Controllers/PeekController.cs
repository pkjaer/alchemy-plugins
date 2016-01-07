using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using Alchemy4Tridion.Plugins;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers
{
    [AlchemyRoutePrefix("PeekService")]
    public class PeekController : AlchemyApiController
    {
        protected static XNamespace schemaNamespace = @"http://www.w3.org/2001/XMLSchema";
        protected static XName schemaFields = schemaNamespace + "element";



        [HttpPost]
        [Route("Peek")]
        public PeekResults Peek(PeekParameters parameters)
        {
            try
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                if (string.IsNullOrWhiteSpace(parameters.ItemUri))
                {
                    throw new ArgumentNullException("ItemUri");
                }

                var result = new PeekResults();

                if (Client.IsExistingObject(parameters.ItemUri))
                {
                    var readOptions = new ReadOptions { LoadFlags = LoadFlags.Expanded | LoadFlags.WebDavUrls | LoadFlags.IncludeDynamicVersionInfo | LoadFlags.KeywordXlinks };
                    var item = Client.Read(parameters.ItemUri, readOptions);
                    AddCommonProperties(item, result);
                    AddPropertiesForItemType(item, result);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        private static void AddCommonProperties(IdentifiableObjectData item, PeekResults result)
        {
            if (item.VersionInfo == null) return;

            if (item.VersionInfo.CreationDate.HasValue)
            {
                result.CreationDate = item.VersionInfo.CreationDate.Value;
            }
            if (item.VersionInfo.RevisionDate.HasValue && item.VersionInfo.RevisionDate.Value != result.CreationDate)
            {
                result.RevisionDate = item.VersionInfo.RevisionDate.Value;
            }
        }

        private void AddPropertiesForItemType(IdentifiableObjectData item, PeekResults result)
        {
            AddPropertiesForRepositoryLocalObject(item as RepositoryLocalObjectData, result);
            AddPropertiesForSchema(item as SchemaData, result);
            AddPropertiesForComponent(item as ComponentData, result);
            AddPropertiesForComponentTemplate(item as ComponentTemplateData, result);
            AddPropertiesForPage(item as PageData, result);
            AddPropertiesForPageTemplate(item as PageTemplateData, result);
            AddPropertiesForTemplateBuildingBlock(item as TemplateBuildingBlockData, result);
            AddPropertiesForStructureGroup(item as StructureGroupData, result);
            AddPropertiesForFolder(item as FolderData, result);
            AddPropertiesForVirtualFolder(item as VirtualFolderData, result);
            AddPropertiesForPublication(item as PublicationData, result);
            AddPropertiesForCategory(item as CategoryData, result);
            AddPropertiesForKeyword(item as KeywordData, result);
            AddPropertiesForTargetGroup(item as TargetGroupData, result);
            AddPropertiesForMultimediaType(item as MultimediaTypeData, result);
            AddPropertiesForUser(item as UserData, result);
            AddPropertiesForGroup(item as GroupData, result);
        }

        private void AddPropertiesForRepositoryLocalObject(RepositoryLocalObjectData rlo, PeekResults result)
        {
            if (rlo == null) return;

            result.LockedBy = GetLockInfo(rlo);
            result.MetadataSchema = LinkResult.From(rlo.MetadataSchema);
            if (rlo.LocationInfo != null)
            {
                result.WebDavUrl = rlo.LocationInfo.WebDavUrl;
            }

            FullVersionInfo versionInfo = rlo.VersionInfo as FullVersionInfo;
            if (versionInfo != null)
            {
                result.Creator = versionInfo.Creator.Title;
                if (result.RevisionDate != null)
                {
                    result.Revisor = versionInfo.Revisor.Title;
                }

                if (versionInfo.LastVersion != null && versionInfo.LastVersion.Value > 1)
                {
                    result.Versions = versionInfo.LastVersion.Value;
                }
            }
        }

        private static void AddPropertiesForSchema(SchemaData schema, PeekResults result)
        {
            if (schema == null) return;

            result.Description = schema.Description;
            result.Namespace = schema.NamespaceUri;
            result.RootElementName = schema.RootElementName;
            AddFieldsSummary(schema, result);
        }

        private static void AddPropertiesForComponent(ComponentData component, PeekResults result)
        {
            if (component == null) return;

            result.Schema = LinkResult.From(component.Schema);

            if (component.ComponentType == ComponentType.Multimedia)
            {
                if (component.BinaryContent.FileSize != null)
                {
                    result.FileSize = FormatFileSize((long)component.BinaryContent.FileSize);
                }
                result.FileName = component.BinaryContent.Filename;
            }
        }

        private void AddPropertiesForComponentTemplate(ComponentTemplateData componentTemplate, PeekResults result)
        {
            if (componentTemplate == null) return;

            result.TemplateType = LookUpTemplateType(componentTemplate.TemplateType, ItemType.ComponentTemplate);

            if (componentTemplate.IsRepositoryPublishable == true)
            {
                result.DynamicTemplateInfo = Resources.DynamicTemplateNotAllowedOnPage;
                if (componentTemplate.AllowOnPage == true)
                {
                    result.DynamicTemplateInfo = Resources.DynamicTemplateAllowedOnPage;
                }
            }

            if (componentTemplate.Priority != null)
            {
                string priority = Resources.PriorityNeverLink;
                switch (componentTemplate.Priority.Value)
                {
                    case 300: priority = Resources.PriorityHigh; break;
                    case 200: priority = Resources.PriorityMedium; break;
                    case 100: priority = Resources.PriorityLow; break;
                }
                result.Priority = priority;
            }
        }

        private static void AddPropertiesForPage(PageData page, PeekResults result)
        {
            if (page == null) return;

            result.Template = LinkResult.From(page.PageTemplate);
            result.FileName = page.FileName;
            result.ComponentPresentations = Resources.None;

            if (page.ComponentPresentations.Any())
            {
                int count = page.ComponentPresentations.Count();
                int templateCount = page.ComponentPresentations.DistinctBy(cp => cp.ComponentTemplate.IdRef).Count();
                result.ComponentPresentations = (templateCount == 1)
                    ? FormatInvariant(Resources.ComponentPresentationSummarySameTemplate, count)
                    : FormatInvariant(Resources.ComponentPresentationSummary, count, templateCount);
            }
        }

        private void AddPropertiesForPageTemplate(PageTemplateData pageTemplate, PeekResults result)
        {
            if (pageTemplate == null) return;

            result.Extension = pageTemplate.FileExtension;
            result.TemplateType = LookUpTemplateType(pageTemplate.TemplateType, ItemType.PageTemplate);
        }

        private void AddPropertiesForTemplateBuildingBlock(TemplateBuildingBlockData templateBuildingBlock, PeekResults result)
        {
            if (templateBuildingBlock == null) return;

            result.MetadataSchema = LinkResult.From(templateBuildingBlock.MetadataSchema);
            result.ParametersSchema = LinkResult.From(templateBuildingBlock.ParameterSchema);
            result.TemplateType = LookUpTemplateType(templateBuildingBlock.TemplateType, ItemType.TemplateBuildingBlock);
        }

        private static void AddPropertiesForStructureGroup(StructureGroupData structureGroup, PeekResults result)
        {
            if (structureGroup == null) return;

            result.Directory = string.IsNullOrWhiteSpace(structureGroup.Directory) ? Resources.EmptyLabel : structureGroup.Directory;
            result.Template = LinkResult.From(structureGroup.DefaultPageTemplate);

            var publishInfo = structureGroup.LocationInfo as PublishLocationInfo;
            if (publishInfo != null)
            {
                result.FullDirectory = publishInfo.PublishLocationPath;
            }

            if (structureGroup.IsActiveResolvedValue == false)
            {
                result.Publishable = Resources.CannotBePublished;
                if (structureGroup.IsActive == true)
                {
                    result.Publishable = Resources.CannotBePublishedDueToAncestor;
                }
            }
        }

        private static void AddPropertiesForFolder(FolderData folder, PeekResults result)
        {
            if (folder == null) return;

            result.LinkedSchema = LinkResult.From(folder.LinkedSchema);
        }

        private static void AddPropertiesForVirtualFolder(VirtualFolderData virtualFolder, PeekResults result)
        {
            if (virtualFolder == null) return;

            result.Description = virtualFolder.Description;
        }

        private static void AddPropertiesForPublication(PublicationData publication, PeekResults result)
        {
            if (publication == null) return;

            result.Key = publication.Key;
            result.PublicationPath = publication.PublicationPath;
            result.PublicationUrl = publication.PublicationUrl;
            result.MultimediaPath = publication.MultimediaPath;
            result.MultimediaUrl = publication.MultimediaUrl;
            result.WebDavUrl = publication.LocationInfo.WebDavUrl;
        }

        private static void AddPropertiesForCategory(CategoryData category, PeekResults result)
        {
            if (category == null) return;

            result.Description = category.Description;
            result.XmlName = category.XmlName;

            if (category.UseForNavigation == false)
            {
                result.Publishable = Resources.CannotBePublished;
            }

            if (category.UseForIdentification == true)
            {
                result.UseForIdentification = Resources.Yes;
            }

            result.LinkedSchema = LinkResult.From(category.KeywordMetadataSchema);
        }

        private static void AddPropertiesForKeyword(KeywordData keyword, PeekResults result)
        {
            if (keyword == null) return;

            result.Description = keyword.Description;
            result.Key = keyword.Key;

            if (keyword.IsAbstract == true)
            {
                result.IsAbstract = Resources.Yes;
            }
        }

        private static void AddPropertiesForTargetGroup(TargetGroupData targetGroup, PeekResults result)
        {
            if (targetGroup == null) return;

            result.Description = targetGroup.Description;
            result.MetadataSchema = LinkResult.From(targetGroup.MetadataSchema);
            int count = targetGroup.Conditions.Count();
            result.Conditions = count > 0 ? count.ToString(CultureInfo.InvariantCulture) : Resources.None;
        }

        private static void AddPropertiesForMultimediaType(MultimediaTypeData multimediaType, PeekResults result)
        {
            if (multimediaType == null) return;

            result.MimeType = multimediaType.MimeType;
            result.FileExtensions = string.Join(", ", multimediaType.FileExtensions);
        }

        private void AddPropertiesForUser(UserData user, PeekResults result)
        {
            if (user == null) return;
            if (user.IsEnabled == false)
            {
                result.Status = Resources.Disabled;
            }
            result.Description = user.Description;
            if (user.Privileges == 1)
            {
                result.IsAdministrator = Resources.Yes;
            }
            if (user.LanguageId != null)
            {
                result.Language = GetLanguageById(user.LanguageId.Value);
            }
            if (user.LocaleId != null)
            {
                result.Locale = GetLocaleById(user.LocaleId.Value);
            }
            if (user.GroupMemberships != null)
            {
                result.GroupMemberships = GetGroupMembershipSummary(user.GroupMemberships);
            }
        }

        private void AddPropertiesForGroup(GroupData group, PeekResults result)
        {
            if (group == null) return;
            result.Description = @group.Description;

            if (group.GroupMemberships != null)
            {
                result.GroupMemberships = GetGroupMembershipSummary(group.GroupMemberships);
            }

            result.Scope = group.Scope.Length > 0
                ? Resources.GroupScopeSpecificPublications
                : Resources.GroupScopeAllPublications;
        }

        private string GetGroupMembershipSummary(IEnumerable<GroupMembershipData> groupMemberships)
        {
            IEnumerable<string> result = groupMemberships.Select(membership => 
                membership.Scope.Length > 0
                    ? FormatInvariant(Resources.GroupMembershipSummaryScoped, membership.Group.Title) 
                    : membership.Group.Title
                );

            return string.Join(", ", result);
        }

        private string GetLocaleById(int localeId)
        {
            var locale = CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(l => l.LCID == localeId);
            return locale != null ? locale.EnglishName : Resources.NotSet;
        }

        private string GetLanguageById(int languageId)
        {
            if (languageId > 0)
            {
                var languages = Client.GetTridionLanguages();
                if (languages != null)
                {
                    var language = languages.FirstOrDefault(l => l.LanguageId == languageId);
                    if (language != null)
                    {
                        return language.NativeName;
                    }
                }
            }

            return Resources.NotSet;
        }

        #region Helper methods

        private LinkResult GetLockInfo(RepositoryLocalObjectData rlo)
        {
            if (rlo.LockInfo == null) return null;

            var result = LinkResult.From(rlo.LockInfo.LockUser);
            if (result != null)
            {
                if (result.Id == Client.GetCurrentUser().Id)
                {
                    result.Title = Resources.You;
                }
            }

            return result;
        }

        private string LookUpTemplateType(string templateType, ItemType itemType)
        {
            var templateTypes = Client.GetListTemplateTypes(itemType);
            var found = templateTypes != null ? templateTypes.FirstOrDefault(t => t.Name == templateType) : null;
            return found != null ? found.Title : templateType;
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
                return FormatInvariant(Resources.FileSizeInBytes, size);
            }

            readable = (readable / 1024);
            return FormatInvariant(resource, readable.ToString("0.##"));
        }

        private static void AddFieldsSummary(SchemaData schema, PeekResults result)
        {
            try
            {
                XDocument doc = XDocument.Parse(schema.Xsd);
                const string nameAttr = "name";

                if (!string.IsNullOrWhiteSpace(schema.RootElementName))
                {
                    var rootElement = doc.Descendants(schemaFields).FirstOrDefault(e => e.Attribute(nameAttr) != null && e.Attribute(nameAttr).Value == schema.RootElementName);
                    result.FieldsSummary = GetFieldSummary(rootElement, Resources.None);
                }

                bool expectFields = (schema.Purpose == SchemaPurpose.Metadata ||
                                     schema.Purpose == SchemaPurpose.Multimedia ||
                                     schema.Purpose == SchemaPurpose.Bundle);

                var metadataRoot = doc.Descendants(schemaFields).FirstOrDefault(e => e.Attribute(nameAttr) != null && e.Attribute(nameAttr).Value == "Metadata");
                result.MetadataFieldsSummary = GetFieldSummary(metadataRoot, expectFields ? Resources.None : null);
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
                    ? FormatInvariant(Resources.SchemaFieldsSummaryAllOptional, count)
                    : countOptional < 1
                        ? FormatInvariant(Resources.SchemaFieldsSummaryAllMandatory, count)
                        : FormatInvariant(Resources.SchemaFieldsSummary, count, countOptional);
            }

            return defaultResult;
        }

        private static string FormatInvariant(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        #endregion
    }
}