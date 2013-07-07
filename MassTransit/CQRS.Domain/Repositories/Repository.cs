using System;
using CQRS.Common;
using MHM.WinFlexOne.CQRS;

namespace CQRS.Domain.Repositories
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;

        public Repository(IEventStore storage)
        {
            _eventStore = storage;
        }

        public void Save(AggregateRoot aggregate, int expectedRevisionNumber)
        {
            _eventStore.Save(aggregate.Id, aggregate.GetUncommittedChanges(), expectedRevisionNumber);
        }

        public T GetById(Guid id, int revisionNumber)
        {
            var aggregate = new T();

            aggregate.InitializeFromHistory(_eventStore.GetForAggregate(id));

            return aggregate;
        }
    }
}
