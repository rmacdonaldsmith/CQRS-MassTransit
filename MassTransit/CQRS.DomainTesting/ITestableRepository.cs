using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Domain;
using MHM.WinFlexOne.CQRS.Domain.Repositories;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace CQRS.DomainTesting
{
    public interface ITestableRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        IEnumerable<IEvent> SavedEvents { get; }
    }
}