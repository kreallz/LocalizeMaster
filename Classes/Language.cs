using System.Collections.ObjectModel;

namespace LocalizeMaster.Classes
{
    public class Language
    {
        public Language(string name, string cultureName)
        {
            Name = name;
            CultureName = cultureName;
            Items = new ObservableCollection<LocalizedString>();
        }

        public string CultureName { get; set; }

        public string Name { get; set; }

        public ObservableCollection<LocalizedString> Items { get; set; }

        public override string ToString()
        {
            return $"{CultureName} ({Name})";
        }

    }
}
