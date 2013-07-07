using System;
using System.Threading;
using CQRS.Commands;
using CQRS.Messages.Events;
using MassTransit;
using MassTransit.Services.Timeout.Messages;

namespace CQRS.Sagas.Tests
{
    public class ClaimRequestTestCoordinator :
        Consumes<TimeoutScheduled>.All,
        Consumes<TimeoutCancelled>.All,
        Consumes<RejectClaimRequest>.All
    {
        private readonly IServiceBus _bus;
        readonly ManualResetEvent _timeoutScheduled = new ManualResetEvent(false);
		readonly ManualResetEvent _timeoutCancelled = new ManualResetEvent(false);
        readonly ManualResetEvent _claimRejected = new ManualResetEvent(false);
        Guid _correlationId = Guid.Empty;
        private readonly TimeSpan _timeout;

        public ClaimRequestTestCoordinator(IServiceBus bus, int timeoutMs)
        {
            _bus = bus;
            _timeout = TimeSpan.FromMilliseconds(timeoutMs);
        }

        public string CorrelationId { get { return _correlationId.ToString(); }}

        //to start testing, the coordinator has been instructed to send this message on the bus
        //we then wait until we get the ScheduleTimeout message back from the Saga (see the Consumes() method, where we signal our threading event)
        public bool CreateClaimRequest(int amount, string claimtype, DateTime serviceDate, string provider, string reason)
        {
            var requestCommand = new ClaimRequestCreatedPendingVerificationEvent();
            requestCommand.Amount = amount;
            requestCommand.ClaimType = claimtype;
            requestCommand.DateOfTransaction = serviceDate;
            requestCommand.ProviderName = provider;
            requestCommand.Reason = reason;
            //requestCommand.ClaimRequestId = requestCommand.CorrelationId.ToString();
            _bus.Publish(requestCommand);

            return _timeoutScheduled.WaitOne(_timeout);
        }

        public void Consume(TimeoutScheduled message)
        {
            _correlationId = message.CorrelationId; //we need to remember the correlation id of the Saga - it comes to us in the TimeoutScheduled message
            _timeoutScheduled.Set();
        }

        public bool VerifyCardUse()
        {
            var cardUseVerified = new CardUseVerifiedEvent
                {
                    ClaimRequestId = CorrelationId,
                };

            _bus.Publish(cardUseVerified);

            //wait for the TimeoutCancelled event from the "timeout service"
            return _timeoutCancelled.WaitOne(_timeout);
        }

        public void Consume(TimeoutCancelled message)
        {
            _timeoutCancelled.Set();
        }

        public bool Timeout()
        {
            _bus.Publish(new TimeoutExpired{CorrelationId = new Guid(CorrelationId)});

            return _claimRejected.WaitOne(_timeout);
        }

        public void Consume(RejectClaimRequest message)
        {
            _claimRejected.Set();
        }
    }
}
