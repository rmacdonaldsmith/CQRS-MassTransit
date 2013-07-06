using System;
using CQRSNSB.Contract.Elections;
using NServiceBus;
using CQRSNSB.InternalMessages.Elections;


namespace CQRS.NSB.CommandProcessor.Elections
{
	public partial class CreateElectionProcessor
	{
		
        partial void HandleImplementation(CreateElection message)
        {
            // Implement your handler logic here.
            Console.WriteLine("Elections received " + message.GetType().Name);
            //throw new Exception("Something went wrong!");
            Bus.Publish<ElectionCreated>();
        }

	}
}