using System;
using NServiceBus;
 
namespace CQRS.NSB.EventProcessor
{
	public partial class EndpointConfig : IConfigureThisEndpoint, AsA_Server    
	{
    }
}