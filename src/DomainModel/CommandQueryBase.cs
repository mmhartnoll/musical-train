using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace DomainModel
{
    public abstract class CommandQueryBase : INotifyPropertyChanged, IDataErrorInfo
    {
        private string error = string.Empty;
        private readonly IDictionary<string, string> errors = new Dictionary<string, string>();

        public string Error
        {
            get => error;
            private set => SetProperty(ref error, value, nameof(Error));
        }

        public IReadOnlyDictionary<string, string> Errors { get; }

        public string this[string propertyName] => Validate(propertyName);

        public event PropertyChangedEventHandler? PropertyChanged;

        public CommandQueryBase()
            => Errors = new ReadOnlyDictionary<string, string>(errors);

        public bool TryValidate()
        {
            ValidationContext context = new(this);
            List<ValidationResult> validationResults = new();

            if (Validator.TryValidateObject(this, context, validationResults, true))
                return true;

            foreach (string memberName in validationResults.SelectMany(result => result.MemberNames))
                Validate(memberName);

            return false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        protected void SetProperty<T>(ref T field, T value, string propertyName, IEqualityComparer<T>? comparer = null)
        {
            if ((comparer ?? EqualityComparer<T>.Default).Equals(field, value))
                return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        private string Validate(string propertyName)
        {
            try
            {
                object? value = GetType()
                    .GetProperty(propertyName)!
                    .GetValue(this);
                ValidationContext context = new(this) { MemberName = propertyName };
                Validator.ValidateProperty(value, context);
                errors.Remove(propertyName);
                Error = string.Empty;
            }
            catch (ValidationException ex)
            {
                errors[propertyName] = ex.Message;
                Error = ex.Message;
            }
            finally
            {
                OnPropertyChanged(nameof(Errors));
            }
            return Error;
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new InvalidOperationException($"Invalid propery name '{propertyName}'.");
        }
    }
}