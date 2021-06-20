using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LocalizeMaster.Classes;
using LocalizeMaster.Helpers;
using MessageBox.Avalonia;
using Newtonsoft.Json;

namespace LocalizeMaster
{
    public partial class SettingsWindow : Window
    {
        public ObservableCollection<Term> Values { get; set; }
        public ObservableCollection<Language> Languages { get; set; }
        public ObservableCollection<Language> Cultures { get; set; }
        public Language? CurrentCulture { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();
            Cultures = new ObservableCollection<Language>(CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures)
                .Select(x=> new Language(x.EnglishName, x.Name)));

            CurrentCulture = Cultures.FirstOrDefault(x => x.CultureName == CultureInfo.CurrentCulture.Name);

            Languages = new ObservableCollection<Language>();
            Languages.CollectionChanged += Languages_CollectionChanged;
            Values = new ObservableCollection<Term>();
            Values.CollectionChanged += Values_CollectionChanged;
            this.DataContext = this;
        }

        private void Languages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (Values == null)
                        return;
                    foreach(var lng in e.NewItems?.OfType<Language>() ?? Enumerable.Empty<Language>())
                    {
                        lng.Items.Clear();
                        foreach (var term in Values)
                            lng.Items.Add(new LocalizedString(term, ""));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:

                    break;
            }
        }

        private void Values_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var term in e.NewItems?.OfType<Term>() ?? Enumerable.Empty<Term>())
                    {
                        term.PropertyChanged += Term_PropertyChanged;
                        foreach (var lng in Languages)
                        {
                            var str = new LocalizedString(term, "");
                            lng.Items.Add(str);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var term in e.OldItems?.OfType<Term>() ?? Enumerable.Empty<Term>())
                    {
                        term.PropertyChanged -= Term_PropertyChanged;
                        foreach (var lng in Languages)
                        {
                            var str = lng.Items.FirstOrDefault(x => x.Key == term);
                            if(str != null)
                                lng.Items.Remove(str);
                        }
                    }
                    break;
            }
        }

        private async void Term_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(Term.Value):
                    if (sender is Term term && this.Values.Any(x => x != term && x.Value == term.Value))
                    {
                        await MessageBoxManager.GetMessageBoxStandardWindow(
                            new MessageBox.Avalonia.DTO.MessageBoxStandardParams()
                            {
                                ContentMessage = $"Value '{term.Value}' is already exist!",
                                MinWidth = 300,
                                MinHeight = 100,
                                SizeToContent = SizeToContent.WidthAndHeight,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                            }).ShowDialog(this);
                    }
                    break;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void AddValue_Click(object sender, RoutedEventArgs e)
        {
            Values.Add(new Term(""));
        }

        private void AddLanguage_Click(object sender, RoutedEventArgs e)
        {
            var lng = (Language)(this.Get<ComboBox>("CulturesComboBox").SelectedItem)!;
            if (lng != null && !Languages.Contains(lng))
                Languages.Add(lng);
        }

        private void RemoveLanguage_Click(object sender, RoutedEventArgs e)
        {
            var lng = (Language)((sender as Button)!.Tag)!;
            this.Languages.Remove(lng);
        }

        private void RemoveValue_Click(object sender, RoutedEventArgs e)
        {
            var value = (sender as Button)!.Tag as Term;
            this.Values.Remove(value!);
        }

        private void LoadFiles(params string[] files)
        {
            this.Languages.Clear();
            this.Values.Clear();

            var list = new List<LocalizationFile>();
            foreach (var file in files)
            {
                var lf = new LocalizationFile() { Name = Path.GetFileNameWithoutExtension(file) };
                try
                {
                    lf.Values = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file))!;
                }
                catch {}
                list.Add(lf);
            }

            foreach (var value in list.SelectMany(x => x.Values.Keys).Distinct())
                this.Values.Add(new Term(value));

            foreach (var culture in list)
            {
                var lng = this.Cultures.FirstOrDefault(x => string.Equals(x.CultureName, culture.Name, System.StringComparison.CurrentCultureIgnoreCase))
                    ?? new Language("Custom", culture.Name);
                this.Languages.Add(lng);
            }

            foreach(var file in list)
            {
                var lng = this.Languages.First(x => string.Equals(x.CultureName, file.Name, System.StringComparison.CurrentCultureIgnoreCase));
                foreach(var value in file.Values)
                {
                    var str = lng.Items.FirstOrDefault(x => string.Equals(x.Key.Value, value.Key, System.StringComparison.CurrentCultureIgnoreCase));
                    if (str != null)
                        str.Value = value.Value;
                }
            }


        }

        private async void LoadFiles_Click(object sender, RoutedEventArgs e)
        {
            var files = await this.OpenFileDialog(filters: new FileDialogFilter() { Extensions = new List<string>() { "json", "JSON" }, Name = "json files" });

            if (files != null && files.Length > 0)
            {
                LoadFiles(files);
            }
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            var folder = await this.OpenFolderDialog();

            if (folder != null)
                foreach (var lng in this.Languages)
                    File.WriteAllText(Path.Combine(folder, $"{lng.CultureName}.json"),
                        JsonConvert.SerializeObject(lng.Items.GroupBy(x => x.Key.Value).ToDictionary(x => x.Key,x => x.First().Value),
                        Formatting.Indented));
        }

    }
}
