using System.Collections.Generic;
using Newtonsoft.Json;

namespace LocalizeMaster.Classes
{
    public class Settings : BaseModel
    {
        private static readonly IReadOnlyList<string> _themes = new List<string>() { "Dark", "Light" };

        [JsonIgnore]
        public IReadOnlyList<string> Themes => _themes;

        private string _theme = "Dark";
        public string Theme
        {
            get => _theme;
            set => SetPropertyValue(nameof(Theme), ref _theme, value);
        }

        private bool _serializeEmptyTerms = true;
        public bool SerializeEmptyTerms
        {
            get => _serializeEmptyTerms;
            set => SetPropertyValue(nameof(SerializeEmptyTerms), ref _serializeEmptyTerms, value);
        }

        private bool _serializeEmptyValues = true;
        public bool SerializeEmptyValues
        {
            get => _serializeEmptyValues;
            set => SetPropertyValue(nameof(SerializeEmptyValues), ref _serializeEmptyValues, value);
        }

    }
}
