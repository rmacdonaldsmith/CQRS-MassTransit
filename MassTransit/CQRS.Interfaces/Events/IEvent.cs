namespace CQRS.Interfaces.Events
{
    public interface IEvent : IMessage
    {
        int Version { get; set; }
    }
}
