using System.Text;

namespace DomainModel.Exceptions
{
    public class InvalidCommandQueryException : DomainException
    {
        public Type CommandQueryType { get; }
        public IReadOnlyDictionary<string, string> FieldErrors { get; }

        public InvalidCommandQueryException(Type commandQueryType, IReadOnlyDictionary<string, string> fieldErrors, string? message = null, Exception? innerException = null)
            : base(message ?? GetDefaultMessage(commandQueryType, fieldErrors), innerException)
        {
            CommandQueryType = commandQueryType;
            FieldErrors = fieldErrors;
        }

        private static string GetDefaultMessage(Type commandQueryType, IReadOnlyDictionary<string, string> fieldErrors)
        {
            StringBuilder sb = new();
            sb.AppendLine($"Invalid {commandQueryType.Name}:");
            foreach (var item in fieldErrors)
                sb.AppendLine($"{item.Key}: {item.Value}");
            return sb.ToString();
        }
    }
}