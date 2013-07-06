using System;
using NServiceBus;
using CQRSNSB.Contract.Elections;


namespace CQRS.NSB.EventProcessor.Elections
{
    public partial class ElectionCreatedProcessor : IHandleMessages<ElectionCreated>
    {
		
		public void Handle(ElectionCreated message)
		{
			this.HandleImplementation(message);
		}

		partial void HandleImplementation(ElectionCreated message);

		public IBus Bus { get; set; }

    }
}