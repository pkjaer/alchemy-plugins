using System;

namespace Alchemy.Plugins.Peek.Controllers.Entries
{
    public class DateEntry
    {
        public string Type { get { return "date"; } }
        public DateTime? Value { get; set; }
        public string Label { get; set; }
        public string User { get; set; }

        public static DateEntry From(DateTime? value, string label)
        {
            return value != null ? new DateEntry { Value = value, Label = label } : null;
        }
    }
}