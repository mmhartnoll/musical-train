using DomainModel.Enumerations;

namespace DomainModel.Events
{
    public class DomainLogEvent : DomainEvent
    {
        public DateTime TimeStamp { get; private init; }
        public LogLevel Level { get; private init; }
        public string Message { get; private init; }

        public DomainLogEvent(LogLevel level, string message)
        {
            TimeStamp = DateTime.Now;
            Level = level;
            Message = message;
        }
    }
}