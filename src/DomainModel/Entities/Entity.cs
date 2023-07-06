using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DomainModel.Entities
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        protected void SetProperty<T>(ref T field, T value, string propertyName, IEqualityComparer<T>? comparer = null)
        {
            ValidateProperty(value, propertyName);
            if ((comparer ?? EqualityComparer<T>.Default).Equals(field, value))
                return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        private void ValidateProperty<T>(T value, string propertyName)
        {
            ValidationContext context = new(this) { MemberName = propertyName };
            Validator.ValidateProperty(value, context);
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new InvalidOperationException($"Invalid propery name '{propertyName}'.");
        }
    }
}