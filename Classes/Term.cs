using System;
namespace LocalizeMaster.Classes
{
    public class Term : BaseModel
    {
        public Term(string term)
        {
            Id = Guid.NewGuid();
            _value = term;
        }

        public Guid Id { get; }

        private string _value;
        public string Value
        {
            get => _value;
            set => SetPropertyValue(nameof(Value), ref _value, value);
        }
    }
}
