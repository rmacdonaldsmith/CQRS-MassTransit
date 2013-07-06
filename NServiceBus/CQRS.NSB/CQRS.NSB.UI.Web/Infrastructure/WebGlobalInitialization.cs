using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace CQRS.NSB.UI.Web
{
    public static class WebGlobalInitialization
    {
        public static IBus InitializeNServiceBus()
        {
            return NServiceBus.Configure.With()
                .Log4Net()
                .DefaultBuilder()
                .XmlSerializer()
                .MsmqTransport()
                    .IsTransactional(false)
                    .PurgeOnStartup(false)
                .UnicastBus()
                    .ImpersonateSender(false)
                .CreateBus()
                .Start();
        }
    }
}
