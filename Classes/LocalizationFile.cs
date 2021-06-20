using System.Collections.Generic;

namespace LocalizeMaster.Classes
{
    public class LocalizationFile
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, string> Values = new Dictionary<string, string>();
    }
}
