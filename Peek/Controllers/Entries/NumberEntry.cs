namespace Alchemy.Plugins.Peek.Controllers.Entries
{
    public class NumberEntry
    {
        public string Type { get { return "number"; } }
        public int? Value { get; set; }
        public string Label { get; set; }

        public static NumberEntry From(int? value, string label)
        {
            return value != null ? new NumberEntry { Value = value, Label = label } : null;
        }
    }
}