using Magnum.TestFramework;
using MassTransit;
using MassTransit.TestFramework.Fixtures;

namespace CQRS.Sagas.Tests.TestSaga
{
    [Scenario]
    public class Given_a_simple_saga_does_not_exist :
        SagaTestFixture<SimpleTestSaga>
    {
        [Given]
        public void A_simple_saga_does_not_exist()
        {
            LocalBus.SubscribeSaga<SimpleTestSaga>(Repository);
        }
    }
}
