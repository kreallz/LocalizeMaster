using System.Collections.Generic;
using System.ComponentModel;

namespace LocalizeMaster.Classes
{
    public abstract class BaseModel : INotifyPropertyChanged
    {
        public BaseModel()
        {

        }

        protected bool SetPropertyValue<T>(string propertyName, ref T property, T value)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return false;
            var oldValue = property;
            property = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #region реализация автообновления при изменении свойств
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
