using System.Collections.ObjectModel;

namespace DomainModel.Exceptions
{
    public class KeyNotFoundException : DomainException
    {
        public IReadOnlyDictionary<string, object> FieldValues { get; }

        public KeyNotFoundException(string field, object value, string? message = null, Exception? innerException = null)
            : this(MapToDictionary(field, value), message, innerException) { }

        public KeyNotFoundException(IReadOnlyDictionary<string, object> fieldValues, string? message = null, Exception? innerException = null)
            : base(message, innerException)
        {
            FieldValues = fieldValues;
        }

        private static IReadOnlyDictionary<string, object> MapToDictionary(string field, object value)
            => new ReadOnlyDictionary<string, object>(new Dictionary<string, object>() { { field, value } });
    }
}