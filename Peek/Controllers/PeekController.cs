using System;
using System.Globalization;
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
                    result.Id = item.Id;
                    AddPropertiesForItemType(item, result);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        private void AddPropertiesForItemType(IdentifiableObjectData item, PeekResults result)
        {
            var rlo = item as RepositoryLocalObjectData;
            if (rlo != null)
            {
                result.Path = rlo.LocationInfo.Path;

                if (rlo.MetadataSchema.IdRef != "tcm:0-0-0")
                {
                    result.MetadataSchema = FormatInvariant("{0} ({1})", rlo.MetadataSchema.Title, rlo.MetadataSchema.IdRef);
                }
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

        private static string FormatInvariant(string format, params object[] arguments)
        {
            return string.Format(CultureInfo.InvariantCulture, format, arguments);
        }
    }
}