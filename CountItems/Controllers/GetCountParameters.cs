namespace Alchemy.Plugins.CountItems.Controllers
{
    public class GetCountParameters
    {
        public string OrganizationalItemId { get; set; }
        public bool CountFolders { get; set; }
        public bool CountComponents { get; set; }
        public bool CountSchemas { get; set; }
        public bool CountComponentTemplates { get; set; }
        public bool CountPageTemplates { get; set; }
        public bool CountTemplateBuildingBlocks { get; set; }
        public bool CountStructureGroups { get; set; }
        public bool CountPages { get; set; }
        public bool CountCategories { get; set; }
        public bool CountKeywords { get; set; }
    }
}