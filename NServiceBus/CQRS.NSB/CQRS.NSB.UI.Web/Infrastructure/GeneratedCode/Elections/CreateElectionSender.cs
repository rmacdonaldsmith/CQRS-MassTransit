using System;
using NServiceBus;
using NServiceBus.Config;
using CQRSNSB.InternalMessages.Elections;

namespace CQRS.NSB.UI.Web.Elections
{
    public partial class CreateElectionSender
    {
        public void Send(CreateElection message)
		{
			Bus.Send(message);	
		}

        public IBus Bus { get; set; }
    }


   public class CreateElectionSenderRegistration : INeedInitialization
   {
       public void Init()
       {
           Configure.Instance.Configurer.ConfigureComponent<CreateElectionSender>(DependencyLifecycle.InstancePerCall);
       }
   }

}