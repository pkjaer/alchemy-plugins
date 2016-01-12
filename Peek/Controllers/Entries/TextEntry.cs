namespace Alchemy.Plugins.Peek.Controllers.Entries
{
    public class TextEntry
    {
        public string Type { get { return "text"; } }
        public string Value { get; set; }
        public string Label { get; set; }

        public static TextEntry From(string value, string label)
        {
            return !string.IsNullOrWhiteSpace(value) ? new TextEntry { Value = value, Label = label } : null;
        }
    }
}