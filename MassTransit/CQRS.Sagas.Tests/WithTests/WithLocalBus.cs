using System;
using System.Collections.Generic;
using System.Reflection;
using Magnum.Reflection;
using Magnum.StateMachine;
using MassTransit.Saga;
using MassTransit.Testing;
using MassTransit.Testing.TestInstanceConfigurators;
using NUnit.Framework;

namespace CQRS.Sagas.Tests.WithTests
{
    public abstract class WithLocalBus<TSaga> where TSaga : SagaStateMachine<TSaga>, ISaga
    {
        private SagaTest<BusTestScenario, TSaga> _test;

        public SagaTest<BusTestScenario, TSaga> Test
        {
            get { return _test; }
        }

        protected TSaga CreateSagaInState(Guid correlationId, State state)
        {
            var saga = FastActivator<TSaga>.Create(correlationId);
            var currentStateField = typeof(StateMachine<TSaga>).GetField("_currentState", BindingFlags.Instance | BindingFlags.NonPublic);
            if (currentStateField == null)
                throw new InvalidOperationException("No _currentState field found. Is this really a StateMachineSaga?");

            currentStateField.SetValue(saga, state);
            return saga;
        }

        protected abstract IEnumerable<TSaga> Given();

        [SetUp]
        public void Initialize()
        {
            _test = TestFactory
                .ForSaga<TSaga>()
                .InSingleBusScenario()
                .New(sagaConfigurator =>
                    {
                        var sagas = Given();

                        var sagaRepository = new InMemorySagaRepository<TSaga>();
                        foreach (var saga in sagas)
                        {
                            sagaRepository.Add(saga);
                        }

                        sagaConfigurator.UseSagaRepository(sagaRepository);

                        When(sagaConfigurator);
                    });

            _test.Execute();
        }

        protected abstract void When(SagaTestInstanceConfigurator<BusTestScenario, TSaga> sagaConfigurator);

        [TearDown]
        public void Teardown()
        {
            _test.Dispose();
        }

    }
}
