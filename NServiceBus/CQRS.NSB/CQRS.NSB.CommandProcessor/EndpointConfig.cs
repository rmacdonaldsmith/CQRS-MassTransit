using System;
using NServiceBus;
 
namespace CQRS.NSB.CommandProcessor
{
	public partial class EndpointConfig : IConfigureThisEndpoint, AsA_Publisher    
	{
    }
}