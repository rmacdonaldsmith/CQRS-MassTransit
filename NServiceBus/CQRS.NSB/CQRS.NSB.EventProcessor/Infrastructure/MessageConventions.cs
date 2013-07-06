using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;

namespace CQRS.NSB.EventProcessor
{
    public class MessageConventions : IWantToRunBeforeConfiguration
    {
        public void Init()
        {
            Configure.Instance
            .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("CQRSNSB.InternalMessages"))
            .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("CQRSNSB.Contract"));
        }
    }
}

