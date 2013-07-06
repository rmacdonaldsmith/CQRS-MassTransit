using System;
using NServiceBus;
using CQRSNSB.InternalMessages.Elections;


namespace CQRS.NSB.CommandProcessor.Elections
{
    public partial class CreateElectionProcessor : IHandleMessages<CreateElection>
    {
		
		public void Handle(CreateElection message)
		{
			this.HandleImplementation(message);
		}

		partial void HandleImplementation(CreateElection message);

		public IBus Bus { get; set; }

    }
}