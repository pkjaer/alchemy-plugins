namespace Alchemy.Plugins.CountItems.Controllers
{
    public class ItemCountResult
    {
        public int Folders { get; set; }
        public int Components { get; set; }
        public int Schemas { get; set; }
        public int ComponentTemplates { get; set; }
        public int PageTemplates { get; set; }
        public int TemplateBuildingBlocks { get; set; }
        public int StructureGroups { get; set; }
        public int Pages { get; set; }
        public int Categories { get; set; }
        public int Keywords { get; set; }
        public int TimeTaken { get; set; }
    }
}