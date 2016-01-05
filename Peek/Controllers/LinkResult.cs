using Tridion.ContentManager.CoreService.Client;

namespace Alchemy.Plugins.Peek.Controllers
{
    public class LinkResult
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public static LinkResult From(Link link)
        {
            if (link != null && link.IdRef != "tcm:0-0-0")
            {
                return new LinkResult
                {
                    Id = link.IdRef,
                    Title = link.Title
                };
            }
            return null;
        }
    }
}