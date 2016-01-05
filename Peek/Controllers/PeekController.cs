using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Alchemy4Tridion.Plugins;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers
{
    [AlchemyRoutePrefix("PeekService")]
    public class PeekController : AlchemyApiController
    {
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
                var readOptions = new ReadOptions { LoadFlags = LoadFlags.Expanded | LoadFlags.WebDavUrls | LoadFlags.IncludeDynamicVersionInfo | LoadFlags.KeywordXlinks};

                if (Client.IsExistingObject(parameters.ItemUri))
                {
                    var item = Client.Read(parameters.ItemUri, readOptions);
                    AddBasicInfo(item, result);
                    AddPropertiesForItemType(item, result);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        private static void AddBasicInfo(IdentifiableObjectData item, PeekResults result)
        {
            result.Id = item.Id;
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


            var rlo = item as RepositoryLocalObjectData;
            if (rlo != null)
            {
                result.LockedBy = LinkResult.From(rlo.LockInfo.LockUser);
                result.MetadataSchema = LinkResult.From(rlo.MetadataSchema);
            }

            var sg = item as StructureGroupData;
            if (sg != null)
            {
                result.Directory = string.IsNullOrWhiteSpace(sg.Directory) ? Resources.EmptyLabel : sg.Directory;
                result.DefaultPageTemplate = LinkResult.From(sg.DefaultPageTemplate);
            }

            var folder = item as FolderData;
            if (folder != null)
            {
                result.LinkedSchema = LinkResult.From(folder.LinkedSchema);
            }

            var component = item as ComponentData;
            if (component != null)
            {
                result.Schema = LinkResult.From(component.Schema);
            }

            /*
             * Possibly interesting item types:
             * 
             * Activity Definition
             * Activity History
             * Activity Instance
             * Category
             * Component
             * Component Template
             * Folder
             * Group
             * Keyword
             * Multimedia Type
             * Page
             * Page Template
             * Process Definition
             * Process History
             * Process Instance
             * Publication
             * Publication Target
             * Schema
             * Structure Group
             * Target Group
             * Target Type
             * Template Building Block
             * User
             * Virtual Folder
             * Work Item
             * Business Process Type
             * 
             * (Addon item types like Address Books and Distribution Lists; they cannot be loaded using the Core Service, though)
             */
        }
    }
}