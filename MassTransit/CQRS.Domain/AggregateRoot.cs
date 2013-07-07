using System;
using System.Collections.Generic;
using System.Linq;
using CQRS.Interfaces.Events;

namespace CQRS.Domain
{
    public abstract class AggregateRoot : IAggregateRoot //, IEquatable<AggregateRoot>
    {
        private readonly List<IEvent> _changes = new List<IEvent>();

        public abstract Guid Id { get; }
        public int LastEventVersion { get; internal set; }
        public bool HasUncommittedChanges 
        {
            get { return _changes.Any(); }
        }

        protected void ApplyEvent(IEvent @event, Action<IEvent> applyDelegate)
        {
            _changes.Add(@event);
            @event.Version = ++LastEventVersion;
            applyDelegate(@event);
        }

        public IEnumerable<IEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void Initialize(IEnumerable<IEvent> eventHistory)
        {
            if(HasUncommittedChanges)
                throw new InvalidOperationException("Cannot intialize an aggregate root that contains uncommitted changes.");

            InitializeFromHistory(eventHistory);
        }

        protected internal abstract void InitializeFromHistory(IEnumerable<IEvent> eventHistory);
        
        public IEnumerable<IEvent> NoEvents 
        {
            get { return new IEvent[0]; }
        }
    }
}
