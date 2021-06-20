namespace LocalizeMaster.Classes
{
    public class LocalizedString : BaseModel
    {
        public LocalizedString(Term key, string value)
        {
            Key = key;
            _value = value;
        }

        public Term Key { get; set; }

        private string _value;
        public string Value
        {
            get => _value;
            set => SetPropertyValue(nameof(value), ref _value, value);
        }
    }
}
