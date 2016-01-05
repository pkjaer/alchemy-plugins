using System;

namespace Alchemy.Plugins.Peek.Controllers
{
    public class PeekResults
    {
        public string Id { get; set; }
        public LinkResult LockedBy { get; set; }
        public string Directory { get; set; }
        public LinkResult Schema { get; set; }
        public LinkResult MetadataSchema { get; set; }
        public LinkResult DefaultPageTemplate { get; set; }
        public LinkResult LinkedSchema { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? RevisionDate { get; set; }
    }
}