using Autofac;
using NUnit.Framework;

namespace MHM.WinFlexOne.CQRS.AutoFacModuleTests
{
    [TestFixture]
    public class Log4NetModuleTests
    {
        [Test]
        public void ModuleTest()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new LogInjectionModule());
            builder.RegisterType<LogTestClass>().AsSelf();

            var container = builder.Build();

            var objectThatLogs = container.Resolve<LogTestClass>();

            Assert.IsNotNull(objectThatLogs);
            Assert.IsNotNull(objectThatLogs.Log);
        }
    }
}
