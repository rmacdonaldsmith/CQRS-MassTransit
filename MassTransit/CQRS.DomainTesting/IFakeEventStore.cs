using System.Collections.Generic;
using CQRS.Common;
using CQRS.Interfaces.Events;

namespace CQRS.DomainTesting
{
    public interface IFakeEventStore : IEventStore
    {
        IEnumerable<IEvent> PeakChanges();
    }
}