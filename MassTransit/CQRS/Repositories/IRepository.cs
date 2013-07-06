using System;

namespace MHM.WinFlexOne.CQRS.Domain.Repositories
{
    public interface IRepository
    {
        void Save(AggregateRoot aggregate, int expectedRevisionNumber);
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : AggregateRoot, new();
        TAggregate GetById<TAggregate>(Guid id, int revisionNumber) where TAggregate : AggregateRoot, new();
    }
}
