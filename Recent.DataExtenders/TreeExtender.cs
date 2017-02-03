using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Tridion.ContentManager.CoreService.Client;
using Tridion.Web.UI.Core.Extensibility;

namespace Alchemy.Plugins.Recent.DataExtenders
{
    public class TreeExtender : DataExtender
    {
        protected SessionAwareCoreServiceClient Client;

        private const string ApplicationId = @"Alchemy.Plugins.RecentItems";
        private string _userId;
        protected string CurrentUserId => _userId ?? (_userId = Client.GetCurrentUser().Id);

        protected class RecentItemSummary
        {
            public string Id;
            public string Title;
            public string Icon;
        }

        private List<RecentItemSummary> _recentItems;

        public override XmlTextReader ProcessRequest(XmlTextReader reader, PipelineContext context)
        {
            return reader;
        }

        public override XmlTextReader ProcessResponse(XmlTextReader reader, PipelineContext context)
        {
            string command = (string) context.Parameters["command"];
            if (command != "GetListFavorites") return reader;

            Client = new SessionAwareCoreServiceClient("netSamlTcp_201603");
            Client.Impersonate(Tridion.Web.UI.Core.Utils.GetUserName());

            Load();

            var doc = XDocument.Load(reader);
            AddRecentItemsList(doc);
            return GetReaderFromDocument(doc);
        }

        private void AddRecentItemsList(XDocument doc)
        {
            XNamespace tcm = "http://www.tridion.com/ContentManager/5.0";
            var rootElem = new XElement(tcm + "Item");
            rootElem.Add(new XAttribute(@"ID", "cme:recentitems"));
            rootElem.Add(new XAttribute(@"Icon", "my-favorites")); // TODO: use the right icon
            rootElem.Add(new XAttribute(@"Title", @"Recent items")); // TODO: use a string resource

            var itemsElem = new XElement(tcm + "Items");

            var items = _recentItems; //GetItems();
            foreach (var item in items)
            {
                var elem = new XElement(tcm + "Item");
                elem.Add(new XAttribute(@"ID", item.Id));
                elem.Add(new XAttribute(@"Title", item.Title));
                elem.Add(new XAttribute(@"Icon", item.Icon));
                elem.Add(new XAttribute(@"HasChildren", "false"));
                itemsElem.Add(elem);
            }

            rootElem.Add(itemsElem);
            doc.Root.Add(rootElem);
        }

        public override string Name => @"Recent Items";


        /*
        protected List<RecentItemSummary> GetItems()
        {
            var result = _recentItems;
            if (result.Count <= 0) return result;

            var itemIds = result.Select(item => item.Id).ToList<string>();
            var lookupResults = Client.GetSystemWideList(new RepositoryLocalObjectsFilterData { ItemIds = itemIds.ToArray() });

            for (int i = 0; i < result.Count; i++)
            {
                result[i].Title = lookupResults[i].Title;
            }

            return result;
        }
        */

        private void Load()
        {
            var appData = Client.ReadApplicationData(CurrentUserId, ApplicationId);
            var serialized = appData?.GetAs<string>();
            Deserialize(serialized);
        }

        protected void Deserialize(string serialized)
        {
            _recentItems = new List<RecentItemSummary>();
            if (string.IsNullOrWhiteSpace(serialized)) return;

            string[] itemDataParts = serialized.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemData in itemDataParts)
            {
                var parts = itemData.Split(new[] {'^'}, StringSplitOptions.RemoveEmptyEntries);
                _recentItems.Add(new RecentItemSummary { Id = parts[0], Title = parts[1], Icon = parts[2] });
            }
        }

        private static XmlTextReader GetReaderFromDocument(XNode document)
        {
            if (document == null) return null;

            // Write it to a XmlTextWriter
            var memoryStream = new MemoryStream();
            var xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            document.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();

            // Read it into a XmlTextReader (that we need to return).
            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new XmlTextReader(memoryStream);
            reader.MoveToContent();
            return reader;
        }
    }
}