namespace CQRS.Interfaces.Events
{
    public interface Handles<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent args);
    }
}
