using System.Globalization;
using System.Linq;
using Alchemy.Plugins.Peek.Controllers.Entries;
using Newtonsoft.Json;
using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Results
{
    public class UserResult : PeekResult
    {
        [JsonProperty(Order = 1)]
        public TextEntry Description { get; set; }
        [JsonProperty(Order = 2)]
        public TextEntry Status { get; set; }
        [JsonProperty(Order = 3)]
        public TextEntry IsAdministrator { get; set; }
        [JsonProperty(Order = 4)]
        public TextEntry Language { get; set; }
        [JsonProperty(Order = 5)]
        public TextEntry Locale { get; set; }
        [JsonProperty(Order = 6)]
        public TextEntry GroupMemberships { get; set; }


        public static UserResult From(UserData item, ISessionAwareCoreService client)
        {
            var result = new UserResult
            {
                Description = TextEntry.From(item.Description, Resources.LabelDescription)
            };

            if (item.IsEnabled == false)
            {
                result.Status = TextEntry.From(Resources.Disabled, Resources.LabelStatus);
            }
            if (item.Privileges == 1)
            {
                result.IsAdministrator = TextEntry.From(Resources.Yes, Resources.LabelIsAdministrator);
            }
            if (item.LanguageId != null)
            {
                result.Language = TextEntry.From(GetLanguageById(item.LanguageId.Value, client), Resources.LabelLanguage);
            }
            if (item.LocaleId != null)
            {
                result.Locale = TextEntry.From(GetLocaleById(item.LocaleId.Value), Resources.LabelLocale);
            }
            if (item.GroupMemberships != null)
            {
                result.GroupMemberships = TextEntry.From(GetGroupMembershipSummary(item.GroupMemberships), Resources.LabelGroupMemberships);
            }

            AddCommonProperties(item, result);
            return result;
        }

        private static string GetLanguageById(int languageId, ISessionAwareCoreService client)
        {
            if (languageId > 0)
            {
                var languages = client.GetTridionLanguages();
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

        private static string GetLocaleById(int localeId)
        {
            var locale = CultureInfo.GetCultures(CultureTypes.SpecificCultures).FirstOrDefault(l => l.LCID == localeId);
            return locale != null ? locale.EnglishName : Resources.NotSet;
        }

    }
}