using Alchemy4Tridion.Plugins;
using System;
using System.Web.Http;
using System.Xml.Linq;
using System.Linq;
using Tridion.ContentManager.CoreService.Client;
using System.Collections.Generic;

namespace Alchemy.Plugins.CountItems.Controllers
{
    [AlchemyRoutePrefix("CountItemsService")]
    public class CountItemsController : AlchemyApiController
    {
        [HttpPost]
        [Route("GetCount")]
        public ItemCountResult GetCount(GetCountParameters parameters)
        {
            if (parameters == null || parameters.OrganizationalItemId == null)
            {
                throw new ArgumentNullException("parameters");
            }

            ItemsFilterData filter = GetFilter(parameters);
            XElement listXml = Client.GetListXml(parameters.OrganizationalItemId, filter);
            return ProcessCounts(listXml);
        }

        private static int CountItemsOfType(XElement listXml, int itemType)
        {
            if (listXml == null)
            {
                throw new ArgumentNullException("listXml");
            }

            XNamespace tcm = "http://www.tridion.com/ContentManager/5.0";
            var nodes = from item in listXml.Descendants(tcm + "Item")
                        let itemTypeAttr = item.Attribute("Type")
                        where itemTypeAttr != null && itemTypeAttr.Value.Equals(itemType.ToString())
                        select item;

            return nodes.Count();
        }

        /// <summary>
        /// Extract the actual counts from the XML.
        /// Set the response _countItemsData object.
        /// </summary>
        private ItemCountResult ProcessCounts(XElement listXml)
        {
            if (listXml == null)
            {
                throw new ArgumentNullException("listXml");
            }

            return new ItemCountResult
            {
                Folders = CountItemsOfType(listXml, 2),
                Components = CountItemsOfType(listXml, 16),
                Schemas = CountItemsOfType(listXml, 8),
                ComponentTemplates = CountItemsOfType(listXml, 32),
                PageTemplates = CountItemsOfType(listXml, 128),
                TemplateBuildingBlocks = CountItemsOfType(listXml, 2048),
                StructureGroups = CountItemsOfType(listXml, 4),
                Pages = CountItemsOfType(listXml, 64),
                Categories = CountItemsOfType(listXml, 512),
                Keywords = CountItemsOfType(listXml, 1024)
            };
        }

        /// <summary>
        /// Create an ItemsFilter for the given Organizational Item type by either instantiating a
        /// RepositoryItemsFilterData (for Publication) or OrganizationalItemItemsFilterData (for Folder or SG).
        /// Populates the ItemTypes property with only the types that were requested to improve performance.
        /// </summary>
        private static ItemsFilterData GetFilter(GetCountParameters parameters)
        {
            ItemsFilterData filter;
            if (parameters.OrganizationalItemId.EndsWith("-1")) // is Publication
            {
                filter = new RepositoryItemsFilterData();
            }
            else // is Folder or Structure Group
            {
                filter = new OrganizationalItemItemsFilterData();
            }

            filter.Recursive = true;

            List<ItemType> itemTypesList = new List<ItemType>();
            if (parameters.CountFolders) { itemTypesList.Add(ItemType.Folder); }
            if (parameters.CountComponents) { itemTypesList.Add(ItemType.Component); }
            if (parameters.CountSchemas) { itemTypesList.Add(ItemType.Schema); }
            if (parameters.CountComponentTemplates) { itemTypesList.Add(ItemType.ComponentTemplate); }
            if (parameters.CountPageTemplates) { itemTypesList.Add(ItemType.PageTemplate); }
            if (parameters.CountTemplateBuildingBlocks) { itemTypesList.Add(ItemType.TemplateBuildingBlock); }
            if (parameters.CountStructureGroups) { itemTypesList.Add(ItemType.StructureGroup); }
            if (parameters.CountPages) { itemTypesList.Add(ItemType.Page); }
            if (parameters.CountCategories) { itemTypesList.Add(ItemType.Category); }
            if (parameters.CountKeywords) { itemTypesList.Add(ItemType.Keyword); }

            filter.ItemTypes = itemTypesList.ToArray();

            return filter;
        }
    }
}