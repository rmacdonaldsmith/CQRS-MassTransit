using System;

namespace MHM.WinFlexOne.CQRS.Domain.Repositories
{
    public class Repository : IRepository
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

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : AggregateRoot, new()
        {
            var aggregate = new TAggregate();

            aggregate.InitializeFromHistory(_eventStore.GetForAggregate(id));

            return aggregate;
        }

        public TAggregate GetById<TAggregate>(Guid id, int revisionNumber) where TAggregate : AggregateRoot, new()
        {
            throw new NotImplementedException();
        }
    }
}
