using System.Collections.ObjectModel;

namespace DomainModel.Exceptions
{
    public class UniqueKeyViolationException : DomainException
    {
        public IReadOnlyDictionary<string, object> FieldValues { get; }

        public UniqueKeyViolationException(string field, object value, string? message = null, Exception? innerException = null)
            : this(MapToDictionary(field, value), message, innerException) { }

        public UniqueKeyViolationException(IReadOnlyDictionary<string, object> fieldValues, string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
            FieldValues = fieldValues;
        }

        private static IReadOnlyDictionary<string, object> MapToDictionary(string field, object value)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            return new ReadOnlyDictionary<string, object>(
                new Dictionary<string, object>() { { field, value } });
        }
    }
}