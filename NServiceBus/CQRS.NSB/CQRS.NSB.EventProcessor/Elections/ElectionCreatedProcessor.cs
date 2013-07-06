using System;
using NServiceBus;
using CQRSNSB.Contract.Elections;


namespace CQRS.NSB.EventProcessor.Elections
{
	public partial class ElectionCreatedProcessor
	{
		
        partial void HandleImplementation(ElectionCreated message)
        {
            // Implement your handler logic here.
            Console.WriteLine("Elections received " + message.GetType().Name);
        }

	}
}