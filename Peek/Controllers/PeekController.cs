using System.Linq;
using Alchemy.Plugins.Peek.Controllers.Results;
using Alchemy4Tridion.Plugins;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers
{
    [AlchemyRoutePrefix("PeekService")]
    public class PeekController : AlchemyApiController
    {
        [HttpPost]
        [Route("Peek")]
        public PeekResult Peek(PeekParameters parameters)
        {
            try
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters");
                }

                if (string.IsNullOrWhiteSpace(parameters.ItemUri))
                {
                    throw new ArgumentException(Resources.MissingItemUri);
                }

                if (Client.IsExistingObject(parameters.ItemUri))
                {
                    var readOptions = new ReadOptions { LoadFlags = LoadFlags.Expanded | LoadFlags.WebDavUrls };
                    var item = Client.Read(parameters.ItemUri, readOptions);

                    switch (GetItemType(item.Id))
                    {
                        case ItemType.Category:
                            return CategoryResult.From((CategoryData)item, Client, CurrentUserId);
                        case ItemType.Component:
                            return ComponentResult.From((ComponentData)item, CurrentUserId);
                        case ItemType.ComponentTemplate:
                            return ComponentTemplateResult.From((ComponentTemplateData)item, Client, CurrentUserId);
                        case ItemType.Folder:
                            return FolderResult.From((FolderData)item, CurrentUserId);
                        case ItemType.Group:
                            return GroupResult.From((GroupData)item);
                        case ItemType.Keyword:
                            return KeywordResult.From((KeywordData)item, CurrentUserId);
                        case ItemType.MultimediaType:
                            return MultimediaTypeResult.From((MultimediaTypeData)item);
                        case ItemType.Page:
                            return PageResult.From((PageData)item, Client, CurrentUserId);
                        case ItemType.PageTemplate:
                            return PageTemplateResult.From((PageTemplateData)item, Client, CurrentUserId);
                        case ItemType.Publication:
                            return PublicationResult.From((PublicationData)item, CurrentUserId);
                        case ItemType.PublicationTarget:
                            return PublicationTargetResult.From((PublicationTargetData)item);
                        case ItemType.Schema:
                            return SchemaResult.From((SchemaData)item, CurrentUserId);
                        case ItemType.StructureGroup:
                            return StructureGroupResult.From((StructureGroupData)item, CurrentUserId);
                        case ItemType.TargetGroup:
                            return TargetGroupResult.From((TargetGroupData)item, CurrentUserId);
                        case ItemType.TargetType:
                            return TargetTypeResult.From((TargetTypeData)item);
                        case ItemType.TemplateBuildingBlock:
                            return TemplateBuildingBlockResult.From((TemplateBuildingBlockData)item, Client, CurrentUserId);
                        case ItemType.User:
                            return UserResult.From((UserData)item, Client);
                        case ItemType.VirtualFolder:
                            return VirtualFolderResult.From((VirtualFolderData)item, CurrentUserId);
                    }
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        private string CurrentUserId
        {
            get
            {
                return Client.GetCurrentUser().Id;
            }
        }

        private static ItemType GetItemType(string itemUri)
        {
            if (!string.IsNullOrWhiteSpace(itemUri) && itemUri.StartsWith("tcm:"))
            {
                var parts = itemUri.Substring(4).Split('-');
                if (parts.All(part => AsNumber(part) != null))
                {
                    switch (parts.Length)
                    {
                        case 2: return ItemType.Component;
                        case 3: return (ItemType)AsNumber(parts[2]).Value;
                    }
                }
            }

            return ItemType.None;
        }

        private static int? AsNumber(string text)
        {
            int result;
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            return null;
        }
    }
}