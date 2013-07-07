using System.Collections.Generic;
using CQRS.Domain;
using CQRS.Domain.Repositories;
using CQRS.Interfaces.Events;

namespace CQRS.DomainTesting
{
    public interface ITestableRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        IEnumerable<IEvent> SavedEvents { get; }
    }
}