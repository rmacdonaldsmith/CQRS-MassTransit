using CQRS.Commands;
using MassTransit;
using Nancy;

namespace CQRS.UI.Web.Nancy.Modules
{
    public class MessagingModule : NancyModule
    {
        public MessagingModule()
        {
            Get["/send"] = parameters =>
                               {
                                   return View["SendMessage.sshtml"];
                               };

            Get["/send/command/{messagetype}"] = parameters =>
                                                     {
                                                         //code to send the message on Mass Transit
                                                         var queueName = "rabbitmq://localhost/commandqueue";
                                                         var bus = ServiceBusFactory.New(configurator =>
                                                         {
                                                             configurator.UseRabbitMq();
                                                             configurator.ReceiveFrom(queueName);
                                                         });

                                                         bus.Publish(new MakeAnElection
                                                                                         {
                                                                                             AdministratorCode = "admcode",
                                                                                             CompanyCode = "cocode",
                                                                                             ParticipantId = "empnum",
                                                                                             ElectionAmount = 1200,
                                                                                             ElectionReason = "reason",
                                                                                         });

                                                         return View["SendMessage.sshtml"];
                                                     };
        }
    }
}