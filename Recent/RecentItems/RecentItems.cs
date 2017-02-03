using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy4Tridion.Plugins.Clients;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Recent
{
    public class RecentItems
    {
        private const string ApplicationId = @"Alchemy.Plugins.RecentItems";
        private const int MaxItemsToKeep = 10;

        private List<RecentItem> _recentItems;
        private string _userId;

        protected AlchemySessionAwareCoreServiceClient Client;
        protected string CurrentUserId => _userId ?? (_userId = Client.GetCurrentUser().Id);

        public RecentItems(AlchemySessionAwareCoreServiceClient client)
        {
            Client = client;
            Load();
        }

        public void Add(RecentItem item)
        {
            RemoveExistingEntry(item);

            if (_recentItems.Count >= MaxItemsToKeep)
            {
                _recentItems.RemoveRange(MaxItemsToKeep, (_recentItems.Count - 1) - MaxItemsToKeep);
            }

            _recentItems.Insert(0, item);
            Save();
        }

        public List<RecentItem> GetItems()
        {
            var result = _recentItems;
            /*if (result.Count <= 0) return result;

            var itemIds = result.Select(item => item.Id).ToList<string>();
            var lookupResults = Client.GetSystemWideList(new RepositoryLocalObjectsFilterData { ItemIds = itemIds.ToArray() });

            for (int i = 0; i < result.Count; i++)
            {
                result[i].Title = lookupResults[i].Title;
            }
            */
            return result;
        }

        private void RemoveExistingEntry(RecentItem item)
        {
            _recentItems.RemoveAll(entry => entry.Id == item.Id);
        }

        private void Load()
        {
            var appData = Client.ReadApplicationData(CurrentUserId, ApplicationId);
            var serialized = appData?.GetAs<string>();
            Deserialize(serialized);
        }

        private void Save()
        {
            var ada = new ApplicationDataAdapter(ApplicationId, Serialize());
            Client.SaveApplicationData(CurrentUserId, new[] { ada.ApplicationData });
        }


        protected string Serialize()
        {
            var result = _recentItems.Select(item => item.Id + "^" + item.Title + "^" + item.Icon).ToList();
            return string.Join("|", result);
        }

        protected void Deserialize(string serialized)
        {
            _recentItems = new List<RecentItem>();
            if (string.IsNullOrWhiteSpace(serialized)) return;

            string[] itemDataParts = serialized.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string itemData in itemDataParts)
            {
                var parts = itemData.Split(new[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
                _recentItems.Add(new RecentItem { Id = parts[0], Title = parts[1], Icon = parts[2] });
            }
        }
    }
}