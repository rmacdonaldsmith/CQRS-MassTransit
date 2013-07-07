using System;

namespace CQRS.Domain.Repositories
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(AggregateRoot aggregate, int expectedRevisionNumber);
        //TAggregate GetById<TAggregate>(Guid id) where TAggregate : AggregateRoot, new();
        T GetById(Guid id, int revisionNumber);
    }
}
