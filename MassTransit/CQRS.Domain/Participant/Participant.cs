using System;
using System.Collections.Generic;
using MHM.WinFlexOne.CQRS.Interfaces.Events;

namespace MHM.WinFlexOne.CQRS.Domain.Participant
{
    public class Participant : AggregateRoot
    {
        private Guid m_participantId;

        public Participant()
        {
            //empty constructor required - nothing to do in here
        }

        public override Guid Id
        {
            get { return m_participantId; }
        }

        protected internal override void InitializeFromHistory(IEnumerable<IEvent> eventHistory)
        {
            throw new NotImplementedException();
        }

        public void RegisterNewParticipant(object command)
        {

        }
    }
}
