using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers.Entries
{
    public class LinkEntry
    {
        public string Type { get { return "link"; } }
        public string Label { get; set; }
        public string LinkId { get; set; }
        public string LinkTitle { get; set; }

        public static LinkEntry From(Link link, string label, string currentUserId)
        {
            if (link != null && link.IdRef != "tcm:0-0-0")
            {
                return new LinkEntry
                {
                    LinkId = link.IdRef,
                    LinkTitle = link.IdRef == currentUserId ? Resources.You : link.Title,
                    Label = label
                };
            }
            return null;
        }
    }
}