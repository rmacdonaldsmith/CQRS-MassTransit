using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using MHM.WinFlexOne.CQRS.Events;
using MassTransit;
using MassTransit.Saga;
using MassTransit.Services.Timeout.Messages;
using MassTransit.TestFramework;
using MassTransit.Tests.TextFixtures;
using NUnit.Framework;

namespace CQRS.Sagas.Tests
{

    public class When_creating_a_new_claim_request : LoopbackLocalAndRemoteTestFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            var sagaRepository = new InMemorySagaRepository<ClaimRequestSaga>();

            //these subscriptions are to replace the Timeout Service
            RemoteBus.SubscribeHandler<ScheduleTimeout>(
                x =>
                {
                    RemoteBus.ShouldHaveSubscriptionFor<TimeoutScheduled>();
                    RemoteBus.Publish(new TimeoutScheduled{CorrelationId = x.CorrelationId, TimeoutAt = x.TimeoutAt});
                });

            RemoteBus.SubscribeHandler<CancelTimeout>(
                x =>
                    {
                        RemoteBus.ShouldHaveSubscriptionFor<TimeoutCancelled>();
                        RemoteBus.Publish(new TimeoutCancelled{CorrelationId = x.CorrelationId});
                    }
                );

            //RemoteBus.SubscribeHandler<ClaimRequestCreatedPendingVerificationEvent>(x =>
            //    {
            //        Debug.WriteLine("Claim Created Pending Verification event received.");
            //    });

            RemoteBus.SubscribeSaga(sagaRepository);

            //event messages that the Saga is subscribed to
            LocalBus.ShouldHaveSubscriptionFor<ClaimRequestCreatedPendingVerificationEvent>();
            LocalBus.ShouldHaveSubscriptionFor<CardUseVerifiedEvent>();
            LocalBus.ShouldHaveSubscriptionFor<TimeoutExpired>();
        }

        protected override void TeardownContext()
        {
            if(LocalBus != null)
                LocalBus.Dispose();

            base.TeardownContext();
        }

        [Test]
        public void Then_a_new_timeout_is_scheduled_and_cancelled_when_the_claim_is_verified()
        {
            var timer = Stopwatch.StartNew();
            var controller = new ClaimRequestTestCoordinator(LocalBus, 4000);

            using (LocalBus.SubscribeInstance(controller).Disposable())
            {
                RemoteBus.ShouldHaveSubscriptionFor<ClaimRequestCreatedPendingVerificationEvent>();
                RemoteBus.ShouldHaveSubscriptionFor<CardUseVerifiedEvent>();

                bool complete = controller.CreateClaimRequest(1200, "ClaimType", DateTime.Today.AddDays(-10), "provider", "reason");
                Assert.IsTrue(complete, "No timeout scheduled message was received for this claim request.");

                timer.Stop();
                Debug.WriteLine(string.Format("Time to handle message: {0}ms", timer.ElapsedMilliseconds));

                complete = controller.VerifyCardUse();
                Assert.IsTrue(complete, "The timeout should have been cancelled and the saga should be complete.");
            }
        }

        [Test]
        public void Then_the_claim_is_rejected_when_no_verification_is_received_within_the_timeout()
        {
            var timer = Stopwatch.StartNew();
            var controller = new ClaimRequestTestCoordinator(LocalBus, 1000);

            using (LocalBus.SubscribeInstance(controller).Disposable())
            {
                RemoteBus.ShouldHaveSubscriptionFor<ClaimRequestCreatedPendingVerificationEvent>();

                bool complete = controller.CreateClaimRequest(1200, "ClaimType", DateTime.Today.AddDays(-10), "provider", "reason");
                Assert.IsTrue(complete, "There should be a timeout scheduled for this claim request.");

                timer.Stop();
                Debug.WriteLine(string.Format("Time to handle message: {0}ms", timer.ElapsedMilliseconds));

                complete = controller.Timeout();
                Assert.IsTrue(complete, "The claim request timed out but the claim was not rejected.");
            }
        }
    }
}
