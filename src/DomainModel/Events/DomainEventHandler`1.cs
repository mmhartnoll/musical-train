namespace DomainModel.Events
{
    public class DomainEventHandler<TDomainEvent> where TDomainEvent : DomainEvent
    {
        private readonly Action<TDomainEvent> action;

        public static DomainEventHandler<TDomainEvent> Empty
            => new((TDomainEvent domainEvent) => { });

        public DomainEventHandler(Action<TDomainEvent> action)
            => this.action = action;

        public void HandleEvent(TDomainEvent domainEvent)
            => action(domainEvent);
    }
}
