using CQRS.Interfaces.Events;

namespace CQRS.Messages.Events
{
    public partial class TimeoutElapsedEvent : IEvent
    {
        public int Version { get; set; }

        public string CorrelationId { get; set; }

        public int ElapsesMs { get; set; }
    }
}
