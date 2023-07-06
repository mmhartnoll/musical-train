namespace DomainModel.Exceptions
{
    public abstract class DomainException : Exception
    {
        public DomainException(string? message = null, Exception? innerException = null)
            : base(message, innerException) { }
    }
}