using System.Collections.Generic;
using MHM.WinFlexOne.CQRS;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace CQRS.DomainTesting
{
    public interface IFakeEventStore : IEventStore
    {
        IEnumerable<IEvent> PeakChanges();
    }
}