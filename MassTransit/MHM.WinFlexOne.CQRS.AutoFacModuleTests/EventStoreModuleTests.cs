using Autofac;
using NUnit.Framework;

namespace MHM.WinFlexOne.CQRS.AutoFacModuleTests
{
    [TestFixture]
    public class EventStoreModuleTests
    {
        [Test]
        public void Run()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new EventStoreInjectionModule{PersistenceType = EventStorePersistenceEnum.InMemory});
            var container = builder.Build();

            var eventStore = container.Resolve<IEventStore>();

            Assert.IsNotNull(eventStore);
        }
    }
}
