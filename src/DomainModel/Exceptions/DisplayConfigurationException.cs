namespace DomainModel.Exceptions
{
    public class DisplayConfigurationException : DomainException
    {
        public DisplayConfigurationException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}