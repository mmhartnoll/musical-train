using DomainModel.Events;

namespace DomainModel
{
    public abstract class DomainService
    {
        protected DomainEventHandler<DomainLogEvent> LogEventHandler { get; private init; }

        protected DomainService(DomainEventHandler<DomainLogEvent> logEventHandler)
            => LogEventHandler = logEventHandler;
    }
}