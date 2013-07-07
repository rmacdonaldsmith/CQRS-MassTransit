using System;
using System.Collections.Generic;
using CQRS.Interfaces.Events;
using CQRS.Messages.Events;
using Common;

namespace CQRS.Domain.Election
{
    public sealed class ElectionState : AggregateStateBase
    {
        private Guid _id;
        private DateTime? _terminationDate;
        private Money _electionAmount;
        private string _qualifyingEvent;

        public override Guid Id
        {
            get { return _id; }
        }

        public DateTime? TerminationDate
        {
            get { return _terminationDate; }
        }

        public Money ElectionAmount
        {
            get { return _electionAmount; }
        }

        public string QualifyingEvent
        {
            get { return _qualifyingEvent; }
        }

        public ElectionState(IEnumerable<IEvent> events)
        {
            base.Register<ElectionMadeEvent>(evnt => When(evnt));
            base.Register<ElectionTerminatedEvent>(evnt => When(evnt));
            base.Register<ElectionAmountChangedEvent>(evnt => When(evnt));

            foreach (var @event in events)
            {
                Apply(@event);
            }
        }

        private void When(ElectionMadeEvent @event)
        {
            //The command has been validated and we are now going to apply
            //the state change to this instance.
            //This state transition CAN NOT FAIL now.
            //Note: this method may be called in either of these cases:
            //  1 - the command is being processed for the first time
            //  2 - the event has been replayed from the event store
            _id = new Guid(@event.Id);
            _electionAmount = @event.ElectionAmount.Dollars();
        }

        private void When(ElectionTerminatedEvent @event)
        {
            _terminationDate = @event.TerminatedDate;
        }

        private void When(ElectionAmountChangedEvent @event)
        {
            _electionAmount = @event.NewElectionAmount.Dollars();
            _qualifyingEvent = @event.QualifyingEvent;
        }
    }
}
