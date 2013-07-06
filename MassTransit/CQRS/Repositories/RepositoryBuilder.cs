using System;
using EventStore;
using MHM.WinFlexOne.CQRS.Domain;

namespace MHM.WinFlexOne.CQRS.Repositories
{
    public interface IRepositoryBuilderPersistDelegate
    {
        IRepositoryBuilderPersistDelegate CreatePersistUsing(Action<Guid, AggregateRoot> persistenceDelegate);

        IRepositoryBuilderPersistDelegate CreateRetrieveUsing(Func<Guid, AggregateRoot> retrieveDelegate);
    }

    public interface IRepositoryBuilderUseEventStore
    {
        IRepositoryBuilderUseEventStore CreateUsingEventStore(IEventStore eventStore);
    }

    public class RepositoryBuilder<TAggregate> : IRepositoryBuilderPersistDelegate, IRepositoryBuilderUseEventStore where TAggregate : AggregateRoot, new()
    {
        private Repository<TAggregate> m_repository;

        public Repository<TAggregate> Build()
        {
            return m_repository;
        }

        public IRepositoryBuilderUseEventStore CreateUsingEventStore(IEventStore eventStore)
        {
            m_repository = new Repository<TAggregate>(eventStore);
            return this;
        }

        public IRepositoryBuilderPersistDelegate CreatePersistUsing(Action<Guid, AggregateRoot> persistenceDelegate)
        {
            return this;
        }

        public IRepositoryBuilderPersistDelegate CreateRetrieveUsing(Func<Guid, AggregateRoot> retrieveDelegate)
        {
            return this;
        }
    }
}
